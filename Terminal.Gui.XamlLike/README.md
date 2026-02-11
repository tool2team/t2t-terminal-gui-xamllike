# Terminal.Gui.XamlLike

A **source generator** for creating Terminal.Gui v2 interfaces using XAML-like syntax.

## ✨ Key Features

- 🎨 **XAML-like syntax** for Terminal.Gui controls
- ⚙️ **Compile-time code generation** via Roslyn Source Generators
- 🔄 **Full MVVM support** with data binding (OneWay, TwoWay)
- 🧩 **IntelliSense support** via included XSD schemas
- 📦 **Zero runtime dependencies** - all code generated at build time

## 📦 Installation

```bash
dotnet add package T2t.Terminal.Gui.XamlLike
```

## 🚀 Quick Start

**1. Create a `.tui.xaml` file:**

```xml
<Window x:Class="MyApp.Views.MainWindow"
        x:DataType="MyApp.ViewModels.MainViewModel"
        Title="My App">

  <Label Text="{Bind WelcomeMessage}" />
  <TextField Text="{Bind UserName, Mode=TwoWay}" />
  <Button Text="Save" Command="{Bind SaveCommand}" />

</Window>
```

**2. Code is auto-generated** at compile time - includes `InitializeComponent()`, bindings, and named controls.

## 📚 Full Documentation

See the [main repository README](../../README.md) for:
- Complete syntax reference
- MVVM patterns and examples
- Supported controls and attributes
- Binding modes and advanced scenarios
