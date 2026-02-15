using System.Collections.ObjectModel;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace ViewShowcaseApp.Views;

public partial class ListViewDemo : View
{
    public ListViewDemo()
    {
        InitializeComponent();

        DemoList.Source = new ListWrapper<string>(new ObservableCollection<string>(fruits));
    }

    private static readonly string[] fruits =
    [
        "Apple",
        "Banana",
        "Cherry",
        "Date",
        "Elderberry",
        "Fig",
        "Grape",
        "Honeydew"
    ];

    private void OnValueChanged(object? sender, ValueChangedEventArgs<int?> e)
    {
        int selectedValue = e.NewValue ?? 0;
        if (selectedValue >= 0 && selectedValue < fruits.Length)
        {
            SelectionLabel.Text = $"Selected: {fruits[selectedValue]}";
        }
    }
}
