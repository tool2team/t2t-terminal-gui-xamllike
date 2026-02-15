using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Terminal.Gui.XamlLike
{
    /// <summary>
    /// Incremental source generator for Terminal.Gui XAML-like files
    /// </summary>
    [Generator]
    public sealed class TuiXamlGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Register the generator for .tui.xaml files
            var xamlFiles = context.AdditionalTextsProvider
                .Where(file => file.Path.EndsWith(".tui.xaml"))
                .Select((file, cancellationToken) => new
                {
                    Path = file.Path,
                    Content = file.GetText(cancellationToken)?.ToString() ?? string.Empty
                });

            // Combine with compilation to access semantic model if needed
            var compilationAndFiles = context.CompilationProvider.Combine(xamlFiles.Collect());

            // Generate source code for each XAML file
            context.RegisterSourceOutput(compilationAndFiles, (context, source) =>
            {
                var compilation = source.Left;
                var files = source.Right;
                
                foreach (var file in files)
                {
                    ProcessXamlFile(context, compilation, file.Path, file.Content);
                }
            });
        }

        private static void ProcessXamlFile(
            SourceProductionContext context, 
            Compilation compilation, 
            string filePath, 
            string content)
        {
            // Add diagnostic to verify generator is running
            context.ReportDiagnostic(Diagnostic.Create(
                TuiDiagnostics.GeneratorProcessingFile, 
                Location.None,
                filePath));

            // Parse the XAML content
            var parseResult = XamlParser.Parse(content, filePath);

            if (!parseResult.IsSuccess)
            {
                if (parseResult.Error != null)
                {
                    context.ReportDiagnostic(parseResult.Error.ToDiagnostic());
                }
                return;
            }

            var document = parseResult.Value;

            // Validate the document
            var validationErrors = XamlParser.Validate(document!);
            foreach (var error in validationErrors)
            {
                context.ReportDiagnostic(error.ToDiagnostic());
            }

            // If there are errors, don't generate code
            if (validationErrors.Any(d => d.Descriptor.DefaultSeverity == DiagnosticSeverity.Error))
            {
                return;
            }

            // Resolve DataType to property name if it's a type
            var dataTypePropertyName = ResolveDataTypeToPropertyName(compilation, document!);

            // Generate the C# code
            var diagnostics = new List<Diagnostic>();
            var generatedCode = GenerateCode(document!, dataTypePropertyName, diagnostics);

            // Report any diagnostics that occurred during code generation
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }

            context.ReportDiagnostic(Diagnostic.Create(
                TuiDiagnostics.GeneratorCodeLength, 
                Location.None,
                generatedCode.Length));

            // Add the generated source
            var sourceText = SourceText.From(generatedCode, Encoding.UTF8);
            var fileName = System.IO.Path.GetFileName(filePath) + ".g.cs";

            context.AddSource(fileName, sourceText);
        }

        /// <summary>
        /// Resolves x:DataType (which may be a full type name) to the actual property name in the class
        /// </summary>
        private static string? ResolveDataTypeToPropertyName(Compilation compilation, XamlDocument document)
        {
            var dataType = document.DataType;
            if (string.IsNullOrEmpty(dataType))
                return null;

            // If dataType doesn't contain a dot, assume it's already a property name
            if (!dataType!.Contains("."))
                return dataType;

            // dataType is a full type name like "MvvmApp.ViewModels.MainViewModel"
            // Find the class being generated
            var className = document.ClassName;
            if (string.IsNullOrEmpty(className))
                return null;

            var classSymbol = compilation.GetTypeByMetadataName(className!);
            if (classSymbol == null)
                return null;

            // Find a property of the specified type
            var members = classSymbol.GetMembers().OfType<IPropertySymbol>();
            foreach (var property in members)
            {
                var propertyTypeName = property.Type.ToDisplayString();
                if (propertyTypeName == dataType)
                {
                    return property.Name;
                }
            }

            // Not found - return null and use explicit paths
            return null;
        }

        private static string GenerateCode(XamlDocument document, string? resolvedDataTypePropertyName, List<Diagnostic> diagnostics)
        {
            var emitter = new CodeEmitter();
            return emitter.GenerateClass(document, resolvedDataTypePropertyName, diagnostics);
        }
    }

    /// <summary>
    /// Generates C# code from parsed XAML documents
    /// </summary>
    public sealed class CodeEmitter
    {
        private readonly StringBuilder _code = new();
        private int _indentLevel = 0;
        private string? _dataTypePropertyName;
        private Dictionary<XamlElement, string> _elementToFieldName = new();
        private Dictionary<string, int> _anonymousControlCounter = new();
        private List<Diagnostic>? _diagnostics;
        private string? _sourceFilePath;

        /// <summary>
        /// Creates a diagnostic with location information from the XAML file
        /// </summary>
        private void ReportDiagnostic(DiagnosticDescriptor descriptor, XamlElement element, params object[] messageArgs)
        {
            if (_diagnostics == null || _sourceFilePath == null)
                return;

            Location location;
            if (element.LineNumber > 0)
            {
                // Create location with line/column info
                var linePosition = new Microsoft.CodeAnalysis.Text.LinePosition(
                    element.LineNumber - 1,  // LinePosition is 0-based
                    element.LinePosition - 1);
                var span = new Microsoft.CodeAnalysis.Text.LinePositionSpan(linePosition, linePosition);
                location = Location.Create(_sourceFilePath, default, span);
            }
            else
            {
                location = Location.None;
            }

            _diagnostics.Add(Diagnostic.Create(descriptor, location, messageArgs));
        }

        public string GenerateClass(XamlDocument document, string? resolvedDataTypePropertyName, List<Diagnostic> diagnostics)
        {
            _code.Clear();
            _indentLevel = 0;
            _elementToFieldName.Clear(); // Reset for each document
            _anonymousControlCounter = new(); // Reset counter
            _diagnostics = diagnostics; // Store diagnostic list
            _sourceFilePath = document.SourceFilePath; // Store source file path for diagnostics

            var className = document.ClassName;
            var namespaceName = GetNamespace(className!);
            var simpleClassName = GetSimpleClassName(className!);

            // Use resolved property name if available, otherwise fall back to document.DataType
            _dataTypePropertyName = resolvedDataTypePropertyName ?? document.DataType;

            // File header
            AppendLine("// <auto-generated />");
            AppendLine("#nullable enable");
            AppendLine();

            // Using statements
            AppendLine("using System;");
            AppendLine("using System.ComponentModel;");
            AppendLine("using Terminal.Gui;");
            AppendLine("using Terminal.Gui.Views;");
            AppendLine("using Terminal.Gui.ViewBase;");
            AppendLine();

            // Namespace
            if (!string.IsNullOrEmpty(namespaceName))
            {
                AppendLine($"namespace {namespaceName}");
                AppendLine("{");
                _indentLevel++;
            }

            // Determine base class from root element (use full name for inheritance)
            var rootElementType = Mappings.GetFullTypeName(document.RootElement.Name);

            if (rootElementType == null)
            {
                _diagnostics?.Add(Diagnostic.Create(
                    TuiDiagnostics.UnknownRootElementType,
                    Location.None,
                    document.RootElement.Name
                ));
                rootElementType = "object"; // Fallback to allow compilation to continue
            }

            if (!Mappings.IsContainer(document.RootElement.Name))
            {
                _diagnostics?.Add(Diagnostic.Create(
                    TuiDiagnostics.RootElementNotContainer,
                    Location.None,
                    document.RootElement.Name
                ));
            }
            // Class declaration with base class
            AppendLine($"partial class {simpleClassName} : {rootElementType}");
            AppendLine("{");
            _indentLevel++;

            // Collect bindings first to know which controls need fields
            // This also builds the element-to-field-name map for bound controls without x:Name
            var bindings = CollectBindings(document.RootElement, _dataTypePropertyName);

            // Generate fields for named controls
            GenerateFields(document.RootElement, bindings, isRoot: false);

            // Generate fields for bound controls without names
            GenerateFieldsForBoundControls(bindings);

            // Generate fields for anonymous controls without bindings
            GenerateFieldsForAnonymousControls(document.RootElement, isRoot: true);

            AppendLine();

            // Generate InitializeComponent method
            GenerateInitializeComponent(document);

            // Generate binding methods if needed
            if (bindings.Count > 0)
            {
                AppendLine();
                GenerateBindingMethods(bindings, _dataTypePropertyName);
            }

            _indentLevel--;
            AppendLine("}");

            if (!string.IsNullOrEmpty(namespaceName))
            {
                _indentLevel--;
                AppendLine("}");
            }

            return _code.ToString();
        }

        private void GenerateFields(XamlElement element, List<BoundControl> bindings, bool isRoot = false)
        {
            // Generate field if element has explicit x:Name
            if (!string.IsNullOrEmpty(element.XName))
            {
                var typeName = Mappings.GetFullTypeName(element.Name, element.XType);
                if (typeName == null)
                {
                    _diagnostics?.Add(Diagnostic.Create(
                        TuiDiagnostics.UnknownControlTypeInGeneration,
                        Location.None,
                        element.Name
                    ));
                    typeName = "object"; // Fallback
                }
                AppendLine($"private {typeName} {element.XName} = null!;");
            }

            foreach (var child in element.Children)
            {
                GenerateFields(child, bindings, isRoot: false);
            }

            // After processing all elements, generate fields for bound controls without explicit names
            // Only do this at the root level to avoid duplicates
            if (!isRoot) // Only once at top level
            {
                return;
            }
        }

        private void GenerateFieldsForBoundControls(List<BoundControl> bindings)
        {
            foreach (var control in bindings)
            {
                if (!control.HasExplicitName)
                {
                    var typeName = Mappings.GetFullTypeName(control.ElementName, control.GenericType);
                    if (typeName == null)
                    {
                        _diagnostics?.Add(Diagnostic.Create(
                            TuiDiagnostics.UnknownControlTypeInGeneration,
                            Location.None,
                            control.ElementName
                        ));
                        typeName = "object";
                    }
                    AppendLine($"private {typeName} {control.GetFieldName()} = null!;");
                }
            }
        }

        private void GenerateFieldsForAnonymousControls(XamlElement element, bool isRoot = false)
        {
            // Skip root element
            if (!isRoot)
            {
                // Generate fields for anonymous controls that don't already have a field
                if (string.IsNullOrEmpty(element.XName) && !_elementToFieldName.ContainsKey(element))
                {
                    // This element needs a generated field
                    var fieldName = GenerateAnonymousFieldName(element.Name);
                    _elementToFieldName[element] = fieldName;

                    var typeName = Mappings.GetFullTypeName(element.Name, element.XType);
                    if (typeName == null)
                    {
                        _diagnostics?.Add(Diagnostic.Create(
                            TuiDiagnostics.UnknownControlTypeInGeneration,
                            Location.None,
                            element.Name
                        ));
                        typeName = "object";
                    }
                    AppendLine($"private {typeName} {fieldName} = null!;");
                }
            }

            foreach (var child in element.Children)
            {
                GenerateFieldsForAnonymousControls(child, isRoot: false);
            }
        }

        private void GenerateInitializeComponent(XamlDocument document)
        {
            // Generate the complete method implementation
            AppendLine("private void InitializeComponent()");
            AppendLine("{");
            _indentLevel++;

            // If root element has x:Name, initialize it to 'this'
            if (!string.IsNullOrEmpty(document.RootElement.XName))
            {
                AppendLine($"{document.RootElement.XName} = this;");
            }

            // Generate root element (use x:Name if present, otherwise 'this')
            GenerateElementCode(document.RootElement, isRoot: true);

            // Setup bindings if any exist
            var bindings = CollectBindings(document.RootElement, _dataTypePropertyName);
            if (bindings.Count > 0)
            {
                AppendLine();
                AppendLine("SetupBindings();");
            }

            _indentLevel--;
            AppendLine("}");
        }

        private void GenerateElementCode(XamlElement element, bool isRoot = false)
        {
            // Get control type name with optional generic type
            var typeName = Mappings.GetFullTypeName(element.Name, element.XType);

            if (typeName == null)
            {
                _diagnostics?.Add(Diagnostic.Create(
                    TuiDiagnostics.UnknownControlTypeInGeneration,
                    Location.None,
                    element.Name
                ));
                typeName = "object"; // Fallback to allow generation to continue
            }

            // Determine the variable name to use
            string variableName;

            if (isRoot)
            {
                variableName = element.XName ?? "this";
            }
            else if (!string.IsNullOrEmpty(element.XName))
            {
                variableName = element.XName!;
            }
            else if (_elementToFieldName.TryGetValue(element, out var fieldName))
            {
                // Use the stored generated name (from CollectBindings or previous call)
                variableName = fieldName;
            }
            else
            {
                // Generate a unique name for anonymous controls
                // Use a simple counter instead of GUID for cleaner names
                variableName = GenerateAnonymousFieldName(element.Name);
                _elementToFieldName[element] = variableName;
            }

            // Initialize the control
            if (!isRoot && !string.IsNullOrEmpty(element.XName))
            {
                // Named control - assign to field
                AppendLine($"{element.XName} = new {typeName}();");
            }
            else if (!isRoot)
            {
                // Anonymous control (with or without binding) - assign to generated field
                AppendLine($"{variableName} = new {typeName}();");
            }

            // Set properties
            foreach (var kvp in element.PropertyAttributes)
            {
                var propName = kvp.Key;
                var value = kvp.Value;

                if (IsBindingExpression(value))
                {
                    // Skip bindings - they're handled separately
                    continue;
                }

                var csharpValue = GetPropertyValue(propName, value);
                AppendLine($"{variableName}.{propName} = {csharpValue};");
            }

            // Wire up events
            foreach (var kvp in element.EventAttributes)
            {
                var xamlEventName = kvp.Key;
                var handlerName = kvp.Value;

                // Check if event is obsolete
                var eventMapping = Mappings.GetEventMapping(element.Name, xamlEventName);
                if (eventMapping?.IsObsolete == true)
                {
                    // Emit diagnostic with file location and skip code generation
                    var obsoleteMessage = eventMapping.GetObsoleteMessage() ?? "";
                    ReportDiagnostic(
                        TuiDiagnostics.ObsoleteEvent,
                        element,
                        xamlEventName,
                        element.Name,
                        obsoleteMessage
                    );
                    continue; // Skip generating code for obsolete event
                }

                // Map XAML event name to actual Terminal.Gui event name
                AppendLine($"{variableName}.{xamlEventName} += {handlerName};");
            }

            // Add children
            foreach (var child in element.Children)
            {
                GenerateElementCode(child);

                // Determine child's variable name
                string childName;
                if (!string.IsNullOrEmpty(child.XName))
                {
                    childName = child.XName!;
                }
                else if (_elementToFieldName.TryGetValue(child, out var childFieldName))
                {
                    childName = childFieldName;
                }
                else
                {
                    // This shouldn't happen anymore, but fallback
                    childName = GenerateAnonymousFieldName(child.Name);
                    _elementToFieldName[child] = childName;
                }

                // Special handling: Buttons in Dialog should be added to Buttons collection
                if (element.Name == "Dialog" && child.Name == "Button")
                {
                    AppendLine($"{variableName}.AddButton({childName});");
                }
                else
                {
                    AppendLine($"{variableName}.Add({childName});");
                }
            }
        }

        private string GenerateAnonymousFieldName(string elementName)
        {
            if(_anonymousControlCounter.TryGetValue(elementName, out var count))
            {
                _anonymousControlCounter[elementName] = count + 1;
            }
            else
            {
                _anonymousControlCounter[elementName] = 1;
            }
            ;
            // Use a counter from BoundControl to keep names consistent
            return $"__{elementName}_{_anonymousControlCounter[elementName]}";
        }

        private void GenerateBindingMethods(List<BoundControl> bindings, string? dataType)
        {
            // Detect the binding source (from dataType or auto-detect from bindings)
            var bindingSource = dataType ?? DetectBindingSource(bindings);
            var needsPropertyChangedSource = bindingSource != null;

            AppendLine("private void SetupBindings()");
            AppendLine("{");
            _indentLevel++;

            // Only validate if we have a binding source (not self-binding)
            if (needsPropertyChangedSource)
            {
                AppendLine($"if ({bindingSource} == null) throw new InvalidOperationException(\"{bindingSource} property must be set before calling InitializeComponent\");");
                AppendLine();
            }

            // Generate initial binding setup
            foreach (var control in bindings)
            {
                foreach (var property in control.BoundProperties)
                {
                    var fieldName = control.GetFieldName();
                    var binding = property.Binding;

                    if (property.IsCommand)
                    {
                        // Command binding - wire up Accepting event to invoke command
                        AppendLine($"// Wire up command: {binding.SourceExpression}");
                        AppendLine($"{fieldName}.Accepting += (s, e) => {{");
                        _indentLevel++;
                        AppendLine($"if ({binding.SourceExpression}?.CanExecute(null) == true)");
                        _indentLevel++;
                        AppendLine($"{binding.SourceExpression}.Execute(null);");
                        _indentLevel--;
                        _indentLevel--;
                        AppendLine("};");
                    }
                    else
                    {
                        // Regular property binding - direct assignment
                        AppendLine($"{fieldName}.{property.PropertyName} = {binding.SourceExpression};");
                    }
                }
            }

            AppendLine();
            AppendLine("// Subscribe to property changes");

            if (needsPropertyChangedSource)
            {
                AppendLine($"{bindingSource}.PropertyChanged += OnViewModelPropertyChanged;");
            }
            else
            {
                AppendLine("PropertyChanged += OnViewModelPropertyChanged;");
            }

            // Generate TwoWay binding event handlers
            foreach (var control in bindings)
            {
                foreach (var property in control.BoundProperties.Where(p => p.Binding.Mode == BindingMode.TwoWay))
                {
                    var fieldName = control.GetFieldName();
                    var eventName = property.GetChangeEventName();
                    if (eventName != null)
                    {
                        AppendLine($"{fieldName}.{eventName} += On{fieldName}{property.PropertyName}Changed;");
                    }
                }
            }

            // Generate Command.CanExecuteChanged handlers
            foreach (var control in bindings)
            {
                foreach (var property in control.BoundProperties.Where(p => p.IsCommand))
                {
                    var fieldName = control.GetFieldName();
                    var binding = property.Binding;
                    AppendLine($"// Subscribe to CanExecuteChanged for {fieldName}");
                    AppendLine($"if ({binding.SourceExpression} != null)");
                    AppendLine("{");
                    _indentLevel++;
                    AppendLine($"{binding.SourceExpression}.CanExecuteChanged += (s, e) => {{");
                    _indentLevel++;
                    AppendLine($"{fieldName}.Enabled = {binding.SourceExpression}.CanExecute(null);");
                    _indentLevel--;
                    AppendLine("};");
                    AppendLine($"// Set initial Enabled state");
                    AppendLine($"{fieldName}.Enabled = {binding.SourceExpression}.CanExecute(null);");
                    _indentLevel--;
                    AppendLine("}");
                }
            }

            _indentLevel--;
            AppendLine("}");

            // Generate property change handler
            AppendLine();
            AppendLine("private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)");
            AppendLine("{");
            _indentLevel++;

            AppendLine("switch (e.PropertyName)");
            AppendLine("{");
            _indentLevel++;

            // Group bindings by the actual property name being watched
            var processedCases = new System.Collections.Generic.HashSet<string>();

            foreach (var control in bindings)
            {
                foreach (var property in control.BoundProperties)
                {
                    // Skip Commands - they don't need PropertyChanged updates
                    if (property.IsCommand)
                        continue;

                    // Extract the property name from the source expression
                    // For "ViewModel.Status" -> use "Status"
                    // For "Status" -> use "Status"
                    var sourceExpr = property.Binding.SourceExpression;
                    var lastDotIndex = sourceExpr.LastIndexOf('.');
                    var propertyName = lastDotIndex >= 0 ? sourceExpr.Substring(lastDotIndex + 1) : sourceExpr;

                    // Use the full source expression as the case key to avoid duplicates
                    var caseKey = $"case \"{propertyName}\":";

                    if (!processedCases.Contains(caseKey))
                    {
                        processedCases.Add(caseKey);
                        AppendLine(caseKey);
                        _indentLevel++;

                        // Find all controls that bind to this property and update them
                        foreach (var ctrl in bindings)
                        {
                            foreach (var prop in ctrl.BoundProperties)
                            {
                                var ctrlSourceExpr = prop.Binding.SourceExpression;
                                var ctrlLastDot = ctrlSourceExpr.LastIndexOf('.');
                                var ctrlPropName = ctrlLastDot >= 0 ? ctrlSourceExpr.Substring(ctrlLastDot + 1) : ctrlSourceExpr;

                                if (ctrlPropName == propertyName)
                                {
                                    AppendLine($"{ctrl.GetFieldName()}.{prop.PropertyName} = {prop.Binding.SourceExpression};");
                                }
                            }
                        }

                        AppendLine("break;");
                        _indentLevel--;
                    }
                }
            }

            _indentLevel--;
            AppendLine("}");
            _indentLevel--;
            AppendLine("}");

            // Generate TwoWay event handlers
            foreach (var control in bindings)
            {
                foreach (var property in control.BoundProperties.Where(p => p.Binding.Mode == BindingMode.TwoWay))
                {
                    var fieldName = control.GetFieldName();
                    var eventName = property.GetChangeEventName();

                    // Get the correct event argument type from EventMappings
                    string eventArgType = "System.EventArgs";
                    if (eventName != null)
                    {
                        var eventMapping = Mappings.GetEventMapping(control.ElementName, eventName);
                        if (eventMapping != null)
                        {
                            // Extract TEventArgs from EventHandler<TEventArgs> using regex
                            var delegateType = eventMapping.DelegateType;
                            var match = Regex.Match(delegateType, @"System\.EventHandler<(.+)>$");
                            if (match.Success)
                            {
                                eventArgType = match.Groups[1].Value;
                            }
                            else if (delegateType == "System.EventHandler")
                            {
                                eventArgType = "System.EventArgs";
                            }
                        }
                    }

                    AppendLine();
                    AppendLine($"private void On{fieldName}{property.PropertyName}Changed(object? sender, {eventArgType} e)");
                    AppendLine("{");
                    _indentLevel++;
                    AppendLine($"if ({property.Binding.SourceExpression} != {fieldName}.{property.PropertyName})");
                    _indentLevel++;
                    AppendLine($"{property.Binding.SourceExpression} = {fieldName}.{property.PropertyName};");
                    _indentLevel--;
                    _indentLevel--;
                    AppendLine("}");
                }
            }
        }

        private List<BoundControl> CollectBindings(XamlElement element, string? dataType)
        {
            var result = new List<BoundControl>();
            CollectBindingsRecursive(element, result, dataType);
            return result;
        }

        private void CollectBindingsRecursive(XamlElement element, List<BoundControl> result, string? dataType)
        {
            var boundProperties = new List<BoundProperty>();

            foreach (var kvp in element.PropertyAttributes)
            {
                var propName = kvp.Key;
                var value = kvp.Value;

                if (IsBindingExpression(value))
                {
                    var binding = BindingExpression.Parse(value, dataType);
                    if (binding != null)
                    {
                        // Pass the control type to BoundProperty so it can look up the change event name
                        boundProperties.Add(new BoundProperty(propName, binding, element.Name));
                    }
                }
            }

            if (boundProperties.Count > 0)
            {
                var control = new BoundControl(element.Name, element.XName, boundProperties, element.XType);
                result.Add(control);

                // Store mapping for elements without explicit names
                if (string.IsNullOrEmpty(element.XName))
                {
                    var fieldName = GenerateAnonymousFieldName(element.Name);
                    control.SetGeneratedFieldName(fieldName);
                    _elementToFieldName[element] = fieldName;
                }
            }

            foreach (var child in element.Children)
            {
                CollectBindingsRecursive(child, result, dataType);
            }
        }

        private void AppendLine(string line = "")
        {
            if (string.IsNullOrEmpty(line))
            {
                _code.AppendLine();
            }
            else
            {
                _code.AppendLine(new string(' ', _indentLevel * 4) + line);
            }
        }

        /// <summary>
        /// Detects the binding source from the bindings (e.g., "ViewModel", "Vm", or null for self-binding)
        /// </summary>
        private static string? DetectBindingSource(List<BoundControl> bindings)
        {
            foreach (var control in bindings)
            {
                foreach (var property in control.BoundProperties)
                {
                    var path = property.Binding.PropertyPath;
                    if (path.Contains("."))
                    {
                        // Extract the first part before the dot (e.g., "ViewModel" from "ViewModel.Status")
                        var parts = path.Split('.');
                        return parts[0];
                    }
                }
            }

            // No binding source found, means self-binding
            return null;
        }

        private static bool IsBindingExpression(string value) =>
            value.Trim().StartsWith("{Bind ") && value.Trim().EndsWith("}");

        private static string GetPropertyValue(string propName, string value)
        {
            // Check if property is a Terminal.Gui type (includes Pos, Dim, Key, Enum, etc.)
            if (Mappings.IsTerminalGuiType(propName))
            {
                var fullType = Mappings.GetFullyQualifiedType(propName);

                // Special handling for Pos and Dim types
                if (fullType == "Terminal.Gui.Views.Pos" || fullType == "Terminal.Gui.Views.Dim")
                {
                    // If value contains parentheses, it's already an expression like Dim.Fill()
                    if (value.Contains("("))
                    {
                        return value;
                    }

                    // For numeric values, convert to appropriate Terminal.Gui v2 types  
                    if (int.TryParse(value, out _))
                    {
                        return value; // Terminal.Gui v2 accepts int directly for Pos/Dim
                    }

                    // If not numeric, treat as expression
                    return value;
                }

                // For other Terminal.Gui types (Key, Enum, etc.), add full namespace
                if (fullType != null)
                {
                    // Handle values like "Key.F1" or "Key.Q.WithCtrl" or just "F1"
                    // Split by dots to analyze the structure
                    var parts = value.Split('.');

                    if (parts.Length == 1)
                    {
                        // Just the value: "F1" → "Terminal.Gui.Input.Key.F1"
                        return $"{fullType}.{value}";
                    }
                    else if (parts.Length >= 2)
                    {
                        // Has dots: "Key.F1" or "Key.Q.WithCtrl"
                        // Check if first part matches the type name
                        var typeShortName = fullType.Substring(fullType.LastIndexOf('.') + 1);

                        if (parts[0] == typeShortName)
                        {
                            // Remove the type prefix and add the full namespace
                            // "Key.F1" → "Terminal.Gui.Input.Key.F1"
                            // "Key.Q.WithCtrl" → "Terminal.Gui.Input.Key.Q.WithCtrl"
                            var valuePart = string.Join(".", parts.Skip(1));
                            return $"{fullType}.{valuePart}";
                        }
                        else
                        {
                            // Doesn't start with type name, prefix the whole thing
                            return $"{fullType}.{value}";
                        }
                    }
                }
                // Fallback: return as-is if we can't get the full type
                return value;
            }

            // Check if property is boolean (case-insensitive)
            if (Mappings.IsBooleanProperty(propName))
            {
                // Parse boolean values (case-insensitive)
                if (bool.TryParse(value, out var boolValue))
                {
                    return boolValue ? "true" : "false";
                }
                // If not a valid boolean, treat as expression
                return value;
            }

            // Check if property is float (like Fraction in ProgressBar)
            if (Mappings.IsFloatProperty(propName))
            {
                // Parse float values using invariant culture (decimal point)
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _))
                {
                    // Ensure float literal has 'f' suffix
                    return value.Contains('.') ? $"{value}f" : $"{value}.0f";
                }
                // If not a valid float, treat as expression
                return value;
            }

            // Check if property is numeric int (like SelectedItem)
            if (Mappings.IsIntProperty(propName) && int.TryParse(value, out _))
            {
                return value; // Return numeric value without quotes
            }

            // Regular string property
            return $"\"{value.Replace("\"", "\\\"")}\"";
        }

        private static string GetNamespace(string fullClassName)
        {
            var lastDot = fullClassName.LastIndexOf('.');
            return lastDot == -1 ? string.Empty : fullClassName.Substring(0, lastDot);
        }

        private static string GetSimpleClassName(string fullClassName)
        {
            var lastDot = fullClassName.LastIndexOf('.');
            return lastDot == -1 ? fullClassName : fullClassName.Substring(lastDot + 1);
        }
    }
}