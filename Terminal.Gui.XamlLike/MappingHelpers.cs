// Utility methods for accessing Terminal.Gui mappings
// This file contains helper methods - data is in auto-generated Mappings.cs

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Terminal.Gui.XamlLike;

/// <summary>
/// Provides utility methods to access Terminal.Gui control mappings
/// The actual data dictionaries are in the auto-generated Mappings.cs file (partial class)
/// </summary>
public static partial class MappingHelpers
{
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
            var targetType = propertyMapping.TargetType;

            // Handle Terminal.Gui types (Pos, Dim, Key, Enum, etc.)
            if (targetType.StartsWith("Terminal.Gui."))
            {
                // Special handling for Pos and Dim types
                if (targetType == "Terminal.Gui.ViewBase.Pos" || targetType == "Terminal.Gui.ViewBase.Dim")
                {
                    // If value contains parentheses, it's already an expression like Dim.Fill()
                    if (value.Contains("(") && value.Contains(")"))
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

                    if (parts[0] == typeShortName || $"System.Nullable<{parts[0]}>" == typeShortName)
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
            if (targetType == "System.Boolean" || targetType == "System.Nullable<System.Boolean>")
            {
                if (bool.TryParse(value, out var boolValue))
                {
                    return boolValue ? "true" : "false";
                }
                // If not a valid boolean, skip it
                return null;
            }

            if (targetType == "System.Single" || targetType == "System.Nullable<System.Single>")
            {
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _))
                {
                    // Ensure float literal has 'f' suffix
                    return value.Contains('.') ? $"{value}f" : $"{value}.0f";
                }
                return null;
            }

            if (targetType == "System.Int32" || targetType == "System.Nullable<System.Int32>" ||
                targetType == "System.Int64" || targetType == "System.Nullable<<System.Int64>" ||
                targetType == "System.Byte" || targetType == "System.Nullable<<System.Byte>")
            {
                if (int.TryParse(value, out _))
                {
                    return value; // Return numeric value without quotes
                }
                return null;
            }

            if (targetType == "System.DateTime" || targetType == "System.Nullable<System.DateTime>")
            {
                if (DateTime.TryParse(value, out _))
                {
                    return $"DateTime.Parse(\"{value}\")";
                }
                return null;
            }

            // For all other types (including interfaces, complex types, etc.)
            // Skip properties we can't set from literals
            if (targetType != "System.String" && targetType != "string")
            {
                // Complex types - skip them
                return null;
            }
        }

        // Default: treat as string property
        return $"\"{value.Replace("\"", "\\\"")}\"";
    }

    /// <summary>
    /// Determines if a property is a Terminal.Gui type that needs full namespace qualification
    /// </summary>
    public static bool IsTerminalGuiType(string propertyName)
    {
        // Search in all control-specific properties if not found yet
        foreach (Dictionary<string, PropertyMapping> properties in Mappings.PropertyMappings.Values)
        {
            if (properties.TryGetValue(propertyName, out PropertyMapping? mapping))
            {
                return mapping.TargetType.StartsWith("Terminal.Gui.");
            }
        }
        return false;
    }
}
