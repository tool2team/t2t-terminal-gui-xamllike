using Terminal.Gui.App;
using Terminal.Gui.ViewBase;

namespace ViewShowcaseApp.Views;

public partial class ListViewDemo : View
{
    public ListViewDemo()
    {
        InitializeComponent();

        DemoList.Data = new[]
        {
            "Apple",
            "Banana",
            "Cherry",
            "Date",
            "Elderberry",
            "Fig",
            "Grape",
            "Honeydew"
        };
    }

    private void OnValueChanged(object? sender, ValueChangedEventArgs<int?> e)
    {
        int selectedValue = e.NewValue ?? 0;
        if (DemoList.Value >= 0 && DemoList.Data is string[] items)
        {
            SelectionLabel.Text = $"Selected: {items[selectedValue]}";
        }
    }
}
