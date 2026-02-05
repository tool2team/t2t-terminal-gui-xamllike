# Flexible Binding Implementation

## Overview

The binding system now supports **three scenarios** with the **same `{Bind ...}` syntax**:

### 1. No binding (static values only)
```xaml
<Window x:Class="SimpleApp.MainView">
  <Label Text="Static text" />
  <Button Clicked="OnButtonClick" />
</Window>
```
✅ No binding code generated

### 2. Binding with x:DataType (RECOMMENDED - simplified syntax)
```xaml
<Window x:Class="MvvmApp.MainView"
        x:DataType="ViewModel">
  <!-- Simplified syntax - prefix added automatically -->
  <Label Text="{Bind Status}" />
  <TextField Text="{Bind UserName, TwoWay}" />
  <Button Enabled="{Bind IsEnabled}" />
</Window>
```
✅ `{Bind Status}` automatically becomes `ViewModel.Status`  
✅ **More readable and concise**  
✅ **All MvvmApp and CommunityMvvmApp examples use this syntax**

### 3. Binding with explicit prefix (backward compatibility)
```xaml
<Window x:Class="MvvmApp.MainView">
  <!-- Explicit syntax - still works -->
  <Label Text="{Bind ViewModel.Status}" />
  <TextField Text="{Bind ViewModel.UserName, TwoWay}" />
</Window>
```
✅ Automatic detection of "ViewModel" as source

### 4. Self-binding (view properties)
```xaml
<Window x:Class="CustomView">
  <!-- Binding to view properties -->
  <Label Text="{Bind Counter}" />
  <Label Text="{Bind WelcomeMessage}" />
</Window>
```
✅ View must implement `INotifyPropertyChanged`

## Technical Changes

### 1. Model.cs
- Added `DataType` property on `XamlDocument`
- Reads `x:DataType` attribute from root element

### 2. BindingModel.cs
- `BindingExpression.Parse()` now accepts optional `dataType` parameter
- If `dataType` is provided and path doesn't contain `.`, automatically adds prefix
- If path already contains `.` (e.g., `ViewModel.Prop`), uses it as-is

### 3. Generator.cs
- Passes `document.DataType` to binding collection and generation methods
- `CollectBindings()` and `CollectBindingsRecursive()` accept `dataType`
- `GenerateBindingMethods()` uses `dataType` to detect binding source
- Switch generation without duplicates: groups by final property name

### 4. XmlParser.cs
- `Validate()` and `ValidateElement()` pass `dataType` during parsing

## Advantages

✅ **Unified syntax**: Same `{Bind ...}` everywhere  
✅ **Flexible**: Works with/without ViewModel, with/without x:DataType  
✅ **Backward compatible**: Existing XAML still works  
✅ **Auto-detection**: Understands context automatically  
✅ **No Terminal.Gui modification**: Everything is in the generator

## Current Limitations

⚠️ **RadioGroup**: Conversion errors `string` → `string[]` and `string` → `int`  
   → Requires special mapping for `RadioLabels` and `SelectedItem` properties

## Generated Code Examples

### With ViewModel
```csharp
private void SetupBindings()
{
    if (ViewModel == null) throw new InvalidOperationException("ViewModel property must be set before calling InitializeComponent");
    
    LblStatus!.Text = ViewModel.Status;
    TxtUserName!.Text = ViewModel.UserName;
    
    ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    TxtUserName!.TextChanged += OnTxtUserNameTextChanged;
}

private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
{
    switch (e.PropertyName)
    {
        case "Status":
            LblStatus!.Text = ViewModel.Status;
            break;
        case "UserName":
            TxtUserName!.Text = ViewModel.UserName;
            break;
    }
}
```

### Self-binding
```csharp
private void SetupBindings()
{
    LblCounter!.Text = Counter;
    
    PropertyChanged += OnViewModelPropertyChanged;
}

private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
{
    switch (e.PropertyName)
    {
        case "Counter":
            LblCounter!.Text = Counter;
            break;
    }
}
```
