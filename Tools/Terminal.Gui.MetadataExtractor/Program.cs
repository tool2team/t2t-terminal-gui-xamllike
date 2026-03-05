using System.Reflection;
using System.Text;
using Terminal.Gui;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Terminal.Gui.MetadataExtractor;

/// <summary>
/// Tool to extract metadata from Terminal.Gui Views for generating bindings and property mappings
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Terminal.Gui Metadata Extractor");
        Console.WriteLine("================================\n");

        // Default output path is the Mappings.cs in the main project
        // Use absolute path resolution from current directory
        var defaultPath = Path.Combine(
            Directory.GetCurrentDirectory(), 
            "..", "..", 
            "Terminal.Gui.XamlLike", 
            "Mappings.cs");
        var outputPath = args.Length > 0 ? args[0] : defaultPath;

        // Get all View types from Terminal.Gui
        var viewTypes = GetAllViewTypes();

        Console.WriteLine($"Found {viewTypes.Count} View types\n");

        // Extract metadata
        var metadata = ExtractMetadata(viewTypes);

        // Generate mapping code
        var code = GenerateMappingCode(metadata);

        // Write to file
        var fullOutputPath = Path.GetFullPath(outputPath);
        var directory = Path.GetDirectoryName(fullOutputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(fullOutputPath, code);

        Console.WriteLine($"\nGenerated mappings written to: {fullOutputPath}");
        Console.WriteLine($"Total Views: {metadata.Count}");
        Console.WriteLine($"Total unique properties: {metadata.SelectMany(m => m.Properties).DistinctBy(p => p.Name).Count()}");
        Console.WriteLine($"Total unique events: {metadata.SelectMany(m => m.Events).DistinctBy(e => e.Name).Count()}");
    }

    static List<Type> GetAllViewTypes()
    {
        var assembly = typeof(View).Assembly;
        var viewTypes = new List<Type>();

        foreach (var type in assembly.GetTypes())
        {
            // Include View and all subclasses
            if (typeof(View).IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
            {
                viewTypes.Add(type);
            }
        }

        return viewTypes.OrderBy(t => t.Name).ToList();
    }

    static List<ViewMetadata> ExtractMetadata(List<Type> viewTypes)
    {
        var metadata = new List<ViewMetadata>();

        foreach (var type in viewTypes)
        {
            Console.WriteLine($"Extracting metadata for: {type.Name}");

            var viewMeta = new ViewMetadata
            {
                TypeName = type.Name,
                FullTypeName = type.FullName ?? type.Name,
                Namespace = type.Namespace ?? "",
                IsContainer = IsContainerType(type),
                Properties = ExtractProperties(type),
                Events = ExtractEvents(type)
            };

            metadata.Add(viewMeta);
        }

        return metadata;
    }

    static bool IsContainerType(Type type)
    {
        // Check if type can contain child views
        // Views typically have an Add method for adding child views
        var addMethod = type.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, 
            new[] { typeof(View) }, null);

        if (addMethod == null)
            return false;

        // Additional heuristic: non-containers are typically input controls
        var nonContainers = new[] 
        { 
            "Label", "TextField", "TextView", "Button", "CheckBox", "RadioGroup", 
            "ProgressBar", "Slider", "DateField", "TimeField", "TextValidateField",
            "NumericUpDown", "SpinnerView", "ColorPicker", "ColorPicker16",
            "AttributePicker", "AutocompleteTextField", "CharMap", "HexView",
            "Line", "ScrollBar"
        };

        return !nonContainers.Contains(type.Name);
    }

    static List<PropertyMetadata> ExtractProperties(Type type)
    {
        var properties = new List<PropertyMetadata>();

        // Get properties from the type and all base types
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            // Skip indexed properties
            if (prop.GetIndexParameters().Length > 0)
                continue;

            // Skip properties without public setter
            if (prop.SetMethod == null || !prop.SetMethod.IsPublic)
                continue;

            // Skip if property is defined in a base class outside Terminal.Gui (like Component)
            if (prop.DeclaringType != null && 
                prop.DeclaringType != type &&
                !prop.DeclaringType.Namespace?.StartsWith("Terminal.Gui") == true)
                continue;

            var propMeta = new PropertyMetadata
            {
                Name = prop.Name,
                PropertyType = prop.PropertyType.Name,
                FullPropertyType = GetFullTypeName(prop.PropertyType),
                IsTerminalGuiType = IsTerminalGuiType(prop.PropertyType),
                IsBoolean = prop.PropertyType == typeof(bool),
                IsInt = prop.PropertyType == typeof(int),
                IsFloat = prop.PropertyType == typeof(float) || prop.PropertyType == typeof(double),
                IsString = prop.PropertyType == typeof(string),
                DeclaringType = prop.DeclaringType?.Name ?? type.Name
            };

            properties.Add(propMeta);
        }

        return properties.OrderBy(p => p.Name).ToList();
    }

    static List<EventMetadata> ExtractEvents(Type type)
    {
        var events = new List<EventMetadata>();

        foreach (var evt in type.GetEvents(BindingFlags.Public | BindingFlags.Instance))
        {
            var eventHandlerType = evt.EventHandlerType;
            if (eventHandlerType == null)
                continue;

            // Simplify the delegate type name for better readability
            var delegateTypeName = SimplifyDelegateType(eventHandlerType);

            var eventMeta = new EventMetadata
            {
                Name = evt.Name,
                DelegateType = delegateTypeName,
                IsObsolete = evt.GetCustomAttribute<ObsoleteAttribute>() != null
            };

            events.Add(eventMeta);
        }

        return events.OrderBy(e => e.Name).ToList();
    }

    static string SimplifyDelegateType(Type delegateType)
    {
        // For EventHandler<TEventArgs>, extract the generic argument
        if (delegateType.IsGenericType && delegateType.GetGenericTypeDefinition() == typeof(EventHandler<>))
        {
            var eventArgsType = delegateType.GetGenericArguments()[0];
            var simplifiedArg = GetSimplifiedFullTypeName(eventArgsType);
            return $"System.EventHandler<{simplifiedArg}>";
        }

        // For non-generic EventHandler
        if (delegateType == typeof(EventHandler))
        {
            return "System.EventHandler";
        }

        // For other delegate types (like NotifyCollectionChangedEventHandler)
        return delegateType.FullName ?? delegateType.Name;
    }

    static string GetSimplifiedFullTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            var genericTypeName = type.Name.Substring(0, type.Name.IndexOf('`'));
            var args = string.Join(", ", genericArgs.Select(t => GetSimplifiedFullTypeName(t)));

            return $"{type.Namespace}.{genericTypeName}<{args}>";
        }

        return type.FullName ?? type.Name;
    }

    static bool IsTerminalGuiType(Type type)
    {
        return type.Namespace?.StartsWith("Terminal.Gui") == true;
    }

    static string GetFullTypeName(Type type)
    {
        // Simplify generic type names for readability in generated code
        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            var genericTypeName = type.Name.Substring(0, type.Name.IndexOf('`'));
            var args = string.Join(", ", genericArgs.Select(t => GetSimplifiedTypeName(t)));

            var ns = type.Namespace;
            if (string.IsNullOrEmpty(ns))
                return $"{genericTypeName}<{args}>";

            return $"{ns}.{genericTypeName}<{args}>";
        }

        return type.FullName ?? type.Name;
    }

    static string GetSimplifiedTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            var genericTypeName = type.Name.Substring(0, type.Name.IndexOf('`'));
            var args = string.Join(", ", genericArgs.Select(t => GetSimplifiedTypeName(t)));
            return $"{genericTypeName}<{args}>";
        }

        // Use simple name for common types
        if (type.Namespace == "System")
        {
            return type.Name switch
            {
                "Int32" => "int",
                "String" => "string",
                "Boolean" => "bool",
                "Single" => "float",
                "Double" => "double",
                "Object" => "object",
                _ => type.Name
            };
        }

        return type.Name;
    }

    static string GenerateMappingCode(List<ViewMetadata> metadata)
    {
        var sb = new StringBuilder();

        // Manual overrides for properties that are not detected correctly by reflection
        var manualPropertyOverrides = new Dictionary<string, Dictionary<string, (string propName, string targetType, string description)>>
        {
            ["CheckBox"] = new Dictionary<string, (string, string, string)>
            {
                ["Value"] = ("Value", "Terminal.Gui.Views.CheckState", "Checked state")
            }
        };

        // File header
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("// Generated by Terminal.Gui.MetadataExtractor");
        sb.AppendLine($"// Generated at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine("namespace Terminal.Gui.XamlLike;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Provides mappings between XAML elements/attributes and Terminal.Gui v2 API");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public static class Mappings");
        sb.AppendLine("{");

        // ControlMappings
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Maps XAML element names to Terminal.Gui control types");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static readonly Dictionary<string, ControlMapping> ControlMappings = new()");
        sb.AppendLine("    {");

        foreach (var view in metadata)
        {
            var isContainer = view.IsContainer ? "true" : "false";
            sb.AppendLine($"        [\"{view.TypeName}\"] = new ControlMapping(\"{view.FullTypeName}\", \"{view.Namespace}\", isContainer: {isContainer}),");
        }

        sb.AppendLine("    };");
        sb.AppendLine();

        // EventMappings
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Maps XAML event names to Terminal.Gui event names by control type");
        sb.AppendLine("    /// NOTE: These mappings may need adjustment based on actual Terminal.Gui v2 API");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static readonly Dictionary<string, Dictionary<string, EventMapping>> EventMappings = new()");
        sb.AppendLine("    {");

        foreach (var view in metadata.Where(v => v.Events.Any()))
        {
            sb.AppendLine($"        [\"{view.TypeName}\"] = new Dictionary<string, EventMapping>");
            sb.AppendLine("        {");
            foreach (var evt in view.Events)
            {
                var obsoleteParam = evt.IsObsolete ? ", isObsolete: true" : "";
                sb.AppendLine($"            [\"{evt.Name}\"] = new EventMapping(\"{evt.Name}\", \"{evt.DelegateType}\", \"{evt.Name} event\"{obsoleteParam}),");
            }
            sb.AppendLine("        },");
        }

        sb.AppendLine("    };");
        sb.AppendLine();

        // TwoWayBindings (detected from properties with change events)
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Maps properties that support TwoWay binding");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static readonly Dictionary<string, Dictionary<string, TwoWayBinding>> TwoWayBindings = new()");
        sb.AppendLine("    {");

        var twoWayBindings = DetectTwoWayBindings(metadata);
        foreach (var (viewName, bindings) in twoWayBindings.OrderBy(kvp => kvp.Key))
        {
            sb.AppendLine($"        [\"{viewName}\"] = new Dictionary<string, TwoWayBinding>");
            sb.AppendLine("        {");
            foreach (var (propName, eventName) in bindings.OrderBy(kvp => kvp.Key))
            {
                sb.AppendLine($"            [\"{propName}\"] = new TwoWayBinding(\"{propName}\", \"{eventName}\", \"{propName} property with change notification\"),");
            }
            sb.AppendLine("        },");
        }

        sb.AppendLine("    };");
        sb.AppendLine();

        // PropertyMappings
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Maps properties by control type, with common properties shared across controls");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static readonly Dictionary<string, Dictionary<string, PropertyMapping>> PropertyMappings = new()");
        sb.AppendLine("    {");

        // Common properties (properties that appear in multiple views)
        var allPropertiesGrouped = metadata
            .SelectMany(v => v.Properties.Select(p => new { View = v, Property = p }))
            .GroupBy(x => x.Property.Name)
            .ToList();

        // Properties that have different types across views should NOT be common
        var polymorphicProperties = allPropertiesGrouped
            .Where(g => g.Select(x => x.Property.FullPropertyType).Distinct().Count() > 1)
            .Select(g => g.Key)
            .ToHashSet();

        var commonPropertyNames = allPropertiesGrouped
            .Where(g => g.Count() >= 5 && !polymorphicProperties.Contains(g.Key)) // Properties in 5+ views with same type
            .Select(g => g.Key)
            .ToHashSet();

        if (commonPropertyNames.Count > 0)
        {
            sb.AppendLine("        [\"Common\"] = new Dictionary<string, PropertyMapping>");
            sb.AppendLine("        {");

            var commonProps = allPropertiesGrouped
                .Where(g => commonPropertyNames.Contains(g.Key))
                .Select(g => g.First().Property)
                .DistinctBy(p => p.Name)
                .OrderBy(p => p.Name);

            foreach (var prop in commonProps)
            {
                sb.AppendLine($"            [\"{prop.Name}\"] = new PropertyMapping(\"{prop.Name}\", \"{prop.FullPropertyType}\", \"{prop.Name} property\"),");
            }
            sb.AppendLine("        },");
        }

        // Control-specific properties (including polymorphic properties)
        foreach (var view in metadata)
        {
            var specificProps = view.Properties
                .Where(p => !commonPropertyNames.Contains(p.Name) || polymorphicProperties.Contains(p.Name))
                .ToList();

            // Add manual overrides if they exist for this view
            if (manualPropertyOverrides.TryGetValue(view.TypeName, out var viewManualOverrides))
            {
                Console.WriteLine($"  → Applying {viewManualOverrides.Count} manual overrides for {view.TypeName}");
                foreach (var (propName, propData) in viewManualOverrides)
                {
                    // Remove any auto-detected version and add the manual override
                    specificProps.RemoveAll(p => p.Name == propName);
                    // We'll add the override directly in output below
                }
            }

            if (specificProps.Count > 0 || manualPropertyOverrides.ContainsKey(view.TypeName))
            {
                if (view.TypeName == "CheckBox")
                {
                    Console.WriteLine($"  → CheckBox: specificProps={specificProps.Count}, hasOverrides={manualPropertyOverrides.ContainsKey(view.TypeName)}");
                }

                sb.AppendLine($"        [\"{view.TypeName}\"] = new Dictionary<string, PropertyMapping>");
                sb.AppendLine("        {");

                // Add manual overrides first
                if (manualPropertyOverrides.TryGetValue(view.TypeName, out var viewOverrides))
                {
                    foreach (var (propName, propData) in viewOverrides.OrderBy(kvp => kvp.Key))
                    {
                        if (view.TypeName == "CheckBox")
                        {
                            Console.WriteLine($"  → Adding override: {propName} = {propData.targetType}");
                        }
                        sb.AppendLine($"            [\"{propName}\"] = new PropertyMapping(\"{propData.propName}\", \"{propData.targetType}\", \"{propData.description}\"),");
                    }
                }

                // Add auto-detected properties
                foreach (var prop in specificProps.OrderBy(p => p.Name))
                {
                    if (view.TypeName == "CheckBox")
                    {
                        Console.WriteLine($"  → Adding detected: {prop.Name} = {prop.FullPropertyType}");
                    }
                    sb.AppendLine($"            [\"{prop.Name}\"] = new PropertyMapping(\"{prop.Name}\", \"{prop.FullPropertyType}\", \"{prop.Name} property\"),");
                }
                sb.AppendLine("        },");
            }
        }

        sb.AppendLine("    };");
        sb.AppendLine();

        // Helper methods
        GenerateHelperMethods(sb);

        sb.AppendLine("}");
        sb.AppendLine();

        // Helper classes
        GenerateHelperClasses(sb);

        return sb.ToString();
    }

    static Dictionary<string, Dictionary<string, string>> DetectTwoWayBindings(List<ViewMetadata> metadata)
    {
        var result = new Dictionary<string, Dictionary<string, string>>();

        foreach (var view in metadata)
        {
            var bindings = new Dictionary<string, string>();

            // Look for properties that have a corresponding change event
            foreach (var prop in view.Properties)
            {
                // Common patterns for change events
                var possibleEventNames = new[]
                {
                    $"{prop.Name}Changed",
                    "TextChanged",
                    "ValueChanged",
                    "SelectedItemChanged",
                    "SourceChanged"
                };

                foreach (var eventName in possibleEventNames)
                {
                    if (view.Events.Any(e => e.Name == eventName))
                    {
                        bindings[prop.Name] = eventName;
                        break;
                    }
                }
            }

            if (bindings.Count > 0)
            {
                result[view.TypeName] = bindings;
            }
        }

        return result;
    }

    static void GenerateHelperMethods(StringBuilder sb)
    {
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Gets the full type name for a control, with optional generic type parameter");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static string? GetFullTypeName(string elementName, string? genericType = null)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (!ControlMappings.TryGetValue(elementName, out ControlMapping? mapping))");
        sb.AppendLine("            return null; // Type not found - caller should generate diagnostic");
        sb.AppendLine();
        sb.AppendLine("        var typeName = mapping.FullTypeName;");
        sb.AppendLine();
        sb.AppendLine("        // If this control supports generics and a generic type is specified, add it");
        sb.AppendLine("        if (!string.IsNullOrEmpty(genericType) && elementName == \"OptionSelector\")");
        sb.AppendLine("        {");
        sb.AppendLine("            typeName = $\"{typeName}<{genericType}>\";");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        return typeName;");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Checks if a control is a container (can have children)");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static bool IsContainer(string elementName) =>");
        sb.AppendLine("        ControlMappings.TryGetValue(elementName, out ControlMapping? mapping) && mapping.IsContainer;");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Gets the event mapping for a control/event combination");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static EventMapping? GetEventMapping(string controlName, string eventName) =>");
        sb.AppendLine("        EventMappings.TryGetValue(controlName, out Dictionary<string, EventMapping>? events) &&");
        sb.AppendLine("        events.TryGetValue(eventName, out EventMapping? eventMapping)");
        sb.AppendLine("            ? eventMapping");
        sb.AppendLine("            : null;");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Gets TwoWay binding information for a control/property combination");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static TwoWayBinding? GetTwoWayBinding(string controlName, string propertyName) =>");
        sb.AppendLine("        TwoWayBindings.TryGetValue(controlName, out Dictionary<string, TwoWayBinding>? properties) &&");
        sb.AppendLine("        properties.TryGetValue(propertyName, out TwoWayBinding? binding)");
        sb.AppendLine("            ? binding");
        sb.AppendLine("            : null;");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Checks if a property supports TwoWay binding");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static bool SupportsTwoWayBinding(string controlName, string propertyName) =>");
        sb.AppendLine("        GetTwoWayBinding(controlName, propertyName) != null;");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Gets property mapping information by property name and optionally control name");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static PropertyMapping? GetPropertyMapping(string propertyName, string? controlName = null)");
        sb.AppendLine("    {");
        sb.AppendLine("        // Check control-specific properties first if control name is provided");
        sb.AppendLine("        if (!string.IsNullOrEmpty(controlName) &&");
        sb.AppendLine("            PropertyMappings.TryGetValue(controlName, out Dictionary<string, PropertyMapping>? controlProperties) &&");
        sb.AppendLine("            controlProperties.TryGetValue(propertyName, out PropertyMapping? controlMapping))");
        sb.AppendLine("        {");
        sb.AppendLine("            return controlMapping;");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        // Check Common properties");
        sb.AppendLine("        if (PropertyMappings.TryGetValue(\"Common\", out Dictionary<string, PropertyMapping>? commonProperties) &&");
        sb.AppendLine("            commonProperties.TryGetValue(propertyName, out PropertyMapping? commonMapping))");
        sb.AppendLine("        {");
        sb.AppendLine("            return commonMapping;");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        // Search in all control-specific properties if not found yet");
        sb.AppendLine("        foreach (Dictionary<string, PropertyMapping> properties in PropertyMappings.Values)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (properties.TryGetValue(propertyName, out PropertyMapping? mapping))");
        sb.AppendLine("            {");
        sb.AppendLine("                return mapping;");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        return null;");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Checks if an attribute name is a known event for any control type");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static bool IsKnownEvent(string eventName)");
        sb.AppendLine("    {");
        sb.AppendLine("        foreach (Dictionary<string, EventMapping> controlEvents in EventMappings.Values)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (controlEvents.ContainsKey(eventName))");
        sb.AppendLine("            {");
        sb.AppendLine("                return true;");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("        return false;");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Determines if a property should be treated as a boolean value");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static bool IsBooleanProperty(string propertyName)");
        sb.AppendLine("    {");
        sb.AppendLine("        PropertyMapping? mapping = GetPropertyMapping(propertyName);");
        sb.AppendLine("        return mapping?.TargetType == \"System.Boolean\";");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Determines if a property should be treated as an integer value");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static bool IsIntProperty(string propertyName)");
        sb.AppendLine("    {");
        sb.AppendLine("        PropertyMapping? mapping = GetPropertyMapping(propertyName);");
        sb.AppendLine("        return mapping?.TargetType == \"System.Int32\";");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Determines if a property should be treated as a float value");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static bool IsFloatProperty(string propertyName)");
        sb.AppendLine("    {");
        sb.AppendLine("        PropertyMapping? mapping = GetPropertyMapping(propertyName);");
        sb.AppendLine("        return mapping?.TargetType == \"System.Single\" || mapping?.TargetType == \"System.Double\";");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Determines if a property expects an array type");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static bool IsArrayProperty(string propertyName)");
        sb.AppendLine("    {");
        sb.AppendLine("        PropertyMapping? mapping = GetPropertyMapping(propertyName);");
        sb.AppendLine("        return mapping?.TargetType.EndsWith(\"[]\") ?? false;");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Determines if a property is a Terminal.Gui type that needs full namespace qualification");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static bool IsTerminalGuiType(string propertyName)");
        sb.AppendLine("    {");
        sb.AppendLine("        PropertyMapping? mapping = GetPropertyMapping(propertyName);");
        sb.AppendLine("        if (mapping == null) return false;");
        sb.AppendLine();
        sb.AppendLine("        return mapping.TargetType.StartsWith(\"Terminal.Gui.\");");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Gets the fully qualified type name for a property");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static string? GetFullyQualifiedType(string propertyName)");
        sb.AppendLine("    {");
        sb.AppendLine("        PropertyMapping? mapping = GetPropertyMapping(propertyName);");
        sb.AppendLine("        return mapping?.TargetType;");
        sb.AppendLine("    }");
    }

    static void GenerateHelperClasses(StringBuilder sb)
    {
        // ControlMapping class
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Information about a Terminal.Gui control type");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public class ControlMapping");
        sb.AppendLine("{");
        sb.AppendLine("    public string FullTypeName { get; }");
        sb.AppendLine("    public string Namespace { get; }");
        sb.AppendLine("    public bool IsContainer { get; }");
        sb.AppendLine("    public string? Description { get; }");
        sb.AppendLine();
        sb.AppendLine("    public ControlMapping(string fullTypeName, string namespaceName, bool isContainer = false, string? description = null)");
        sb.AppendLine("    {");
        sb.AppendLine("        FullTypeName = fullTypeName;");
        sb.AppendLine("        Namespace = namespaceName;");
        sb.AppendLine("        IsContainer = isContainer;");
        sb.AppendLine("        Description = description;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();

        // EventMapping class
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Information about an event mapping");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public class EventMapping");
        sb.AppendLine("{");
        sb.AppendLine("    public string EventName { get; }");
        sb.AppendLine("    public string DelegateType { get; }");
        sb.AppendLine("    public string Description { get; }");
        sb.AppendLine("    public bool IsObsolete { get; }");
        sb.AppendLine("    public string? ReplacementEvent { get; }");
        sb.AppendLine();
        sb.AppendLine("    public EventMapping(string eventName, string delegateType, string description, bool isObsolete = false, string? replacementEvent = null)");
        sb.AppendLine("    {");
        sb.AppendLine("        EventName = eventName;");
        sb.AppendLine("        DelegateType = delegateType;");
        sb.AppendLine("        Description = description;");
        sb.AppendLine("        IsObsolete = isObsolete;");
        sb.AppendLine("        ReplacementEvent = replacementEvent;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Gets the obsolete message if this event is obsolete");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public string? GetObsoleteMessage() => IsObsolete && ReplacementEvent != null");
        sb.AppendLine("        ? $\"Use '{ReplacementEvent}' instead\"");
        sb.AppendLine("        : null;");
        sb.AppendLine("}");
        sb.AppendLine();

        // TwoWayBinding class
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Information about a TwoWay bindable property");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public class TwoWayBinding");
        sb.AppendLine("{");
        sb.AppendLine("    public string PropertyName { get; }");
        sb.AppendLine("    public string ChangeEventName { get; }");
        sb.AppendLine("    public string Description { get; }");
        sb.AppendLine();
        sb.AppendLine("    public TwoWayBinding(string propertyName, string changeEventName, string description)");
        sb.AppendLine("    {");
        sb.AppendLine("        PropertyName = propertyName;");
        sb.AppendLine("        ChangeEventName = changeEventName;");
        sb.AppendLine("        Description = description;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();

        // PropertyMapping class
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Information about a property mapping");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public class PropertyMapping");
        sb.AppendLine("{");
        sb.AppendLine("    public string PropertyName { get; }");
        sb.AppendLine("    public string TargetType { get; }");
        sb.AppendLine("    public string Description { get; }");
        sb.AppendLine();
        sb.AppendLine("    public PropertyMapping(string propertyName, string targetType, string description)");
        sb.AppendLine("    {");
        sb.AppendLine("        PropertyName = propertyName;");
        sb.AppendLine("        TargetType = targetType;");
        sb.AppendLine("        Description = description;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
    }
}

class ViewMetadata
{
    public string TypeName { get; set; } = "";
    public string FullTypeName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public bool IsContainer { get; set; }
    public List<PropertyMetadata> Properties { get; set; } = new();
    public List<EventMetadata> Events { get; set; } = new();
}

class PropertyMetadata
{
    public string Name { get; set; } = "";
    public string PropertyType { get; set; } = "";
    public string FullPropertyType { get; set; } = "";
    public bool IsTerminalGuiType { get; set; }
    public bool IsBoolean { get; set; }
    public bool IsInt { get; set; }
    public bool IsFloat { get; set; }
    public bool IsString { get; set; }
    public string DeclaringType { get; set; } = "";
}

class EventMetadata
{
    public string Name { get; set; } = "";
    public string DelegateType { get; set; } = "";
    public bool IsObsolete { get; set; }
}
