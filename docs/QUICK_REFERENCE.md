# Quick Reference - Terminal.Gui.XamlLike

Quick reference guide for XAML-like syntax.

## 🎯 Basic Structure

```xml
<Window x:Class="Namespace.ClassName"
        x:Name="Root"
        x:DataType="ViewModel"
        Title="Title"
        Width="Dim.Fill()"
        Height="Dim.Fill()">
  <!-- Controls -->
</Window>
```

## 📋 Essential Attributes

| Attribute | Required | Description | Example |
|----------|-------------|-------------|---------|
| `x:Class` | ✅ | Full C# class name | `"MyApp.Views.MainView"` |
| `x:Name` | ❌ | Generated field name | `"Root"`, `"BtnSave"` |
| `x:DataType` | ❌ | Default binding context | `"ViewModel"` |

## 🎨 Controls

### Containers

```xml
<Window Title="..." />
<FrameView Title="..." />
<ScrollView />
```

### Display

```xml
<Label Text="..." />
```

### Input

```xml
<TextField Text="..." Width="20" />
<TextView Text="..." Width="40" Height="10" />
<CheckBox Text="..." Checked="true" />
```

### Actions

```xml
<Button Text="..." Accepting="OnClick" />
```

### Lists

```xml
<ListView />
```

## 📐 Positioning

```xml
<!-- Fixed values -->
<Label X="10" Y="5" Width="20" Height="1" />

<!-- Terminal.Gui expressions -->
<Label X="Pos.Center()" Y="Pos.Bottom(otherView) + 1" />
<TextField Width="Dim.Fill(2)" Height="Dim.Percent(50)" />
```

## 🔗 Data Binding

### Syntax with x:DataType (Recommended)

```xml
<Window x:DataType="ViewModel">
  <!-- OneWay (default) -->
  <Label Text="{Bind Status}" />
  <Button Enabled="{Bind CanSave}" />
  
  <!-- TwoWay -->
  <TextField Text="{Bind UserName, Mode=TwoWay}" />
  <CheckBox Checked="{Bind IsEnabled, Mode=TwoWay}" />
</Window>
```

### Explicit Syntax (Without x:DataType)

```xml
<Label Text="{Bind ViewModel.Status}" />
<TextField Text="{Bind ViewModel.UserName, Mode=TwoWay}" />
```

### Nested Properties

```xml
<Label Text="{Bind User.Name}" />
<Label Text="{Bind Config.Display.Title}" />
```

### Binding to Collections

```xml
<ListView Source="{Bind AvailableItems}" />
```

## ⚡ Events

```xml
<Button Accepting="OnSaveClicked" />
<TextField TextChanged="OnTextChanged" Accept="OnAccept" />
<CheckBox ValueChanged="OnValueChanged" />
<Window Loaded="OnLoaded" Closing="OnClosing" />
```

## 🏗️ ViewModel Template

```csharp
using System.ComponentModel;

public class MainViewModel : INotifyPropertyChanged
{
    private string _myProperty = "";
    
    public string MyProperty
    {
        get => _myProperty;
        set
        {
            if (_myProperty != value)
            {
                _myProperty = value;
                OnPropertyChanged(nameof(MyProperty));
            }
        }
    }
    
    // Computed property
    public string ComputedProperty => $"Computed: {MyProperty}";
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

## 🏗️ View Template

```csharp
using Terminal.Gui;

namespace MyApp.Views
{
    public partial class MainView : Window
    {
        public MainViewModel ViewModel { get; }

        public MainView()
        {
            ViewModel = new MainViewModel();
            InitializeComponent(); // Auto-generated
        }

        // Event handlers
        private void OnSaveClicked(object? sender, EventArgs e)
        {
            // Logic here
        }
    }
}
```

## 🎯 Binding Modes

| Mode | Symbol | Description | Usage |
|------|---------|-------------|-------|
| `OneWay` | → | ViewModel → UI | Display (default) |
| `TwoWay` | ↔️ | ViewModel ↔️ UI | User input |

## 📦 Properties Supporting TwoWay

| Control | Property | Event |
|----------|-----------|-----------|
| `TextField` | `Text` | `TextChanged` |
| `TextView` | `Text` | `TextChanged` |
| `CheckBox` | `Checked` | `ValueChanged` |

## 💡 Complete Examples

### Simple (Without ViewModel)

```xml
<Window x:Class="SimpleView" Title="Simple">
  <Label Text="Hello World" />
  <Button Text="Click" Accepting="OnClick" />
</Window>
```

```csharp
public partial class SimpleView : Window
{
    public SimpleView() => InitializeComponent();

    private void OnClick(object? sender, EventArgs e)
    {
        // Logic here
    }
}
```

### MVVM (With ViewModel)

```xml
<Window x:Class="MvvmView" x:DataType="ViewModel">
  <Label Text="{Bind Message}" />
  <TextField Text="{Bind Input, Mode=TwoWay}" />
  <Button Text="Save" Accepting="OnSave" Enabled="{Bind CanSave}" />
</Window>
```

```csharp
public partial class MvvmView : Window
{
    public MainViewModel ViewModel { get; }
    
    public MvvmView()
    {
        ViewModel = new MainViewModel();
        InitializeComponent();
    }
    
    private void OnSave(object? sender, EventArgs e)
    {
        ViewModel.Save();
    }
}
```

## 🚫 Common Pitfalls

### ❌ Forgetting INotifyPropertyChanged

```csharp
// ❌ BAD - No UI update
public string Status { get; set; }

// ✅ GOOD
public string Status
{
    get => _status;
    set
    {
        _status = value;
        OnPropertyChanged(nameof(Status));
    }
}
```

### ❌ Initializing ViewModel After InitializeComponent

```csharp
// ❌ BAD - Exception thrown
public MainView()
{
    InitializeComponent();
    ViewModel = new MainViewModel();
}

// ✅ GOOD
public MainView()
{
    ViewModel = new MainViewModel();
    InitializeComponent();
}
```

### ❌ Not Notifying Computed Properties

```csharp
public string FirstName
{
    set
    {
        _firstName = value;
        OnPropertyChanged(nameof(FirstName));
        // ❌ FORGOT: OnPropertyChanged(nameof(FullName));
    }
}

public string FullName => $"{FirstName} {LastName}";
```

## 📚 Complete Documentation

- [XAML Format](docs/format.md)
- [MVVM Guide](docs/mvvm-guide.md)
- [Binding Implementation](BINDING_IMPLEMENTATION.md)

## 🆘 Quick Help

```bash
# Build
dotnet build

# Run examples
dotnet run --project samples/SimpleApp
dotnet run --project samples/MvvmApp
dotnet run --project samples/CommunityMvvmApp

# View generated files
# Look in obj/Debug/net8.0/Terminal.Gui.XamlLike/
```

## 🔗 Useful Links

- [Terminal.Gui Documentation](https://gui-cs.github.io/Terminal.Gui/)
- [INotifyPropertyChanged (Microsoft)](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged)
- [MVVM Pattern (Microsoft)](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
