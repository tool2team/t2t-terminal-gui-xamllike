using Terminal.Gui.App;
using Terminal.Gui.ViewBase;

namespace ViewShowcaseApp.Views;

public partial class NumericUpDownDemo : View
{
    private const decimal PricePerUnit = 19.99m;

    public NumericUpDownDemo()
    {
        InitializeComponent();

        // Configure quantity selector
        // TODO QuantitySelector.Minimum = 0;
        // TODO QuantitySelector.Maximum = 100;
        QuantitySelector.Value = 1;
        QuantitySelector.Increment = 1;
        UpdateTotal();

        // Configure temperature selector
        // TODO TemperatureSelector.Minimum = -50;
        // TODO TemperatureSelector.Maximum = 50;
        TemperatureSelector.Value = 20;
        TemperatureSelector.Increment = 1;
        UpdateTemperature();
    }

    private void OnQuantityChanged(object? sender, ValueChangedEventArgs<int> e)
    {
        UpdateTotal();
    }

    private void UpdateTotal()
    {
        var quantity = QuantitySelector.Value;
        var total = quantity * PricePerUnit;
        TotalLabel.Text = $"Total: ${total:F2} ({quantity} × ${PricePerUnit:F2})";
    }

    private void OnTemperatureChanged(object? sender, ValueChangedEventArgs<int> e)
    {
        UpdateTemperature();
    }

    private void UpdateTemperature()
    {
        var celsius = TemperatureSelector.Value;
        var fahrenheit = (celsius * 9 / 5) + 32;
        TemperatureLabel.Text = $"Temperature: {celsius}°C ({fahrenheit}°F)";
    }
}
