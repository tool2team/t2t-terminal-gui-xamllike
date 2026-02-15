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

        // Containers
        new ViewInfo() 
        { 
            Name = "FrameView", 
            Category = "Containers",
            Description = "Container with border",
            CreateView = () => new FrameViewDemo() 
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