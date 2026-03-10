// Utility methods for accessing Terminal.Gui mappings
// This file contains helper methods - data is in auto-generated Mappings.cs

#nullable enable

using System.Collections.Generic;

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
