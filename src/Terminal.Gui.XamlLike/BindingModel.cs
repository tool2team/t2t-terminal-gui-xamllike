using System.Collections.Generic;
using System;

namespace Terminal.Gui.XamlLike
{
    /// <summary>
    /// Represents a data binding expression from XAML
    /// </summary>
    public class BindingExpression
    {
        public string PropertyPath { get; }
        public BindingMode Mode { get; }
        public string SourceExpression { get; }

        public BindingExpression(string propertyPath, BindingMode mode, string sourceExpression)
        {
            PropertyPath = propertyPath;
            Mode = mode;
            SourceExpression = sourceExpression;
        }
    /// <summary>
    /// Parses a binding expression from XAML syntax like "{Bind Status}" or "{Bind UserName, Mode=TwoWay}"
    /// </summary>
    /// <param name="expression">The binding expression string</param>
    /// <param name="dataType">Optional DataType from x:DataType attribute (e.g., "ViewModel")</param>
    public static BindingExpression? Parse(string expression, string? dataType = null)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return null;

        var trimmed = expression.Trim();
        if (!trimmed.StartsWith("{Bind ") || !trimmed.EndsWith("}"))
            return null;

        // Remove "{Bind " and "}"
        var content = trimmed.Substring(6, trimmed.Length - 7).Trim();

        // Split by comma to handle Mode parameter
        var parts = content.Split(',');
        var propertyPath = parts[0].Trim();
        var mode = BindingMode.OneWay; // default

        // Check for Mode parameter
        for (int i = 1; i < parts.Length; i++)
        {
            var part = parts[i].Trim();
            if (part.StartsWith("Mode=") || part.StartsWith("mode="))
            {
                var modeValue = part.Substring(5).Trim();
                if (System.Enum.TryParse<BindingMode>(modeValue, true, out var parsedMode))
                {
                    mode = parsedMode;
                }
            }
        }

        // Generate the source expression
        // If dataType is specified and propertyPath doesn't contain a dot, prepend dataType
        // If propertyPath already contains a dot (e.g., "ViewModel.Property"), use it as-is
        // Otherwise (no dataType, no dot), it's a self-binding property
        string sourceExpression;
        if (!string.IsNullOrEmpty(dataType) && !propertyPath.Contains("."))
        {
            sourceExpression = $"{dataType}.{propertyPath}";
        }
        else
        {
            sourceExpression = propertyPath;
        }

        return new BindingExpression(propertyPath, mode, sourceExpression);
    }

    /// <summary>
    /// Gets the property name from a property path (e.g., "Status" from "Status" or "Header.Title" from "Header.Title")
    /// </summary>
    public string GetRootPropertyName()
    {
        var parts = PropertyPath.Split('.');
        return parts[0];
    }

    /// <summary>
    /// Checks if this is a nested property path (contains dots)
    /// </summary>
    public bool IsNestedProperty => PropertyPath.Contains(".");
}

/// <summary>
/// Binding mode for data binding
/// </summary>
public enum BindingMode
{
    /// <summary>
    /// Data flows from source (ViewModel) to target (UI) only
    /// </summary>
    OneWay,
    
    /// <summary>
    /// Data flows in both directions between source and target
    /// </summary>
    TwoWay
}

    /// <summary>
    /// Information about a control that has bindings
    /// </summary>
    public class BoundControl
    {
        private string? _generatedFieldName;

        public string ElementName { get; }
        public string? FieldName { get; }
        public string? GenericType { get; }
        public List<BoundProperty> BoundProperties { get; }

        public BoundControl(string elementName, string? fieldName, List<BoundProperty> boundProperties, string? genericType = null)
        {
            ElementName = elementName;
            FieldName = fieldName;
            GenericType = genericType;
            BoundProperties = boundProperties;
        }

        /// <summary>
        /// Gets the C# field name for this control.
        /// If no x:Name is specified, generates a unique field name automatically.
        /// </summary>
        public string GetFieldName()
        {
            if (!string.IsNullOrEmpty(FieldName))
                return FieldName!;

            // Return the pre-generated field name
            return _generatedFieldName ?? throw new InvalidOperationException("Field name not set for bound control");
        }

        /// <summary>
        /// Sets the generated field name for this control
        /// </summary>
        public void SetGeneratedFieldName(string fieldName)
        {
            _generatedFieldName = fieldName;
        }

        /// <summary>
        /// Checks if this control has an explicit x:Name
        /// </summary>
        public bool HasExplicitName => !string.IsNullOrEmpty(FieldName);
    }

    /// <summary>
    /// Information about a bound property on a control
    /// </summary>
    public class BoundProperty
    {
        public string PropertyName { get; }
        public BindingExpression Binding { get; }

        public BoundProperty(string propertyName, BindingExpression binding)
        {
            PropertyName = propertyName;
            Binding = binding;
        }

        /// <summary>
        /// Gets the event name for TwoWay binding (if applicable)
        /// </summary>
        public string? GetChangeEventName() => PropertyName switch
        {
            "Text" => "TextChanged",
            "Checked" => "Toggled", 
            "SelectedIndex" => "SelectedIndexChanged",
            _ => null
        };

        /// <summary>
        /// Checks if this property supports TwoWay binding
        /// </summary>
        public bool SupportsTwoWay => GetChangeEventName() != null;
    }
}