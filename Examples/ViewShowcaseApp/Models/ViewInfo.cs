using Terminal.Gui.ViewBase;
using ViewShowcaseApp.Views;

namespace ViewShowcaseApp.Models;

public class ViewInfo
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Func<View> CreateView { get; set; } = () => new View();

    public override string ToString() => Name;
}

public static class ViewInfoCollection
{
    public static readonly List<ViewInfo> AllViews = 
    [
        // Basic Controls
        new ViewInfo() 
        { 
            Name = "Label", 
            Category = "Basic Controls",
            Description = "Text display control",
            CreateView = () => new LabelDemo() 
        },
        new ViewInfo() 
        { 
            Name = "Button", 
            Category = "Basic Controls",
            Description = "Clickable button",
            CreateView = () => new ButtonDemo() 
        },
        new ViewInfo() 
        { 
            Name = "TextField", 
            Category = "Basic Controls",
            Description = "Single-line text input",
            CreateView = () => new TextFieldDemo() 
        },
        new ViewInfo() 
        { 
            Name = "CheckBox", 
            Category = "Basic Controls",
            Description = "Checkbox control",
            CreateView = () => new CheckBoxDemo() 
        },
        new ViewInfo() 
        { 
            Name = "ProgressBar", 
            Category = "Basic Controls",
            Description = "Progress bar",
            CreateView = () => new ProgressBarDemo() 
        },

        // Selectors
        new ViewInfo() 
        { 
            Name = "ListView", 
            Category = "Selectors",
            Description = "Scrollable list",
            CreateView = () => new ListViewDemo() 
        },

        // Data Entry
        new ViewInfo() 
        { 
            Name = "DatePicker", 
            Category = "Data Entry",
            Description = "Visual date picker with calendar",
            CreateView = () => new DatePickerDemo() 
        },
        new ViewInfo() 
        { 
            Name = "NumericUpDown", 
            Category = "Data Entry",
            Description = "Numeric input with increment/decrement buttons",
            CreateView = () => new NumericUpDownDemo() 
        },

        // Containers
        new ViewInfo() 
        { 
            Name = "FrameView", 
            Category = "Containers",
            Description = "Container with border",
            CreateView = () => new FrameViewDemo() 
        },
        new ViewInfo() 
        { 
            Name = "TabView", 
            Category = "Containers",
            Description = "Tab container with multiple pages",
            CreateView = () => new TabViewDemo() 
        },
        // Data Display
        new ViewInfo() 
        { 
            Name = "TableView", 
            Category = "Data Display",
            Description = "Tabular data with scrolling",
            CreateView = () => new TableViewDemo() 
        },
        new ViewInfo() 
        { 
            Name = "TreeView", 
            Category = "Data Display",
            Description = "Hierarchical tree structure",
            CreateView = () => new TreeViewDemo() 
        },

        // Navigation
        new ViewInfo() 
        { 
            Name = "MenuBar", 
            Category = "Navigation",
            Description = "Application menu bar",
            CreateView = () => new MenuBarDemo() 
        },

        // Dialogs
        new ViewInfo() 
        { 
            Name = "Dialog", 
            Category = "Dialogs",
            Description = "Modal dialog window",
            CreateView = () => new DialogDemo() 
        },

        // Visual Elements
        new ViewInfo() 
        { 
            Name = "SpinnerView", 
            Category = "Visual Elements",
            Description = "Loading spinner animation",
            CreateView = () => new SpinnerViewDemo() 
        },
        new ViewInfo() 
        { 
            Name = "ColorPicker", 
            Category = "Visual Elements",
            Description = "Color selection control",
            CreateView = () => new ColorPickerDemo() 
        }
    ];

    public static ViewInfo DefaultView { get; } = new ViewInfo() 
    {
        Name = "UnknownView",
        Category = "Unknown",
        Description = "Unknown view",
        CreateView = () => new View()
    };

    /// <summary>
    /// Gets all views (same as AllViews, kept for compatibility)
    /// </summary>
    public static IEnumerable<ViewInfo> GetAllViews() => AllViews;

    /// <summary>
    /// Gets views grouped by category
    /// </summary>
    public static IEnumerable<IGrouping<string, ViewInfo>> GetViewsByCategory() => 
        AllViews.GroupBy(v => v.Category);

    /// <summary>
    /// Gets all unique categories
    /// </summary>
    public static IEnumerable<string> GetCategories() => 
        AllViews.Select(v => v.Category).Distinct().OrderBy(c => c);
}