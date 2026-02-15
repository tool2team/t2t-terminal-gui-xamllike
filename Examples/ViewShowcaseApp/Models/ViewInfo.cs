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
    public static readonly Dictionary<string, List<ViewInfo>> ViewsByCategory = new()
    {
        { "Basic Controls", [
            new ViewInfo() { Name = "Label", Description = "Text display control",
                    CreateView = () => new LabelDemo() },

            new ViewInfo() { Name = "Button", Description = "Clickable button",
                    CreateView = () => new ButtonDemo() },

            new ViewInfo() { Name = "TextField", Description = "Single-line text input",
                    CreateView = () => new TextFieldDemo() },

            new ViewInfo() { Name = "CheckBox", Description = "Checkbox control",
                    CreateView = () => new CheckBoxDemo() },

            new ViewInfo() { Name = "ProgressBar", Description = "Progress bar",
                    CreateView = () => new ProgressBarDemo() }
        ]},

        { "Selectors", [
            new ViewInfo() { Name = "ListView", Category = "Selectors", Description = "Scrollable list",
                    CreateView = () => new ListViewDemo() },

            // Containers
            new ViewInfo() { Name = "FrameView", Category = "Containers", Description = "Container with border",
                    CreateView = () => new FrameViewDemo() },
        ]}
        // Note: Other views (TextView, DateField, etc.) can be added with similar XAML files
    };

    public static ViewInfo DefaultView { get; } = new ViewInfo() 
    {
        Name = "UnknownView", Category = "Default", Description = "Unknown view",
        CreateView = () => new View()
    };

    public static IEnumerable<ViewInfo> GetAllViews() => ViewsByCategory.SelectMany(k => k.Value);
}