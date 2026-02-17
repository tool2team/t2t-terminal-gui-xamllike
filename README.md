# Terminal.Gui.XamlLike

A C# source generator that brings **XAML-like syntax** to [Terminal.Gui v2](https://github.com/gui-cs/Terminal.Gui), enabling rapid development of terminal-based applications with familiar markup and full MVVM support.

```xml
<Dialog Title="Hello Terminal!" Width="40" Height="10">
  <Label Text="{Bind WelcomeMessage}" X="Pos.Center()" Y="2" />
  <Button Text="OK" IsDialogButton="true" IsDefault="true" />
</Dialog>
```

**Write declarative UI, get type-safe code at compile time.** 🚀

## 🚀 Features

- ✅ **Familiar XAML-like syntax** for .NET developers
- ✅ **Full MVVM support** with data binding (OneWay, TwoWay)
- ✅ **Dialog management** with IsDialogButton property
- ✅ **Computed properties** with automatic updates
- ✅ **`x:DataType` attribute** for simplified syntax (like MAUI)
- ✅ **50+ Terminal.Gui controls** supported
- ✅ **Code generation** at compile time via Source Generators
- ✅ **IntelliSense support** for `.tui.xaml` files via XSD schema
- ✅ **Compatible with Terminal.Gui v2**

## 📦 Installation

*Coming soon: NuGet Package*

For now, clone the repository and add a project reference:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\Terminal.Gui.XamlLike\Terminal.Gui.XamlLike.csproj" 
                    OutputItemType="Analyzer" 
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. Create a `.tui.xaml` file

```xml
<Window x:Class="MyApp.Views.MainView"
        x:Name="Root"
        x:DataType="MyApp.ViewModels.MainViewModel"
        Title="My Application"
        Width="Dim.Fill()"
        Height="Dim.Fill()">

  <Label Text="{Bind WelcomeMessage}" />

  <TextField Text="{Bind UserName, Mode=TwoWay}" 
             TextChanged="OnUserNameChanged" />

  <Button Text="Save" 
          Accepting="OnSaveClicked" />

</Window>
```

### 2. Create the ViewModel

```csharp
using System.ComponentModel;

namespace MyApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _userName = "";

        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WelcomeMessage)));
                }
            }
        }

        public string WelcomeMessage => 
            string.IsNullOrEmpty(UserName) ? "Hello!" : $"Hello, {UserName}!";

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
```

### 3. Create the code-behind

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

        private void OnUserNameChanged(object? sender, EventArgs e) { }

        private void OnSaveClicked(object? sender, EventArgs e)
        {
            // Save logic
        }
    }
}
```

### 4. Build and Run

The code is automatically generated at compile time. The generated file (`MainView.tui.xaml.g.cs`) contains:
- Fields for named controls
- The `InitializeComponent()` method
- Automatic binding code

## 📚 Documentation

### Quick Start
- **[Quick Reference](docs/QUICK_REFERENCE.md)** - Fast reference for XAML syntax, controls, and patterns
- **[Documentation Index](docs/README.md)** - Complete documentation overview

### Features
- **[Dialog Buttons](docs/DIALOG_BUTTONS.md)** - Complete guide for Dialog management
  - IsDialogButton property and design rationale
  - Auto-positioned vs manually positioned buttons
  - Migration guide for upgrading
- **[Binding Implementation](docs/BINDING_IMPLEMENTATION.md)** - Technical details of binding system
- **[Changelog](docs/CHANGELOG.md)** - Version history and changes

### Examples
Check the `Examples/` folder for working demos:
- **SimpleApp** - Basic usage without ViewModel
- **MvvmApp** - Full MVVM pattern
- **ViewShowcaseApp** - All controls and features showcase

## 🔑 Key Concepts

### x:DataType Attribute

Specifies the ViewModel type for simplified bindings (like MAUI):

```xml
<!-- With x:DataType (RECOMMENDED - like MAUI) -->
<Window x:DataType="MyApp.ViewModels.MainViewModel">
  <!-- {Bind Status} resolves to ViewModel.Status -->
  <Label Text="{Bind Status}" />
</Window>

<!-- Without x:DataType - explicit path -->
<Window>
  <Label Text="{Bind ViewModel.Status}" />
</Window>
```

The generator automatically finds the property with the specified type in your view class.

### Binding Modes

| Mode | Description |
|------|-------------|
| `OneWay` (default) | Data flows from ViewModel to UI only |
| `TwoWay` | Data flows in both directions |

```xml
<!-- Read-only -->
<Label Text="{Bind Status}" />

<!-- Two-way -->
<TextField Text="{Bind UserName, Mode=TwoWay}" />
```

### Supported Controls

- `Window` - Main window
- `Dialog` - Modal dialog window with button management
- `Label` - Text label
- `Button` - Button (with IsDialogButton support for dialogs)
- `TextField` - Text input field
- `TextView` - Multiline text area
- `CheckBox` - Checkbox
- `RadioGroup` - Radio button group
- `OptionSelector` - Option selector
- `ListView` - List view
- `FrameView` - Container with border
- `MenuBar` - Menu bar with items
- `TabView` - Tab container
- And 50+ more Terminal.Gui controls...

See [Quick Reference](docs/QUICK_REFERENCE.md) for complete list.

### Dialog Management

Create dialogs with auto-positioned or manually positioned buttons:

```xml
<Dialog Title="Confirm Action" Width="40" Height="10">
  <Label Text="Are you sure?" X="Pos.Center()" Y="2" />

  <!-- Auto-positioned dialog buttons -->
  <Button Text="Yes" IsDialogButton="true" IsDefault="true" Accepting="OnYes" />
  <Button Text="No" IsDialogButton="true" Accepting="OnNo" />
</Dialog>
```

See [Dialog Buttons Guide](docs/DIALOG_BUTTONS.md) for complete documentation.


## 🛠️ Development

### Prerequisites

- .NET 8.0 SDK or higher
- Terminal.Gui v2 (included via NuGet)

### Build the project

```bash
dotnet build
```

### Run tests

```bash
dotnet test
```

## 🤝 Contributing

Contributions are welcome! Feel free to:

1. Fork the project
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 Current Limitations

- No reusable styles
- No data templates
- No resource system
- No value converters for bindings

## 🗺️ Roadmap

- [ ] XAML styles support
- [ ] Data templates
- [ ] Resource system
- [ ] Value converters
- [ ] Support for more Terminal.Gui controls

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [Terminal.Gui](https://github.com/gui-cs/Terminal.Gui) - TUI framework for .NET
- .NET MAUI/Xamarin.Forms community for MVVM pattern inspiration

---

**Note**: This project is under active development. The API may change.

