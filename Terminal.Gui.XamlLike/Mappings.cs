using System.Collections.Generic;

namespace Terminal.Gui.XamlLike
{
    /// <summary>
    /// Provides mappings between XAML elements/attributes and Terminal.Gui v2 API
    /// </summary>
    public static class Mappings
    {
        /// <summary>
        /// Maps XAML element names to Terminal.Gui control types
        /// </summary>
        public static readonly Dictionary<string, ControlMapping> ControlMappings = new Dictionary<string, ControlMapping>
        {
            ["Window"] = new ControlMapping("Terminal.Gui.Views.Window", "Terminal.Gui.Views", isContainer: true),
            ["Label"] = new ControlMapping("Terminal.Gui.Views.Label", "Terminal.Gui.Views"),
            ["Button"] = new ControlMapping("Terminal.Gui.Views.Button", "Terminal.Gui.Views"),
            ["TextField"] = new ControlMapping("Terminal.Gui.Views.TextField", "Terminal.Gui.Views"),
            ["TextView"] = new ControlMapping("Terminal.Gui.Views.TextView", "Terminal.Gui.Views"),
            ["CheckBox"] = new ControlMapping("Terminal.Gui.Views.CheckBox", "Terminal.Gui.Views"),
            ["OptionSelector"] = new ControlMapping("Terminal.Gui.Views.OptionSelector", "Terminal.Gui.Views", isContainer: false),
            ["ListView"] = new ControlMapping("Terminal.Gui.Views.ListView", "Terminal.Gui.Views"),
            ["FrameView"] = new ControlMapping("Terminal.Gui.Views.FrameView", "Terminal.Gui.Views", isContainer: true),
            ["ScrollView"] = new ControlMapping("Terminal.Gui.Views.ScrollView", "Terminal.Gui.Views", isContainer: true),
            ["TabView"] = new ControlMapping("Terminal.Gui.Views.TabView", "Terminal.Gui.Views", isContainer: true),
            ["MenuBar"] = new ControlMapping("Terminal.Gui.Views.MenuBar", "Terminal.Gui.Views"),
            ["StatusBar"] = new ControlMapping("Terminal.Gui.Views.StatusBar", "Terminal.Gui.Views")
        };

        /// <summary>
        /// Maps XAML event names to Terminal.Gui event names by control type
        /// NOTE: These mappings may need adjustment based on actual Terminal.Gui v2 API
        /// </summary>
        public static readonly Dictionary<string, Dictionary<string, EventMapping>> EventMappings = new Dictionary<string, Dictionary<string, EventMapping>>
        {
            ["Button"] = new Dictionary<string, EventMapping>
            {
                ["Accepting"] = new EventMapping("Accepting", "System.EventHandler<System.EventArgs>", "Button accepting event in v2")
            },
            ["TextField"] = new Dictionary<string, EventMapping>
            {
                ["TextChanged"] = new EventMapping("TextChanged", "System.EventHandler<System.EventArgs>", "Text change event"),
                ["Accept"] = new EventMapping("Accept", "System.EventHandler<System.EventArgs>", "Accept input event")
            },
            ["CheckBox"] = new Dictionary<string, EventMapping>
            {
                ["Toggled"] = new EventMapping("Toggled", "System.EventHandler", "Toggle state change event")
            },
            ["OptionSelector"] = new Dictionary<string, EventMapping>
            {
                ["SelectedItemChanged"] = new EventMapping("SelectedItemChanged", "System.EventHandler", "Selection change event")
            },
            ["ListView"] = new Dictionary<string, EventMapping>
            {
                ["SelectedItemChanged"] = new EventMapping("SelectedItemChanged", "System.EventHandler", "Selection change event"),
                ["OpenSelectedItem"] = new EventMapping("OpenSelectedItem", "System.EventHandler", "Item activation event")
            },
            ["Window"] = new Dictionary<string, EventMapping>
            {
                ["Loaded"] = new EventMapping("Loaded", "System.EventHandler", "Window loaded event"),
                ["Closing"] = new EventMapping("Closing", "System.EventHandler", "Window closing event")
            }
        };

        /// <summary>
        /// Maps properties that support TwoWay binding
        /// </summary>
        public static readonly Dictionary<string, Dictionary<string, TwoWayBinding>> TwoWayBindings = new Dictionary<string, Dictionary<string, TwoWayBinding>>
        {
            ["TextField"] = new Dictionary<string, TwoWayBinding>
            {
                ["Text"] = new TwoWayBinding("Text", "TextChanged", "ustring", "Text property with change notification")
            },
            ["TextView"] = new Dictionary<string, TwoWayBinding>
            {
                ["Text"] = new TwoWayBinding("Text", "TextChanged", "ustring", "Text property with change notification")
            },
            ["CheckBox"] = new Dictionary<string, TwoWayBinding>
            {
                ["Checked"] = new TwoWayBinding("Checked", "Toggled", "bool", "Checked state with toggle notification")
            }
        };

