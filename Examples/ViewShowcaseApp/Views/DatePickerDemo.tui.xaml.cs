using Terminal.Gui.App;
using Terminal.Gui.ViewBase;

namespace ViewShowcaseApp.Views;

public partial class DatePickerDemo : View
{
    public DatePickerDemo()
    {
        InitializeComponent();
        
        // Set initial date to today
        DemoDatePicker.Value = DateTime.Today;
        UpdateSelectedDateLabel();
    }

    private void OnValueChanged(object? sender, ValueChangedEventArgs<DateTime> e)
    {
        UpdateSelectedDateLabel();
    }

    private void UpdateSelectedDateLabel()
    {
        SelectedDateLabel.Text = $"Selected Date: {DemoDatePicker.Value:dddd, MMMM dd, yyyy}";
    }
}
