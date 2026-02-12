using System.Collections.Generic;

namespace Terminal.Gui.XamlLike
{
    /// <summary>
    /// Represents a parsed .tui.xaml document
    /// </summary>
    public class XamlDocument
    {
        public XamlElement RootElement { get; }
        public string SourceFilePath { get; }

        public XamlDocument(XamlElement rootElement, string sourceFilePath)
        {
            RootElement = rootElement;
            SourceFilePath = sourceFilePath;
        }

        /// <summary>
        /// Gets the x:Class value from the root element
        /// </summary>
        public string? ClassName => RootElement.GetAttributeValue("x:Class");

        /// <summary>
        /// Gets the x:DataType value from the root element (binding context)
        /// </summary>
        public string? DataType => RootElement.GetAttributeValue("x:DataType");
    }

    /// <summary>
    /// Represents an XML element in the .tui.xaml file
    /// </summary>
    public class XamlElement
    {
        public string Name { get; }
        public Dictionary<string, string> Attributes { get; }
        public List<XamlElement> Children { get; }

        /// <summary>
        /// Line number in the source XAML file (1-based, 0 if not available)
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Column number in the source XAML file (1-based, 0 if not available)
        /// </summary>
        public int LinePosition { get; }

        public XamlElement(string name, Dictionary<string, string> attributes, List<XamlElement> children, int lineNumber = 0, int linePosition = 0)
        {
            Name = name;
            Attributes = attributes;
            Children = children;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }
    /// <summary>
    /// Gets the x:Name value for this element
    /// </summary>
    public string? XName => GetAttributeValue("x:Name");

    /// <summary>
    /// Gets the x:Type value for this element (used for generic types)
    /// </summary>
    public string? XType => GetAttributeValue("x:Type");

    /// <summary>
    /// Gets an attribute value by name
    /// </summary>
    public string? GetAttributeValue(string name) => 
        Attributes.TryGetValue(name, out var value) ? value : null;

    /// <summary>
    /// Checks if this element has an attribute
    /// </summary>
    public bool HasAttribute(string name) => Attributes.ContainsKey(name);

    /// <summary>
    /// Gets all non-xmlns and non-x: attributes (property attributes)
    /// </summary>
    public Dictionary<string, string> PropertyAttributes
    {
        get
        {
            var result = new Dictionary<string, string>();
            foreach (var kvp in Attributes)
            {
                var key = kvp.Key;
                var value = kvp.Value;
                if (!key.StartsWith("x:") && !key.StartsWith("xmlns") && !IsEventAttribute(key))
                {
                    result[key] = value;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Gets all event attributes (those that don't contain bindings or expressions)
    /// </summary>
    public Dictionary<string, string> EventAttributes
    {
        get
        {
            var result = new Dictionary<string, string>();
            foreach (var kvp in Attributes)
            {
                var key = kvp.Key;
                var value = kvp.Value;
                if (!key.StartsWith("x:") && !key.StartsWith("xmlns") && IsEventAttribute(key) && !IsBinding(value) && !IsExpression(value))
                {
                    result[key] = value;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Checks if a value is a binding expression {Bind ...}
    /// </summary>
    private static bool IsBinding(string value) => 
        value.Trim().StartsWith("{Bind ") && value.Trim().EndsWith("}");

    /// <summary>
    /// Checks if a value is an expression like Dim.Fill()
    /// </summary>
    private static bool IsExpression(string value) => 
        value.Contains("(") && value.Contains(")");

    /// <summary>
    /// Checks if an attribute name represents an event (uses Mappings.EventMappings)
    /// </summary>
    private static bool IsEventAttribute(string attributeName) => Mappings.IsKnownEvent(attributeName);
}

    /// <summary>
    /// Represents property information for code generation
    /// </summary>
    public class PropertyInfo
    {
        public string Name { get; }
        public string Value { get; }
        public PropertyType Type { get; }

        public PropertyInfo(string name, string value, PropertyType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Gets the C# code representation of this property value
        /// </summary>
        public string GetCodeValue() => Type switch
        {
            PropertyType.String => $"\"{Value.Replace("\"", "\\\"")}\"",
            PropertyType.Expression => Value, // Emit as-is (Dim.Fill(), etc.)
            PropertyType.Binding => throw new System.InvalidOperationException("Bindings should be handled separately"),
            _ => Value
        };
    }

    /// <summary>
    /// Type of property value
    /// </summary>
    public enum PropertyType
    {
        String,     // Regular string property
        Expression, // C# expression like Dim.Fill()
        Binding     // Data binding {Bind ...}
    }
}