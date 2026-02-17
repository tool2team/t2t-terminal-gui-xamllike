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
            ["ProgressBar"] = new ControlMapping("Terminal.Gui.Views.ProgressBar", "Terminal.Gui.Views"),
            ["MenuBar"] = new ControlMapping("Terminal.Gui.Views.MenuBar", "Terminal.Gui.Views"),
            ["MenuBarItem"] = new ControlMapping("Terminal.Gui.Views.MenuBarItem", "Terminal.Gui.Views", isContainer: true),
            ["MenuItem"] = new ControlMapping("Terminal.Gui.Views.MenuItem", "Terminal.Gui.Views"),
            ["StatusBar"] = new ControlMapping("Terminal.Gui.Views.StatusBar", "Terminal.Gui.Views", isContainer: true),
            ["Shortcut"] = new ControlMapping("Terminal.Gui.Views.Shortcut", "Terminal.Gui.Views"),
            ["Slider"] = new ControlMapping("Terminal.Gui.Views.Slider", "Terminal.Gui.Views"),
            ["TileView"] = new ControlMapping("Terminal.Gui.Views.TileView", "Terminal.Gui.Views", isContainer: true),
            ["AttributePicker"] = new ControlMapping("Terminal.Gui.Views.AttributePicker", "Terminal.Gui.Views"),
            ["AutocompleteTextField"] = new ControlMapping("Terminal.Gui.Views.AutocompleteTextField", "Terminal.Gui.Views"),
            ["MessageBox"] = new ControlMapping("Terminal.Gui.Views.MessageBox", "Terminal.Gui.Views"),
            ["DateField"] = new ControlMapping("Terminal.Gui.Views.DateField", "Terminal.Gui.Views"),
            ["TimeField"] = new ControlMapping("Terminal.Gui.Views.TimeField", "Terminal.Gui.Views"),
            ["DatePicker"] = new ControlMapping("Terminal.Gui.Views.DatePicker", "Terminal.Gui.Views"),
            ["TextValidateField"] = new ControlMapping("Terminal.Gui.Views.TextValidateField", "Terminal.Gui.Views"),
            ["NumericUpDown"] = new ControlMapping("Terminal.Gui.Views.NumericUpDown", "Terminal.Gui.Views"),
            ["SpinnerView"] = new ControlMapping("Terminal.Gui.Views.SpinnerView", "Terminal.Gui.Views"),
            ["ColorPicker"] = new ControlMapping("Terminal.Gui.Views.ColorPicker", "Terminal.Gui.Views"),
            ["ColorPicker16"] = new ControlMapping("Terminal.Gui.Views.ColorPicker16", "Terminal.Gui.Views"),
            ["TableView"] = new ControlMapping("Terminal.Gui.Views.TableView", "Terminal.Gui.Views"),
            ["TreeView"] = new ControlMapping("Terminal.Gui.Views.TreeView", "Terminal.Gui.Views"),
            ["GraphView"] = new ControlMapping("Terminal.Gui.Views.GraphView", "Terminal.Gui.Views"),
            ["CharMap"] = new ControlMapping("Terminal.Gui.Views.CharMap", "Terminal.Gui.Views"),
            ["HexView"] = new ControlMapping("Terminal.Gui.Views.HexView", "Terminal.Gui.Views"),
            ["Line"] = new ControlMapping("Terminal.Gui.Views.Line", "Terminal.Gui.Views"),
            ["LinearRange"] = new ControlMapping("Terminal.Gui.Views.LinearRange", "Terminal.Gui.Views"),
            ["Menu"] = new ControlMapping("Terminal.Gui.Views.Menu", "Terminal.Gui.Views", isContainer: true),
            ["Bar"] = new ControlMapping("Terminal.Gui.Views.Bar", "Terminal.Gui.Views", isContainer: true),
            ["OpenDialog"] = new ControlMapping("Terminal.Gui.Views.OpenDialog", "Terminal.Gui.Views", isContainer: true),
            ["SaveDialog"] = new ControlMapping("Terminal.Gui.Views.SaveDialog", "Terminal.Gui.Views", isContainer: true),
            ["FileDialog"] = new ControlMapping("Terminal.Gui.Views.FileDialog", "Terminal.Gui.Views", isContainer: true),
            ["Dialog"] = new ControlMapping("Terminal.Gui.Views.Dialog", "Terminal.Gui.Views", isContainer: true),
            ["Wizard"] = new ControlMapping("Terminal.Gui.Views.Wizard", "Terminal.Gui.Views", isContainer: true),
            ["WizardStep"] = new ControlMapping("Terminal.Gui.Views.WizardStep", "Terminal.Gui.Views", isContainer: true),
            ["ScrollBar"] = new ControlMapping("Terminal.Gui.Views.ScrollBar", "Terminal.Gui.Views"),
            ["Tab"] = new ControlMapping("Terminal.Gui.Views.Tab", "Terminal.Gui.Views", isContainer: true),
            ["FlagSelector"] = new ControlMapping("Terminal.Gui.Views.FlagSelector", "Terminal.Gui.Views")
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
            },
            ["DateField"] = new Dictionary<string, EventMapping>
            {
                ["ValueChanged"] = new EventMapping("ValueChanged", "System.EventHandler", "Date change event")
            },
            ["TimeField"] = new Dictionary<string, EventMapping>
            {
                ["ValueChanged"] = new EventMapping("ValueChanged", "System.EventHandler", "Time change event")
            },
            ["DatePicker"] = new Dictionary<string, EventMapping>
            {
                ["ValueChanged"] = new EventMapping("ValueChanged", "System.EventHandler", "Date change event")
            },
            ["ColorPicker"] = new Dictionary<string, EventMapping>
            {
                ["ValueChanged"] = new EventMapping("ValueChanged", "System.EventHandler", "Color change event")
            },
            ["ColorPicker16"] = new Dictionary<string, EventMapping>
            {
                ["ValueChanged"] = new EventMapping("ValueChanged", "System.EventHandler", "Color change event")
            },
            ["AttributePicker"] = new Dictionary<string, EventMapping>
            {
                ["AttributeChanged"] = new EventMapping("AttributeChanged", "System.EventHandler", "Attribute change event")
            },
            ["AutocompleteTextField"] = new Dictionary<string, EventMapping>
            {
                ["TextChanged"] = new EventMapping("TextChanged", "System.EventHandler", "Text change event")
            },
            ["NumericUpDown"] = new Dictionary<string, EventMapping>
            {
                ["ValueChanged"] = new EventMapping("ValueChanged", "System.EventHandler", "Value change event")
            },
            ["TableView"] = new Dictionary<string, EventMapping>
            {
                ["SelectedCellChanged"] = new EventMapping("SelectedCellChanged", "System.EventHandler<Terminal.Gui.Views.SelectedCellChangedEventArgs>", "Selected cell change event"),
                ["CellActivated"] = new EventMapping("CellActivated", "System.EventHandler<Terminal.Gui.Views.CellActivatedEventArgs>", "Cell activated event")
            },
            ["TreeView"] = new Dictionary<string, EventMapping>
            {
                ["SelectionChanged"] = new EventMapping("SelectionChanged", "System.EventHandler", "Selection change event"),
                ["ObjectActivated"] = new EventMapping("ObjectActivated", "System.EventHandler", "Object activation event")
            },
            ["TextValidateField"] = new Dictionary<string, EventMapping>
            {
                ["TextChanged"] = new EventMapping("TextChanged", "System.EventHandler", "Text change event")
            }
        };

        /// <summary>
        /// Maps properties that support TwoWay binding
        /// </summary>
        public static readonly Dictionary<string, Dictionary<string, TwoWayBinding>> TwoWayBindings = new()
        {
            ["TextField"] = new Dictionary<string, TwoWayBinding>
            {
                ["Text"] = new TwoWayBinding("Text", "TextChanged", "Text property with change notification")
            },
            ["TextView"] = new Dictionary<string, TwoWayBinding>
            {
                ["Text"] = new TwoWayBinding("Text", "TextChanged", "Text property with change notification")
            },
            ["CheckBox"] = new Dictionary<string, TwoWayBinding>
            {
                ["Value"] = new TwoWayBinding("Value", "ValueChanged", "Checked state with value change notification")
            },
            ["ListView"] = new Dictionary<string, TwoWayBinding>
            {
                ["Value"] = new TwoWayBinding("Value", "ValueChanged", "Selected item index with change notification"),
                ["Source"] = new TwoWayBinding("Source", "SourceChanged", "Data source with change notification")
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
                ["Visible"] = new PropertyMapping("Visible", "bool", "Visibility state"),
                ["CanFocus"] = new PropertyMapping("CanFocus", "bool", "Whether the view can receive focus")
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
                ["Value"] = new PropertyMapping("Value", "Terminal.Gui.Views.CheckState", "Checked state")
            },
            ["Button"] = new Dictionary<string, PropertyMapping>
            {
                ["IsDefault"] = new PropertyMapping("IsDefault", "bool", "Whether this button is the default button in a dialog"),
                ["IsDialogButton"] = new PropertyMapping("IsDialogButton", "bool", "Set to true to add this button to Dialog.Buttons collection (AddButton) instead of regular child (Add). Default is false.")
            },
            ["Dialog"] = new Dictionary<string, PropertyMapping>
            {
                ["ButtonAlignment"] = new PropertyMapping("ButtonAlignment", "Terminal.Gui.ViewBase.Alignment", "Button alignment (Start, Center, End, Fill, Justified)"),
                ["ButtonAlignmentModes"] = new PropertyMapping("ButtonAlignmentModes", "Terminal.Gui.ViewBase.AlignmentModes", "Button alignment modes (StartToEnd or EndToStart)")
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
                ["AutoSpin"] = new PropertyMapping("AutoSpin", "bool", "Spinner active state")
            },
            ["ProgressBar"] = new Dictionary<string, PropertyMapping>
            {
                ["Fraction"] = new PropertyMapping("Fraction", "float", "Progress fraction between 0 and 1"),
                ["ProgressBarFormat"] = new PropertyMapping("ProgressBarFormat", "Terminal.Gui.Views.ProgressBarFormat", "Format of the progress bar (Simple, SimplePlusPercentage, Framed, FramedProgressPadded)"),
                ["ProgressBarStyle"] = new PropertyMapping("ProgressBarStyle", "Terminal.Gui.Views.ProgressBarStyle", "Style of the progress bar (Blocks, Continuous, MarqueeBlocks, MarqueeContinuous)"),
                ["BidirectionalMarquee"] = new PropertyMapping("BidirectionalMarquee", "bool", "Whether marquee styles are bidirectional"),
                ["SegmentCharacter"] = new PropertyMapping("SegmentCharacter", "System.Text.Rune", "Character used for segments in meter views")
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
            },
            ["DateField"] = new Dictionary<string, PropertyMapping>
            {
                ["Value"] = new PropertyMapping("Value", "System.DateTime", "Date value"),
                ["Format"] = new PropertyMapping("Format", "string", "Date format string")
            },
            ["TimeField"] = new Dictionary<string, PropertyMapping>
            {
                ["Value"] = new PropertyMapping("Value", "System.TimeSpan", "Time value"),
                ["Format"] = new PropertyMapping("Format", "string", "Time format string")
            },
            ["DatePicker"] = new Dictionary<string, PropertyMapping>
            {
                ["Value"] = new PropertyMapping("Value", "System.DateTime", "Selected date")
            },
            ["ColorPicker"] = new Dictionary<string, PropertyMapping>
            {
                ["SelectedColor"] = new PropertyMapping("SelectedColor", "Terminal.Gui.Drawing.Color", "Selected color")
            },
            ["ColorPicker16"] = new Dictionary<string, PropertyMapping>
            {
                ["SelectedColor"] = new PropertyMapping("SelectedColor", "Terminal.Gui.Drawing.Color", "Selected color")
            },
            ["AttributePicker"] = new Dictionary<string, PropertyMapping>
            {
                ["SelectedAttribute"] = new PropertyMapping("SelectedAttribute", "Terminal.Gui.Drawing.Attribute", "Selected attribute")
            },
            ["AutocompleteTextField"] = new Dictionary<string, PropertyMapping>
            {
                ["Text"] = new PropertyMapping("Text", "ustring", "Text content"),
                ["AutocompleteSource"] = new PropertyMapping("AutocompleteSource", "Terminal.Gui.Views.IAutocomplete", "Autocomplete source")
            },
            ["NumericUpDown"] = new Dictionary<string, PropertyMapping>
            {
                ["Value"] = new PropertyMapping("Value", "int", "Numeric value"),
                ["Minimum"] = new PropertyMapping("Minimum", "int", "Minimum value"),
                ["Maximum"] = new PropertyMapping("Maximum", "int", "Maximum value"),
                ["Increment"] = new PropertyMapping("Increment", "int", "Increment amount")
            },
            ["TableView"] = new Dictionary<string, PropertyMapping>
            {
                ["Table"] = new PropertyMapping("Table", "Terminal.Gui.Views.ITableSource", "Data table source"),
                ["SelectedRow"] = new PropertyMapping("SelectedRow", "int", "Selected row index"),
                ["SelectedColumn"] = new PropertyMapping("SelectedColumn", "int", "Selected column index")
            },
            ["TreeView"] = new Dictionary<string, PropertyMapping>
            {
                ["TreeSource"] = new PropertyMapping("TreeSource", "object", "Tree data source"),
                ["SelectedObject"] = new PropertyMapping("SelectedObject", "object", "Selected item")
            },
            ["TextValidateField"] = new Dictionary<string, PropertyMapping>
            {
                ["Text"] = new PropertyMapping("Text", "ustring", "Text content"),
                ["ValidationPattern"] = new PropertyMapping("ValidationPattern", "string", "Validation regex pattern")
            },
            ["CharMap"] = new Dictionary<string, PropertyMapping>
            {
                ["StartCodePoint"] = new PropertyMapping("StartCodePoint", "int", "Starting Unicode code point")
            },
            ["HexView"] = new Dictionary<string, PropertyMapping>
            {
                ["Source"] = new PropertyMapping("Source", "System.IO.Stream", "Data source stream")
            },
            ["Line"] = new Dictionary<string, PropertyMapping>
            {
                ["Orientation"] = new PropertyMapping("Orientation", "Terminal.Gui.ViewBase.Orientation", "Orientation (Horizontal or Vertical)"),
                ["LineStyle"] = new PropertyMapping("LineStyle", "Terminal.Gui.Drawing.LineStyle", "Line drawing style")
            },
            ["LinearRange"] = new Dictionary<string, PropertyMapping>
            {
                ["Value"] = new PropertyMapping("Value", "object", "Current value")
            },
            ["MessageBox"] = new Dictionary<string, PropertyMapping>
            {
                ["Message"] = new PropertyMapping("Message", "ustring", "Message content"),
                ["Buttons"] = new PropertyMapping("Buttons", "string[]", "Button labels")
            },
            ["TileView"] = new Dictionary<string, PropertyMapping>
            {
                ["Orientation"] = new PropertyMapping("Orientation", "Terminal.Gui.ViewBase.Orientation", "Orientation (Horizontal or Vertical)")
            },
            ["SpinnerView"] = new Dictionary<string, PropertyMapping>
            {
                ["Style"] = new PropertyMapping("Style", "Terminal.Gui.Views.SpinnerStyle", "Spinner style")
            },
            ["ScrollBar"] = new Dictionary<string, PropertyMapping>
            {
                ["Value"] = new PropertyMapping("Value", "int", "Scroll position")
            },
            ["FlagSelector"] = new Dictionary<string, PropertyMapping>
            {
                ["Value"] = new PropertyMapping("Value", "object", "Selected flags value")
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
        /// Determines if a property should be treated as an integer value
        /// </summary>
        public static bool IsIntProperty(string propertyName)
        {
            var mapping = GetPropertyMapping(propertyName);
            return mapping?.TargetType == "int";
        }

        /// <summary>
        /// Determines if a property should be treated as a float value
        /// </summary>
        public static bool IsFloatProperty(string propertyName)
        {
            var mapping = GetPropertyMapping(propertyName);
            return mapping?.TargetType == "float";
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
        public string Description { get; }

        public TwoWayBinding(string propertyName, string changeEventName, string description)
        {
            PropertyName = propertyName;
            ChangeEventName = changeEventName;
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