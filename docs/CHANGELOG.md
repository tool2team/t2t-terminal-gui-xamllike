# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased] - 2024-12-XX

### ✨ Added

- **Complete MVVM support** with data binding
  - `x:DataType` attribute to simplify bindings
  - `OneWay` binding support (default)
  - `TwoWay` binding support for `TextField`, `TextView`, `CheckBox`
  - Auto-detection of binding source (ViewModel or Self)

- **Automatic binding code generation**
  - `SetupBindings()` method generated automatically
  - `PropertyChanged` handling for updates
  - Computed properties support

- **Array properties support**
  - `RadioGroup.RadioLabels` can be bound to `string[]`
  - Automatic detection of array type properties

- **Numeric properties**
  - `RadioGroup.SelectedItem` supports `int` values
  - No quotes around numeric values

- **Complete documentation**
  - Complete XAML format guide (`docs/format.md`)
  - Detailed MVVM guide (`docs/mvvm-guide.md`)
  - Technical implementation documentation (`BINDING_IMPLEMENTATION.md`)
  - README with quick start

### 🔧 Fixed

- **Event duplication**: Events are no longer generated as properties AND events
  - Before: `TxtUserName.TextChanged = "OnUserNameChanged"; TxtUserName.TextChanged += OnUserNameChanged;`
  - After: `TxtUserName.TextChanged += OnUserNameChanged;`

- **Uninitialized Root field**: The `Root` field (root element with `x:Name`) is now initialized
  - Added `Root = this;` in `InitializeComponent()`

- **Non-existent SetNeedsDisplay()**: Removed call to this method that doesn't exist in Terminal.Gui v2

- **Duplicate binding errors**: Binding `switch case` statements no longer generate duplicates
  - Smart grouping by property name

- **Type conversion for RadioGroup**
  - `RadioLabels` now accepts `string[]` via binding
  - `SelectedItem` accepts `int` values without conversion

### 🎨 Improved

- **Simplified binding syntax**
  - Before: `{Bind ViewModel.Status}`
  - After: `{Bind Status}` (with `x:DataType="ViewModel"`)

- **Property mappings**
  - Added `IsNumericProperty()` for `int` properties
  - Added `IsArrayProperty()` for `string[]` properties
  - Specific mappings for `RadioGroup`

### 📚 Examples

- **SimpleApp**: Demo without ViewModel (static values + events)
- **MvvmApp**: MVVM demo with custom ViewModels
- **CommunityMvvmApp**: Demo with CommunityToolkit.Mvvm

All examples now use:
- ✅ `x:DataType` for simplified syntax
- ✅ Bindings to ViewModel properties
- ✅ `TwoWay` binding for user input

## [0.1.0] - Initial (Date unknown)

### Added

- Basic XAML-like source generator
- Support for basic Terminal.Gui controls
- XML parsing and C# code generation
- Event support
- Basic XAML validation

---

**Format based on** [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