        /// <summary>
        /// Common property mappings that may need special handling
        /// </summary>
        public static readonly Dictionary<string, PropertyMapping> PropertyMappings = new Dictionary<string, PropertyMapping>
        {
            ["X"] = new PropertyMapping("X", "Pos", "Terminal.Gui.Views.Pos", "Horizontal position"),
            ["Y"] = new PropertyMapping("Y", "Pos", "Terminal.Gui.Views.Pos", "Vertical position"),
            ["Width"] = new PropertyMapping("Width", "Dim", "Terminal.Gui.Views.Dim", "Width dimension"),
            ["Height"] = new PropertyMapping("Height", "Dim", "Terminal.Gui.Views.Dim", "Height dimension"),
            ["Text"] = new PropertyMapping("Text", "string", "ustring", "Text content"),
            ["Title"] = new PropertyMapping("Title", "string", "ustring", "Title text"),
            ["Checked"] = new PropertyMapping("Checked", "bool", "bool", "Checked state"),
            ["Enabled"] = new PropertyMapping("Enabled", "bool", "bool", "Enabled state"),
            ["Visible"] = new PropertyMapping("Visible", "bool", "bool", "Visibility state"),
            ["SelectedItem"] = new PropertyMapping("SelectedItem", "int", "int", "Selected item index"),
            ["Options"] = new PropertyMapping("Options", "string[]", "string[]", "Option selector items")
        };

        /// <summary>
        /// Gets the full type name for a control, with optional generic type parameter
        /// </summary>
        public static string GetControlTypeName(string elementName, string? genericType = null)
        {
            if (!ControlMappings.TryGetValue(elementName, out var mapping))
                return $"Terminal.Gui.Views.{elementName}";

            var typeName = mapping.FullTypeName;

            // If this control supports generics and a generic type is specified, add it
            if (!string.IsNullOrEmpty(genericType) && elementName == "OptionSelector")
            {
                typeName = $"{typeName}<{genericType}>";
            }

            return typeName;
        }

        /// <summary>
        /// Checks if a control is a container (can have children)
        /// </summary>
        public static bool IsContainer(string elementName) =>
            ControlMappings.TryGetValue(elementName, out var mapping) && mapping.IsContainer;

        /// <summary>
        /// Gets the event mapping for a control/event combination
        /// </summary>
        public static EventMapping? GetEventMapping(string controlName, string eventName) =>
            EventMappings.TryGetValue(controlName, out var events) && 
            events.TryGetValue(eventName, out var eventMapping) 
                ? eventMapping 
                : null;

        /// <summary>
        /// Gets TwoWay binding information for a control/property combination
        /// </summary>
        public static TwoWayBinding? GetTwoWayBinding(string controlName, string propertyName) =>
            TwoWayBindings.TryGetValue(controlName, out var properties) && 
            properties.TryGetValue(propertyName, out var binding) 
                ? binding 
                : null;

        /// <summary>
        /// Checks if a property supports TwoWay binding
        /// </summary>
        public static bool SupportsTwoWayBinding(string controlName, string propertyName) =>
            GetTwoWayBinding(controlName, propertyName) != null;

        /// <summary>
        /// Gets property mapping information
        /// </summary>
        public static PropertyMapping? GetPropertyMapping(string propertyName) =>
            PropertyMappings.TryGetValue(propertyName, out var mapping) ? mapping : null;

        /// <summary>
        /// Determines if a property value needs special expression handling
        /// </summary>
        public static bool IsExpressionProperty(string propertyName) => propertyName switch
        {
            "X" or "Y" or "Width" or "Height" => true,
            _ => false
        };

        /// <summary>
        /// Determines if a property should be treated as a numeric value (not quoted)
        /// </summary>
        public static bool IsNumericProperty(string propertyName) => propertyName switch
        {
            "SelectedItem" => true,
            _ => false
        };

        /// <summary>
        /// Determines if a property expects an array type
        /// </summary>
        public static bool IsArrayProperty(string propertyName) => propertyName switch
        {
            "Options" => true,
            _ => false
        };
    }

    /// <summary>
    /// Information about a Terminal.Gui control type
    /// </summary>
    public class ControlMapping
    {
        public string FullTypeName { get; }
        public string Namespace { get; }
        public bool IsContainer { get; }
        public string? Description { get; }

        public ControlMapping(string fullTypeName, string namespaceName, bool isContainer = false, string? description = null)
        {
            FullTypeName = fullTypeName;
            Namespace = namespaceName;
            IsContainer = isContainer;
            Description = description;
        }
    }

    /// <summary>
    /// Information about an event mapping
    /// </summary>
    public class EventMapping
    {
        public string EventName { get; }
        public string DelegateType { get; }
        public string Description { get; }

        public EventMapping(string eventName, string delegateType, string description)
        {
            EventName = eventName;
            DelegateType = delegateType;
            Description = description;
        }
    }

    /// <summary>
    /// Information about a TwoWay bindable property
    /// </summary>
    public class TwoWayBinding
    {
        public string PropertyName { get; }
        public string ChangeEventName { get; }
        public string PropertyType { get; }
        public string Description { get; }

        public TwoWayBinding(string propertyName, string changeEventName, string propertyType, string description)
        {
            PropertyName = propertyName;
            ChangeEventName = changeEventName;
            PropertyType = propertyType;
            Description = description;
        }
    }

    /// <summary>
    /// Information about a property mapping
    /// </summary>
    public class PropertyMapping
    {
        public string PropertyName { get; }
        public string XamlType { get; }
        public string TargetType { get; }
        public string Description { get; }

        public PropertyMapping(string propertyName, string xamlType, string targetType, string description)
        {
            PropertyName = propertyName;
            XamlType = xamlType;
            TargetType = targetType;
            Description = description;
        }
    }
}