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
        public static readonly Dictionary<string, ControlMapping> ControlMappings = new()
        {
            ["Window"] = new ControlMapping("Terminal.Gui.Views.Window", "Terminal.Gui.Views", isContainer: true),
            ["View"] = new ControlMapping("Terminal.Gui.ViewBase.View", "Terminal.Gui.ViewBase", isContainer: true),
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
            ["MenuBarItem"] = new ControlMapping("Terminal.Gui.Views.MenuBarItem", "Terminal.Gui.Views", isContainer: true),
            ["MenuItem"] = new ControlMapping("Terminal.Gui.Views.MenuItem", "Terminal.Gui.Views"),
            ["StatusBar"] = new ControlMapping("Terminal.Gui.Views.StatusBar", "Terminal.Gui.Views", isContainer: true),
            ["Shortcut"] = new ControlMapping("Terminal.Gui.Views.Shortcut", "Terminal.Gui.Views")
        };

        /// <summary>
        /// Maps XAML event names to Terminal.Gui event names by control type
        /// NOTE: These mappings may need adjustment based on actual Terminal.Gui v2 API
        /// </summary>
        public static readonly Dictionary<string, Dictionary<string, EventMapping>> EventMappings = new()
        {
            ["Button"] = new Dictionary<string, EventMapping>
            {
                ["Accepting"] = new EventMapping("Accepting", "System.EventHandler<System.EventArgs>", "Button accepting event"),
                ["Clicked"] = new EventMapping("Clicked", "System.EventHandler<System.EventArgs>", "Button click event (legacy)", isObsolete: true, replacementEvent: "Accepting")
            },
            ["TextField"] = new Dictionary<string, EventMapping>
            {
                ["TextChanged"] = new EventMapping("TextChanged", "System.EventHandler<System.EventArgs>", "Text change event"),
                ["Accept"] = new EventMapping("Accept", "System.EventHandler<System.EventArgs>", "Accept input event")
            },
            ["CheckBox"] = new Dictionary<string, EventMapping>
            {
                ["ValueChanged"] = new EventMapping("ValueChanged", "System.EventHandler", "Value change event"),
                ["Toggled"] = new EventMapping("Toggled", "System.EventHandler", "Toggle state change event (legacy)", isObsolete: true, replacementEvent: "ValueChanged")
            },
            ["OptionSelector"] = new Dictionary<string, EventMapping>
            {
                ["SelectedItemChanged"] = new EventMapping("SelectedItemChanged", "System.EventHandler", "Selection change event")
            },
            ["ListView"] = new Dictionary<string, EventMapping>
            {
                ["ValueChanged"] = new EventMapping("ValueChanged", "System.EventHandler<Terminal.Gui.App.ValueChangedEventArgs<int?>>", "Selection change event"),
                ["ValueChanging"] = new EventMapping("ValueChanging", "System.EventHandler<Terminal.Gui.App.ValueChangingEventArgs<int?>>", "Selection changing event"),
                ["SourceChanged"] = new EventMapping("SourceChanged", "System.EventHandler", "Source data changed event"),
                ["CollectionChanged"] = new EventMapping("CollectionChanged", "System.Collections.Specialized.NotifyCollectionChangedEventHandler", "Collection changed event"),
                ["RowRender"] = new EventMapping("RowRender", "System.EventHandler<Terminal.Gui.Views.ListViewRowEventArgs>", "Row render event"),
                ["SelectedItemChanged"] = new EventMapping("SelectedItemChanged", "System.EventHandler", "Selection change event (legacy)", isObsolete: true, replacementEvent: "ValueChanged"),
                ["OpenSelectedItem"] = new EventMapping("OpenSelectedItem", "System.EventHandler", "Item activation event")
            },
            ["Window"] = new Dictionary<string, EventMapping>
            {
                ["Loaded"] = new EventMapping("Loaded", "System.EventHandler", "Window loaded event"),
                ["Closing"] = new EventMapping("Closing", "System.EventHandler", "Window closing event")
            },
            ["Shortcut"] = new Dictionary<string, EventMapping>
            {
                ["Accepting"] = new EventMapping("Accepting", "System.EventHandler<Terminal.Gui.Input.CommandEventArgs>", "Shortcut accepting event"),
                ["Accepted"] = new EventMapping("Accepted", "System.EventHandler<Terminal.Gui.Input.CommandEventArgs>", "Shortcut accepted event"),
                ["OrientationChanging"] = new EventMapping("OrientationChanging", "System.EventHandler<Terminal.Gui.App.ValueChangingEventArgs<Terminal.Gui.ViewBase.Orientation>>", "Orientation changing event"),
                ["OrientationChanged"] = new EventMapping("OrientationChanged", "System.EventHandler<Terminal.Gui.App.ValueChangedEventArgs<Terminal.Gui.ViewBase.Orientation>>", "Orientation changed event")
            },
            ["MenuItem"] = new Dictionary<string, EventMapping>
            {
                ["Accepting"] = new EventMapping("Accepting", "System.EventHandler<Terminal.Gui.Input.CommandEventArgs>", "MenuItem accepting event"),
                ["Accepted"] = new EventMapping("Accepted", "System.EventHandler<Terminal.Gui.Input.CommandEventArgs>", "MenuItem accepted event")
            },
            ["MenuBarItem"] = new Dictionary<string, EventMapping>
            {
                ["Accepting"] = new EventMapping("Accepting", "System.EventHandler<Terminal.Gui.Input.CommandEventArgs>", "MenuBarItem accepting event"),
                ["Accepted"] = new EventMapping("Accepted", "System.EventHandler<Terminal.Gui.Input.CommandEventArgs>", "MenuBarItem accepted event"),
                ["PopoverMenuOpenChanged"] = new EventMapping("PopoverMenuOpenChanged", "System.EventHandler", "Popover menu open state changed event")
            }
        };

        /// <summary>
        /// Maps properties that support TwoWay binding
        /// </summary>
        public static readonly Dictionary<string, Dictionary<string, TwoWayBinding>> TwoWayBindings = new()
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
                ["Checked"] = new TwoWayBinding("Checked", "ValueChanged", "bool", "Checked state with value change notification")
            },
            ["ListView"] = new Dictionary<string, TwoWayBinding>
            {
                ["Value"] = new TwoWayBinding("Value", "ValueChanged", "int?", "Selected item index with change notification"),
                ["Source"] = new TwoWayBinding("Source", "SourceChanged", "IListDataSource", "Data source with change notification")
            }
        };

        /// <summary>
        /// Maps properties by control type, with common properties shared across controls
        /// </summary>
        public static readonly Dictionary<string, Dictionary<string, PropertyMapping>> PropertyMappings = new()
        {
            ["Common"] = new Dictionary<string, PropertyMapping>
            {
                ["X"] = new PropertyMapping("X", "Terminal.Gui.Views.Pos", "Horizontal position"),
                ["Y"] = new PropertyMapping("Y", "Terminal.Gui.Views.Pos", "Vertical position"),
                ["Width"] = new PropertyMapping("Width", "Terminal.Gui.Views.Dim", "Width dimension"),
                ["Height"] = new PropertyMapping("Height", "Terminal.Gui.Views.Dim", "Height dimension"),
                ["Text"] = new PropertyMapping("Text", "ustring", "Text content"),
                ["Title"] = new PropertyMapping("Title", "ustring", "Title text"),
                ["Enabled"] = new PropertyMapping("Enabled", "bool", "Enabled state"),
                ["Visible"] = new PropertyMapping("Visible", "bool", "Visibility state")
            },
            ["TextField"] = new Dictionary<string, PropertyMapping>
            {
                ["ReadOnly"] = new PropertyMapping("ReadOnly", "bool", "Read-only state")
            },
            ["TextView"] = new Dictionary<string, PropertyMapping>
            {
                ["ReadOnly"] = new PropertyMapping("ReadOnly", "bool", "Read-only state")
            },
            ["CheckBox"] = new Dictionary<string, PropertyMapping>
            {
                ["Checked"] = new PropertyMapping("Checked", "bool", "Checked state")
            },
            ["OptionSelector"] = new Dictionary<string, PropertyMapping>
            {
                ["Options"] = new PropertyMapping("Options", "string[]", "Option selector items")
            },
            ["ListView"] = new Dictionary<string, PropertyMapping>
            {
                ["SelectedItem"] = new PropertyMapping("SelectedItem", "int", "Selected item index"),
                ["AllowMultipleSelection"] = new PropertyMapping("AllowMultipleSelection", "bool", "Allow multiple selection")
            },
            ["SpinnerView"] = new Dictionary<string, PropertyMapping>
            {
                ["IsSpinning"] = new PropertyMapping("IsSpinning", "bool", "Spinner active state")
            },
            ["Shortcut"] = new Dictionary<string, PropertyMapping>
            {
                ["Key"] = new PropertyMapping("Key", "Terminal.Gui.Input.Key", "Key binding (e.g., Key.F1, Key.Q.WithCtrl)"),
                ["HelpText"] = new PropertyMapping("HelpText", "ustring", "Help text displayed in the middle of the Shortcut"),
                ["Text"] = new PropertyMapping("Text", "ustring", "Alias for HelpText - Help text displayed in the middle"),
                ["BindKeyToApplication"] = new PropertyMapping("BindKeyToApplication", "bool", "Whether key is bound to application (HotKeyBindings) or view (KeyBindings)"),
                ["AlignmentModes"] = new PropertyMapping("AlignmentModes", "Terminal.Gui.ViewBase.AlignmentModes", "Alignment mode for the Shortcut content (StartToEnd or EndToStart)"),
                ["MinimumKeyTextSize"] = new PropertyMapping("MinimumKeyTextSize", "int", "Minimum size of the key text for alignment purposes"),
                ["ForceFocusColors"] = new PropertyMapping("ForceFocusColors", "bool", "Force focus colors"),
                ["Orientation"] = new PropertyMapping("Orientation", "Terminal.Gui.ViewBase.Orientation", "Orientation of the Shortcut (Horizontal or Vertical)")
            },
            ["MenuItem"] = new Dictionary<string, PropertyMapping>
            {
                ["HotKey"] = new PropertyMapping("HotKey", "Terminal.Gui.Input.Key", "Hot key for activation")
            },
            ["MenuBarItem"] = new Dictionary<string, PropertyMapping>
            {
                ["HotKey"] = new PropertyMapping("HotKey", "Terminal.Gui.Input.Key", "Hot key for activation"),
                ["PopoverMenuOpen"] = new PropertyMapping("PopoverMenuOpen", "bool", "Whether the popover menu is open")
            }
        };

        /// <summary>
        /// Gets the full type name for a control, with optional generic type parameter
        /// </summary>
        public static string? GetFullTypeName(string elementName, string? genericType = null)
        {
            if (!ControlMappings.TryGetValue(elementName, out var mapping))
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
        /// Gets property mapping information by property name only (searches all controls and Common)
        /// </summary>
        public static PropertyMapping? GetPropertyMapping(string propertyName)
        {
            // Check Common properties first
            if (PropertyMappings.TryGetValue("Common", out var commonProperties) &&
                commonProperties.TryGetValue(propertyName, out var commonMapping))
            {
                return commonMapping;
            }

            // Search in all control-specific properties
            foreach (var controlProperties in PropertyMappings.Values)
            {
                if (controlProperties.TryGetValue(propertyName, out var mapping))
                {
                    return mapping;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if an attribute name is a known event for any control type
        /// </summary>
        public static bool IsKnownEvent(string eventName)
        {
            foreach (var controlEvents in EventMappings.Values)
            {
                if (controlEvents.ContainsKey(eventName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if a property should be treated as a boolean value
        /// </summary>
        public static bool IsBooleanProperty(string propertyName)
        {
            var mapping = GetPropertyMapping(propertyName);
            return mapping?.TargetType == "bool";
        }

        /// <summary>
        /// Determines if a property should be treated as a numeric value (not quoted)
        /// </summary>
        public static bool IsNumericProperty(string propertyName)
        {
            var mapping = GetPropertyMapping(propertyName);
            return mapping?.TargetType == "int";
        }

        /// <summary>
        /// Determines if a property expects an array type
        /// </summary>
        public static bool IsArrayProperty(string propertyName)
        {
            var mapping = GetPropertyMapping(propertyName);
            return mapping?.TargetType.EndsWith("[]") ?? false;
        }

        /// <summary>
        /// Determines if a property is a Terminal.Gui type that needs full namespace qualification
        /// </summary>
        public static bool IsTerminalGuiType(string propertyName)
        {
            var mapping = GetPropertyMapping(propertyName);
            if (mapping == null) return false;

            return mapping.TargetType.StartsWith("Terminal.Gui.");
        }

        /// <summary>
        /// Gets the fully qualified type name for a property
        /// </summary>
        public static string? GetFullyQualifiedType(string propertyName)
        {
            var mapping = GetPropertyMapping(propertyName);
            return mapping?.TargetType;
        }
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
        public bool IsObsolete { get; }
        public string? ReplacementEvent { get; }

        public EventMapping(string eventName, string delegateType, string description, bool isObsolete = false, string? replacementEvent = null)
        {
            EventName = eventName;
            DelegateType = delegateType;
            Description = description;
            IsObsolete = isObsolete;
            ReplacementEvent = replacementEvent;
        }

        /// <summary>
        /// Gets the obsolete message if this event is obsolete
        /// </summary>
        public string? GetObsoleteMessage() => IsObsolete && ReplacementEvent != null
            ? $"Use '{ReplacementEvent}' instead"
            : null;
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
        public string TargetType { get; }
        public string Description { get; }

        public PropertyMapping(string propertyName, string targetType, string description)
        {
            PropertyName = propertyName;
            TargetType = targetType;
            Description = description;
        }
    }
}