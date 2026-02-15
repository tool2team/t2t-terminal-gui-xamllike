using Terminal.Gui.ViewBase;
using ViewShowcaseApp.Views;

namespace ViewShowcaseApp.Models;

public class ViewInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Func<View> CreateView { get; set; } = () => new View();

    public override string ToString() => Name;
}

public static class ViewInfoCollection
{
    static readonly List<ViewInfo> Views =
    [
        // Basic Controls
        new() { Name = "Label", Category = "Basic Controls", Description = "Text display control", 
                CreateView = () => new LabelDemo() },

        new() { Name = "Button", Category = "Basic Controls", Description = "Clickable button", 
                CreateView = () => new ButtonDemo() },

        new() { Name = "TextField", Category = "Basic Controls", Description = "Single-line text input", 
                CreateView = () => new TextFieldDemo() },

        new() { Name = "CheckBox", Category = "Basic Controls", Description = "Checkbox control", 
                CreateView = () => new CheckBoxDemo() },

        // Selectors
        new() { Name = "ListView", Category = "Selectors", Description = "Scrollable list", 
                CreateView = () => new ListViewDemo() },

        // Containers
        new() { Name = "FrameView", Category = "Containers", Description = "Container with border", 
                CreateView = () => new FrameViewDemo() },

        // Note: Other views (TextView, DateField, etc.) can be added with similar XAML files
    ];

    public static ViewInfo DefaultView { get; } = new ViewInfo() 
    {
        Name = "UnknownView", Category = "Default", Description = "Unknown view",
        CreateView = () => new View()
    };

    public static IEnumerable<string> GetCategories() => Views.Select(v => v.Category).Distinct().OrderBy(c => c);

    public static IEnumerable<ViewInfo> GetViewsInCategory(string category) => 
        Views.Where(v => v.Category == category).OrderBy(v => v.Name);

    public static IEnumerable<ViewInfo> GetAllViews() => Views.OrderBy(v => v.Name);
}