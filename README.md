# Terminal.Gui.XamlLike

A source generator to create Terminal.Gui interfaces with XAML-like syntax.

## 🚀 Features

- ✅ **Familiar XAML-like syntax** for .NET developers
- ✅ **Full MVVM support** with data binding
- ✅ **Two-way binding** (TwoWay) for user input
- ✅ **Computed properties** with automatic updates
- ✅ **`x:DataType` attribute** for simplified syntax
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
          Clicked="OnSaveClicked" />

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

- [📖 Quick Reference](docs/QUICK_REFERENCE.md) - Quick syntax guide and cheat sheet
- [🔧 Binding Implementation](docs/BINDING_IMPLEMENTATION.md) - Technical details
- [📝 Changelog](docs/CHANGELOG.md) - Version history and changes

## 🎨 Examples

The repository contains three examples:

### SimpleApp - No ViewModel
Simple application without binding, manual UI management.

```bash
cd samples/SimpleApp
dotnet run
```

### MvvmApp - Custom MVVM
MVVM application with custom ViewModels.

```bash
cd samples/MvvmApp
dotnet run
```

### CommunityMvvmApp - CommunityToolkit.Mvvm
Application using CommunityToolkit.Mvvm with source generators.

```bash
cd samples/CommunityMvvmApp
dotnet run
```

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
- `Label` - Text label
- `Button` - Button
- `TextField` - Text input field
- `TextView` - Multiline text area
- `CheckBox` - Checkbox
- `OptionSelector` - Option selector
- `FrameView` - Container with border
- And more...


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

