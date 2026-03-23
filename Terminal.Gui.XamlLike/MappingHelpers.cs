// Utility methods for accessing Terminal.Gui mappings
// This file contains helper methods - data is in auto-generated Mappings.cs

#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Terminal.Gui.XamlLike;

/// <summary>
/// Provides utility methods to access Terminal.Gui control mappings
/// The actual data dictionaries are in the auto-generated Mappings.cs file (partial class)
/// </summary>
public static partial class MappingHelpers
{
    //static bool FullStartsWith(this string str, string value) => new Regex($"{value}\\W").IsMatch(str);

    static bool FullStartsWith(this string str, string value)
    {
        if (!str.StartsWith(value))
            return false;
        
        // If exact match or followed by non-letter/digit character
        return str.Length == value.Length || 
               !char.IsLetterOrDigit(str[value.Length]);
    }

    /// <summary>
    /// Gets the full type name for a control, with optional generic type parameter
    /// </summary>
    public static string? GetFullTypeName(string elementName, string? genericType = null)
    {
        if (!Mappings.ControlMappings.TryGetValue(elementName, out ControlMapping? mapping))
            return null; // Type not found - caller should generate diagnostic

        var typeName = mapping.FullTypeName;

        // If this control supports generics and a generic type is specified, add it
        if (!string.IsNullOrEmpty(genericType) && elementName == "OptionSelector")
        {
            typeName = $"{typeName}<{genericType}>";
        }

        return typeName;
    }

    /// <summary>
    /// Gets the control mapping for a control
    /// </summary>
    public static ControlMapping? GetControlMapping(string controlName) =>
        Mappings.ControlMappings.TryGetValue(controlName, out ControlMapping? mapping)
            ? mapping
            : null;

    /// <summary>
    /// Gets the event mapping for a control/event combination
    /// </summary>
    public static EventMapping? GetEventMapping(string controlName, string eventName) =>
        Mappings.EventMappings.TryGetValue(controlName, out Dictionary<string, EventMapping>? events) &&
        events.TryGetValue(eventName, out EventMapping? eventMapping)
            ? eventMapping
            : null;

    /// <summary>
    /// Gets TwoWay binding information for a control/property combination
    /// </summary>
    public static TwoWayBinding? GetTwoWayBinding(string controlName, string propertyName) =>
        Mappings.TwoWayBindings.TryGetValue(controlName, out Dictionary<string, TwoWayBinding>? properties) &&
        properties.TryGetValue(propertyName, out TwoWayBinding? binding)
            ? binding
            : null;

    /// <summary>
    /// Gets property mapping information by property name and optionally control name
    /// </summary>
    public static PropertyMapping? GetPropertyMapping(string controlName, string propertyName)
    {
        // Check control-specific properties first if control name is provided
        if (Mappings.PropertyMappings.TryGetValue(controlName, out Dictionary<string, PropertyMapping>? controlProperties) &&
            controlProperties.TryGetValue(propertyName, out PropertyMapping? controlMapping))
        {
            return controlMapping;
        }

        // Check Common properties
        if (Mappings.PropertyMappings.TryGetValue("Common", out Dictionary<string, PropertyMapping>? commonProperties) &&
            commonProperties.TryGetValue(propertyName, out PropertyMapping? commonMapping))
        {
            return commonMapping;
        }
        return null;
    }

