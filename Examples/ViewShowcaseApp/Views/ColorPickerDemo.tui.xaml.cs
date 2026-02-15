using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;

namespace ViewShowcaseApp.Views;

public partial class ColorPickerDemo : View
{
    public ColorPickerDemo()
    {
        InitializeComponent();
    }

    private void OnFullValueChanged(object? sender, ValueChangedEventArgs<Color?> e)
    {
        var color = FullColorPicker.SelectedColor;
        FullColorLabel.Text = $"Selected Color: RGB({color.R}, {color.G}, {color.B})";
    }
}
