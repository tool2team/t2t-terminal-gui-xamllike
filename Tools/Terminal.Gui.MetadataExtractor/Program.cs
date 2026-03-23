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
        var nullabilityContext = new NullabilityInfoContext();

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
                FullPropertyType = GetSimplifiedTypeName(prop.PropertyType),
                IsTerminalGuiType = IsTerminalGuiType(prop.PropertyType),
                IsNullable = IsNullableProperty(prop, nullabilityContext),
                DeclaringType = prop.DeclaringType?.Name ?? type.Name
            };

            properties.Add(propMeta);
        }

        return properties.OrderBy(p => p.Name).ToList();
    }

    static bool IsNullableProperty(PropertyInfo prop, NullabilityInfoContext context)
    {
        var type = prop.PropertyType;

        // Value types: nullable only if Nullable<T>
        if (type.IsValueType)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        // Reference types: use NullabilityInfoContext to detect nullable annotations
        // This properly handles string vs string? in nullable context
        var nullabilityInfo = context.Create(prop);
        return nullabilityInfo.WriteState == NullabilityState.Nullable;
    }

    static List<EventMetadata> ExtractEvents(Type type)
    {
        var events = new List<EventMetadata>();

        foreach (var evt in type.GetEvents(BindingFlags.Public | BindingFlags.Instance))
        {
            var eventHandlerType = evt.EventHandlerType;
            if (eventHandlerType == null)
                continue;

            var eventMeta = new EventMetadata
            {
                Name = evt.Name,
                DelegateType = SimplifyDelegateType(eventHandlerType)
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
            var simplifiedArg = GetSimplifiedTypeName(eventArgsType);
            return $"EventHandler<{simplifiedArg}>";
        }

        // For non-generic EventHandler
        if (delegateType == typeof(EventHandler))
        {
            return "EventHandler";
        }

        // For other delegate types (like NotifyCollectionChangedEventHandler)
        return delegateType.FullName ?? delegateType.Name;
    }

    static bool IsTerminalGuiType(Type type)
    {
        return type.Namespace?.StartsWith("Terminal.Gui") == true;
    }

    static string GetSimplifiedTypeName(Type type)
    {
        // Simplify generic type names for readability in generated code
        if(Nullable.GetUnderlyingType(type) is Type utype)
        {
            var ns = utype.Namespace;
            var utypename = GetSimplifiedTypeName(utype);

            return $"{utypename}?";
        }

        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            var genericTypeName = type.Name.Substring(0, type.Name.IndexOf('`'));
            var args = string.Join(", ", genericArgs.Select(GetSimplifiedTypeName));
            var ns = type.Namespace;

            return string.IsNullOrEmpty(ns)
                ? $"{genericTypeName}<{args}>"
                : $"{ns}.{genericTypeName}<{args}>";
        }

        if(type.IsArray)
        {
            var elementType = type.GetElementType();
            var arg = GetSimplifiedTypeName(elementType!);

            return $"{arg}[]";
        }

        // Use simple name for common types
        if (type.Namespace == "System")
        {
            return type.Name switch
            {
                "Int32" => "int",
                "UInt32" => "uint",
                "Int64" => "long",
                "UInt64" => "ulong",
                "String" => "string",
                "Boolean" => "bool",
                "Single" => "float",
                "Double" => "double",
                "Object" => "object",
                _ => type.Name
            };
        }

        return type.FullName ?? type.Name;
    }

    static string GenerateMappingCode(List<ViewMetadata> metadata)
    {
        var sb = new StringBuilder();

        // Manual overrides for properties that are not detected correctly by reflection
        var manualPropertyOverrides = new Dictionary<string, Dictionary<string, (string propName, string targetType, string description, bool isNullable)>>
        {
            ["CheckBox"] = new Dictionary<string, (string, string, string, bool)>
            {
                ["Value"] = ("Value", "Terminal.Gui.Views.CheckState", "Checked state", false) // CheckState is an enum (value type)
            },
            ["Button"] = new Dictionary<string, (string, string, string, bool)>
            {
                ["IsDialogButton"] = ("IsDialogButton", "bool", "Indicates if button should be added via AddButton (Dialog)", false) // Generator-only property
            }
        };

        // File header
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("// Generated by Terminal.Gui.MetadataExtractor");
        sb.AppendLine($"// Generated at: {DateTime.Now:yyyy-MM-dd}");
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
                sb.AppendLine($"            [\"{evt.Name}\"] = new EventMapping(\"{evt.Name}\", \"{evt.DelegateType}\"),");
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
                sb.AppendLine($"            [\"{propName}\"] = new TwoWayBinding(\"{propName}\", \"{eventName}\"),");
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
                sb.AppendLine($"            [\"{prop.Name}\"] = new PropertyMapping(\"{prop.Name}\", \"{prop.FullPropertyType}\"),");
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
                foreach (var (propName, propData) in viewManualOverrides)
                {
                    // Remove any auto-detected version and add the manual override
                    specificProps.RemoveAll(p => p.Name == propName);
                    // We'll add the override directly in output below
                }
            }

            if (specificProps.Count > 0 || manualPropertyOverrides.ContainsKey(view.TypeName))
            {
                sb.AppendLine($"        [\"{view.TypeName}\"] = new Dictionary<string, PropertyMapping>");
                sb.AppendLine("        {");

                // Add manual overrides first
                if (manualPropertyOverrides.TryGetValue(view.TypeName, out var viewOverrides))
                {
                    foreach (var (propName, propData) in viewOverrides.OrderBy(kvp => kvp.Key))
                    {
                        string isNullableParam = propData.isNullable ? ", true" : "";
                        sb.AppendLine($"            [\"{propName}\"] = new PropertyMapping(\"{propData.propName}\", \"{propData.targetType}\"{isNullableParam}),");
                    }
                }

                // Add auto-detected properties
                foreach (var prop in specificProps.OrderBy(p => p.Name))
                {
                    string isNullableParam = prop.IsNullable ? ", true" : "";
                    sb.AppendLine($"            [\"{prop.Name}\"] = new PropertyMapping(\"{prop.Name}\", \"{prop.FullPropertyType}\"{isNullableParam}),");
                }
                sb.AppendLine("        },");
            }
        }

        sb.AppendLine("    };");
        sb.AppendLine();


        sb.AppendLine("}");
        sb.AppendLine();

        return sb.ToString();
    }

    static Dictionary<string, Dictionary<string, string>> DetectTwoWayBindings(List<ViewMetadata> metadata)
    {
        var result = new Dictionary<string, Dictionary<string, string>>();

        foreach (var view in metadata)
        {
            var bindings = new Dictionary<string, string>();

            // Only process controls that accept user input
            if (!IsUserInputControl(view.TypeName))
                continue;

            // Look for properties that are both user-modifiable and have a corresponding change event
            foreach (var prop in view.Properties)
            {
                // Skip read-only or configuration properties
                if (!IsUserModifiableProperty(view.TypeName, prop.Name))
                    continue;

                // Common patterns for change events
                var possibleEventNames = new[]
                {
                    $"{prop.Name}Changed",
                    "TextChanged",
                    "ValueChanged",
                    "SelectedItemChanged",
                    "SourceChanged",
                    "CheckedChanged"
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

    /// <summary>
    /// Determines if a control accepts user input and can have TwoWay bindings
    /// </summary>
    static bool IsUserInputControl(string typeName)
    {
        var inputControls = new HashSet<string>
        {
            // Text input
            "TextField", "TextView", "TextValidateField", "AutocompleteTextField",
            "DateField", "TimeField",

            // Selection
            "CheckBox", "RadioGroup", "ComboBox", "ListView", "TreeView",
            "OptionSelector", "FlagSelector", "LinearRange",

            // Numeric input
            "NumericUpDown", "Slider", "ScrollBar",

            // Color/Attribute selection
            "ColorPicker", "ColorPicker16", "AttributePicker",

            // Other interactive
            "CharMap", "HexView", "TableView"
        };

        return inputControls.Contains(typeName);
    }

    /// <summary>
    /// Determines if a property is user-modifiable (not just configuration)
    /// </summary>
    static bool IsUserModifiableProperty(string controlType, string propertyName)
    {
        // Common user-modifiable properties across controls
        var alwaysModifiable = new HashSet<string>
        {
            "Text", "Value", "SelectedItem", "SelectedIndex", 
            "Checked", "Selected", "CurrentValue"
        };

        if (alwaysModifiable.Contains(propertyName))
            return true;

        // Control-specific user-modifiable properties
        var controlSpecific = new Dictionary<string, HashSet<string>>
        {
            ["CheckBox"] = new() { "CheckedState", "Checked" },
            ["RadioGroup"] = new() { "SelectedItem", "Selected" },
            ["ListView"] = new() { "SelectedItem", "SelectedItems", "Source" },
            ["TreeView"] = new() { "SelectedObject", "Objects" },
            ["ComboBox"] = new() { "SelectedItem", "Source" },
            ["ColorPicker"] = new() { "SelectedColor" },
            ["ColorPicker16"] = new() { "SelectedColor" },
            ["TableView"] = new() { "SelectedRow", "SelectedColumn" },
            ["HexView"] = new() { "Position", "Address" },
            ["CharMap"] = new() { "SelectedCodePoint" },
            ["ScrollBar"] = new() { "Position" },
            ["Slider"] = new() { "Value", "CurrentValue" },
            ["NumericUpDown"] = new() { "Value" },
            ["OptionSelector"] = new() { "Selected", "SelectedOption" },
            ["FlagSelector"] = new() { "Flags" },
            ["LinearRange"] = new() { "SelectedOption" }
        };

        if (controlSpecific.TryGetValue(controlType, out var specificProps))
        {
            return specificProps.Contains(propertyName);
        }

        // Configuration/Layout properties - NOT user-modifiable
        var nonModifiable = new HashSet<string>
        {
            "Width", "Height", "X", "Y", "Title", "BorderStyle",
            "Visible", "Enabled", "CanFocus", "TabStop", "TabIndex",
            "ColorScheme", "Cursor", "Frame", "Id", "IsDefault",
            "SuperView", "Subviews", "Parent", "Bounds",
            // Many more configuration properties...
            "AutoSize", "TextAlignment", "VerticalTextAlignment",
            "HotKey", "HotKeySpecifier", "IsInitialized", "WantContinuousButtonPressed"
        };

        return !nonModifiable.Contains(propertyName);
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
    public bool IsNullable { get; set; }
    public string DeclaringType { get; set; } = "";
}

class EventMetadata
{
    public string Name { get; set; } = "";
    public string DelegateType { get; set; } = "";
}