    public static string? GetPropertyValue(string controlName, string propName, string value)
    {
        // Check if property has a mapping with type information
        var propertyMapping = GetPropertyMapping(controlName, propName);

        if (propertyMapping != null)
        {
            var targetType = propertyMapping.TargetType.TrimEnd('?'); // TODO isTerminalGUIType

            // Handle Terminal.Gui types (Pos, Dim, Key, Enum, etc.)
            if (targetType.FullStartsWith("Terminal.Gui"))
            {
                // Special handling for Pos
                if (targetType.FullStartsWith("Terminal.Gui.ViewBase.Pos"))
                {
                    // If value contains parentheses, it's already an expression like Pos.Auto()
                    if (value.Contains("(") && value.Contains(")"))
                    {
                        return value;
                    }

                    var dims = value.Split(',').Select(d => d.Trim());
                    // If not valid, skip it
                    if (dims.Count() > 2 || !dims.All(d => int.TryParse(d, out _)))
                    {
                        return null;
                    }
                    if (dims.Count() > 1)
                    {
                        return $"new Terminal.Gui.ViewBase.Pos({string.Join(", ", dims)})";
                    }

                    // If not numeric, treat as expression
                    return value;
                }

                // Special handling for Dim type
                if (targetType.FullStartsWith("Terminal.Gui.ViewBase.Dim"))
                {
                    // If value contains parentheses, it's already an expression like Dim.Fill()
                    if (value.Contains("(") && value.Contains(")"))
                    {
                        return value;
                    }

                    var dims = value.Split(',').Select(d => d.Trim());
                    // If not valid, skip it
                    if (dims.Count() > 2 || !dims.All(d => int.TryParse(d, out _)))
                    {
                        return null;
                    }
                    if (dims.Count() > 1)
                    {
                        return $"new Terminal.Gui.ViewBase.Dim({string.Join(", ", dims)})";
                    }

                    // If not numeric, treat as expression
                    return value;
                }

                // For other Terminal.Gui types (Key, Enum, etc.), add full namespace
                // Handle values like "Key.F1" or "Key.Q.WithCtrl" or just "F1"
                // Or "CheckState.Checked" → "Terminal.Gui.Views.CheckState.Checked"

                // Skip generic/placeholder values
                if (string.IsNullOrEmpty(value))
                {
                    return null; // Skip this property - can't generate valid code
                }

                var parts = value.Split('.');

                if (parts.Length == 1)
                {
                    // Just the value: "F1" → "Terminal.Gui.Input.Key.F1"
                    return $"{targetType}.{value}";
                }
                else if (parts.Length >= 2)
                {
                    // Has dots: "Key.F1" or "Key.Q.WithCtrl" or "CheckState.Checked"
                    // Check if first part matches the type name
                    var typeShortName = targetType.Split('.').Last();

                    if (parts[0] == typeShortName)
                    {
                        // Remove the type prefix and add the full namespace
                        var valuePart = string.Join(".", parts.Skip(1));
                        return $"{targetType}.{valuePart}";
                    }
                    else
                    {
                        // Doesn't start with type name, prefix the whole thing
                        return $"{targetType}.{value}";
                    }
                }
            }


            // Handle System types based on TargetType

            if (targetType.FullStartsWith("bool"))
            {
                if (bool.TryParse(value, out var boolValue))
                {
                    return boolValue ? "true" : "false";
                }
                // If not a valid boolean, skip it
                return null;
            }

            if (targetType.FullStartsWith("float"))
            {
                if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                {
                    // Ensure float literal has 'f' suffix
                    return value.Contains('.') ? $"{value}f" : $"{value}.0f";
                }
                return null;
            }

            if (targetType.FullStartsWith("int") ||
                targetType.FullStartsWith("long") ||
                targetType.FullStartsWith("byte"))
            {
                if (int.TryParse(value, out _))
                {
                    return value;
                }
                return null;
            }

            if (targetType.FullStartsWith("DateTime"))
            {
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    return $"DateTime.Parse(\"{value}\", System.Globalization.CultureInfo.InvariantCulture)";
                }
                return null;
            }


            if (targetType.FullStartsWith("System.Drawing.Point"))
            {
                string[] dims = value.Split(',');
                // If not a valid Point, skip it
                if (dims.Length != 2)
                {
                    return null;
                }
                var parts = dims.Select(s => int.TryParse(s.Trim(), out int val) ? val : (int?)null);
                // If not a valid Point, skip it
                if (parts.Contains(null))
                {
                    return null;
                }
                return $"new System.Drawing.Point({string.Join(", ", parts)})";
            }

            if (targetType.FullStartsWith("System.Drawing.Size"))
            {
                string[] dims = value.Split(',');
                // If not a valid Point, skip it
                if (dims.Length != 2)
                {
                    return null;
                }
                var parts = dims.Select(s => int.TryParse(s.Trim(), out int val) ? val : (int?)null);
                // If not a valid Point, skip it
                if (parts.Contains(null))
                {
                    return null;
                }
                return $"new System.Drawing.Size({string.Join(", ", parts)})";
            }

            if (targetType.FullStartsWith("System.Drawing.PointF"))
            {
                string[] dims = value.Split(',');
                // If not a valid PointF, skip it
                if (dims.Length != 2)
                {
                    return null;
                }
                var parts = dims.Select(s => float.TryParse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float val) ? val : (float?)null);
                // If not a valid Point, skip it
                if (parts.Contains(null))
                {
                    return null;
                }
                return $"new System.Drawing.PointF({string.Join(", ", dims.Select(d => d.Contains('.') ? $"{d}f" : $"{d}.0f"))})";
            }

            if (targetType.FullStartsWith("System.Text.Rune"))
            {
                return $"new System.Text.Rune('{value}')";
            }

            if(targetType.FullStartsWith("System.Collections.Generic.IReadOnlyList<string>")
                || targetType == "string[]")
            {
                string[] dims = value.Split(',');
                return $"[{string.Join(", ", dims.Select(d => $"\"{d.Replace("\"", "\\\"").Replace("\\", "\\\\")}\""))}]";
            }

            // For all other types (including interfaces, complex types, etc.)
            // Skip properties we can't set from literals
            if (targetType != "string")
            {
                // Complex types - skip them
                return null;
            }
        }

        // Default: treat as string property
        return $"\"{value.Replace("\"", "\\\"")}\"";
    }
}
