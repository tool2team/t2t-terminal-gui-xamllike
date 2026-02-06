# Terminal.Gui.XamlLike - XSD Schema

This directory contains XSD schema files that provide **IntelliSense** support in Visual Studio for `.tui.xaml` files.

## 📦 Files

- **`tui-xaml.xsd`** - Main schema defining Terminal.Gui controls
- **`xaml-2006.xsd`** - XAML namespace schema (x:Name, x:Class, x:DataType, x:Type)

## 🚀 How to Use

### Option 1: Automatic Schema Association (Recommended)

Add the schema reference in your `.tui.xaml` files:

```xml
<Window xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xsi:schemaLocation="http://schemas.gui-cs.github.io/tui/2026/xaml 
                            ../../Terminal.Gui.XamlLike/Schema/tui-xaml.xsd"
        xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
        x:Class="MyApp.Views.MainView"
        x:Name="Root"
        x:DataType="MyApp.ViewModels.MainViewModel">
  <!-- Your controls here -->
</Window>
```

### Option 2: Visual Studio XML Schema Cache

1. Copy `tui-xaml.xsd` and `xaml-2006.xsd` to Visual Studio's XML Schema cache:
   ```
   %LOCALAPPDATA%\Microsoft\VisualStudio\<version>\Xml\Schemas\
   ```

2. Or configure in **Tools → Options → Text Editor → XML → Miscellaneous**

### Option 3: Per-Project Configuration

Add to your `.csproj`:

```xml
<ItemGroup>
  <None Include="Terminal.Gui.XamlLike\Schema\*.xsd">
    <SubType>Designer</SubType>
  </None>
</ItemGroup>
```

## ✨ Features Provided by XSD

- ✅ **Auto-completion** for control names (`<TextField>`, `<Button>`, etc.)
- ✅ **Auto-completion** for attributes (`Text`, `Width`, `X`, `Y`, etc.)
- ✅ **Tooltips** with documentation for each control and attribute
- ✅ **Validation** of element structure
- ❌ **Not provided:** Go to Definition (requires VSIX extension)

## 🎯 Supported Controls

The schema includes IntelliSense for:

- `Window` - Main window container
- `Label` - Text display
- `Button` - Clickable button
- `TextField` - Single-line text input
- `TextView` - Multi-line text area
- `CheckBox` - Checkbox control
- `OptionSelector` - Generic option selector
- `ListView` - List view
- `FrameView` - Container with border

## 📝 Common Attributes

All controls support:

- `X`, `Y` - Position (e.g., `"2"`, `"Pos.Center()"`)
- `Width`, `Height` - Dimensions (e.g., `"20"`, `"Dim.Fill()"`)
- `Enabled`, `Visible` - State properties
- `x:Name` - Control identifier for code-behind
- `x:Type` - Generic type parameter

## 🔗 Binding Syntax

The schema recognizes binding expressions:

```xml
<!-- OneWay binding -->
<Label Text="{Bind Status}" />

<!-- TwoWay binding -->
<TextField Text="{Bind UserName, Mode=TwoWay}" />
```

## 🛠️ Updating the Schema

When adding new controls to the generator:

1. Update `tui-xaml.xsd` with the new element definition
2. Add appropriate attributes and documentation
3. Test in Visual Studio by opening a `.tui.xaml` file

## 📚 References

- [XML Schema (XSD) Documentation](https://www.w3.org/TR/xmlschema-0/)
- [Visual Studio XML IntelliSense](https://learn.microsoft.com/en-us/visualstudio/xml-tools/)
