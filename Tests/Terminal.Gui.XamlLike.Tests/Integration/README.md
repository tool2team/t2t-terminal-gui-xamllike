# Terminal.Gui XamlLike Integration Tests

## Overview

This directory contains integration tests for all Terminal.Gui Views supported by the XAML-like generator.

## Structure

```
Integration/
├── Assets/              # XAML test files (*.tui.xaml)
├── Snapshots/           # Expected generated C# files (*.tui.xaml.g.cs)
└── Views/               # Test classes (*Tests.cs)
```

## Test Coverage

### Complete Coverage (54 Views)

All Terminal.Gui Views now have integration tests:

**ViewBase Namespace:**
- Adornment ✓
- Border ✓
- Margin ✓
- Padding ✓
- View ✓

**Views Namespace:**
- AttributePicker ✓
- Bar ✓
- Button ✓
- CharMap ✓
- CheckBox ✓
- ColorPicker ✓
- ColorPicker16 ✓
- ComboBox ✓
- DateField ✓
- DatePicker ✓
- Dialog ✓
- FileDialog ✓
- FlagSelector ✓
- FrameView ✓
- GraphView ✓
- HexView ✓
- Label ✓
- LegendAnnotation ✓
- Line ✓
- LinearRange ✓
- ListView ✓
- Menu ✓
- MenuBar ✓
- MenuBarItem ✓
- MenuItem ✓
- NumericUpDown ✓
- OpenDialog ✓
- OptionSelector ✓
- PopoverMenu ✓
- ProgressBar ✓
- Runnable ✓
- SaveDialog ✓
- ScrollBar ✓
- Shortcut ✓
- SpinnerView ✓
- StatusBar ✓
- Tab ✓
- TableView ✓
- TabView ✓
- TextField ✓
- TextValidateField ✓
- TextView ✓
- TimeField ✓
- TreeView ✓
- Window ✓
- Wizard ✓
- WizardStep ✓

## How Tests Work

Each test:
1. Loads a XAML file from `Assets/`
2. Runs the incremental source generator
3. Compares the generated C# code with a snapshot in `Snapshots/`

## Adding New Tests

To add a test for a new View:

1. Create a test class in `Views/`:
   ```csharp
   using Terminal.Gui.Views;
   
   namespace Terminal.Gui.XamlLike.Tests.Integration.Views;
   
   public class MyViewTests : BaseViewTests<MyView>
   {
   }
   ```

2. Create a XAML asset in `Assets/`:
   ```xml
   <MyView x:Class="Terminal.Gui.XamlLike.Tests.Integration.Views.MyViewTestView"
           xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
           xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
           Width="40"
           Height="10">
     
     <!-- Test content -->
     
   </MyView>
   ```

3. Run the test once to generate the snapshot

## Updating Snapshots

When you intentionally change the generator output:

1. Define `BUILD_SNAPSHOTS` in your project
2. Run the tests - they will update the snapshots
3. Review the changes in git diff
4. Remove the `BUILD_SNAPSHOTS` define
5. Run tests again to verify

## Notes

- Generic types (like `Dialog<T>`) are excluded from auto-generation
- Views in `Terminal.Gui.ViewBase` use that namespace in their using statement
- All other Views use `Terminal.Gui.Views`
