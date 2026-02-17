# Dialog Buttons - Complete Guide

## Overview

Buttons in a `Dialog` can be added in two different ways:

1. **Dialog buttons** (`AddButton()`) - Automatically positioned at the bottom of the dialog
2. **Regular buttons** (`Add()`) - Manually positioned like normal controls

This guide covers the `IsDialogButton` property, its design rationale, usage patterns, and migration scenarios.

---

## Table of Contents

- [The IsDialogButton Property](#the-isdialogbutton-property)
- [Design Decision](#design-decision)
- [Default Behavior](#default-behavior)
- [When to Use IsDialogButton](#when-to-use-isdialogbutton)
- [Dialog Properties](#dialog-properties)
- [Complete Examples](#complete-examples)
- [Use Cases](#use-cases)
- [Best Practices](#best-practices)
- [Migration Guide](#migration-guide)
- [API Reference](#api-reference)

---

## The IsDialogButton Property

The `IsDialogButton` property controls how a `Button` is added to its parent `Dialog`:

- **`IsDialogButton="false"`** (default): Button is added via `Add()` - manual positioning
- **`IsDialogButton="true"`**: Button is added via `AddButton()` - automatic positioning

### Why This Design?

**Default: `false` (explicit over magical)**

The default value of `false` was chosen to provide:

1. ✅ **Explicit behavior** - No hidden "magic" based on parent context
2. ✅ **Consistency** - Same as all other controls (use `Add()` by default)
3. ✅ **Predictability** - Developers know exactly what will happen
4. ✅ **Less verbosity** - Common case (manual positioning) requires no extra attribute
5. ✅ **No surprises** - Buttons in nested containers work as expected

---

## Design Decision

### 🔄 Evolution of the API

#### ❌ BEFORE (problematic - magical behavior)

Early versions had implicit behavior:

```xml
<Dialog Title="Confirm">
  <!-- These buttons were AUTOMATICALLY dialog buttons -->
  <Button Text="OK" />      <!-- Automatic AddButton() -->
  <Button Text="Cancel" />  <!-- Automatic AddButton() -->
</Dialog>
```

**Problems:**
- 🚫 Magical behavior based on parent
- 🚫 Not explicit
- 🚫 Confusing for manually positioned buttons
- 🚫 Inconsistent with other controls

#### ✅ AFTER (current - explicit behavior)

```xml
<Dialog Title="Confirm">
  <!-- For auto-positioned buttons, mark explicitly -->
  <Button Text="OK" IsDialogButton="true" />      <!-- AddButton() -->
  <Button Text="Cancel" IsDialogButton="true" />  <!-- AddButton() -->
</Dialog>

<!-- OR for manual positioning (default behavior) -->
<Dialog Title="Custom">
  <Button Text="OK" 
          X="Pos.Center() - 5" 
          Y="Pos.Bottom() - 3" />  <!-- Add() -->
</Dialog>
```

**Benefits:**
- ✅ Explicit and predictable
- ✅ Consistent with all other controls
- ✅ No special default behavior
- ✅ Fewer surprises
- ✅ Clear intent when reading code

---

## Default Behavior

### Manual Positioning (Default)

**By default, all Buttons use `Add()` (standard behavior)**:

```xml
<Dialog Title="Custom Dialog" Width="50" Height="15">
  
  <!-- These buttons are manually positioned (default behavior) -->
  <Button Text="OK" 
          X="Pos.Center() - 5" 
          Y="Pos.Bottom() - 3"
          Accepting="OnOk" />
          
  <Button Text="Cancel"
          X="Pos.Center() + 5"
          Y="Pos.Bottom() - 3"
          Accepting="OnCancel" />
          
</Dialog>
```

**Generated code:**
```csharp
var okButton = new Terminal.Gui.Views.Button();
okButton.Text = "OK";
okButton.X = Pos.Center() - 5;
okButton.Y = Pos.Bottom() - 3;
okButton.Accepting += OnOk;
this.Add(okButton);  // ← Uses Add() by default

var cancelButton = new Terminal.Gui.Views.Button();
cancelButton.Text = "Cancel";
cancelButton.X = Pos.Center() + 5;
cancelButton.Y = Pos.Bottom() - 3;
cancelButton.Accepting += OnCancel;
this.Add(cancelButton);  // ← Uses Add() by default
```

### Explicit Use for Dialog Buttons

To add buttons to `Dialog.Buttons` (automatically positioned), use `IsDialogButton="true"`:

```xml
<Dialog Title="Confirm" Width="40" Height="10">
  
  <Label Text="Are you sure?" X="Pos.Center()" Y="2" />
  
  <!-- Dialog buttons (auto-positioned at bottom) -->
  <Button Text="OK" 
          IsDialogButton="true"
          IsDefault="true" 
          Accepting="OnOk" />
          
  <Button Text="Cancel"
          IsDialogButton="true"
          Accepting="OnCancel" />
          
</Dialog>
```

**Generated code:**
```csharp
var label = new Terminal.Gui.Views.Label();
label.Text = "Are you sure?";
label.X = Pos.Center();
label.Y = 2;
this.Add(label);

var okButton = new Terminal.Gui.Views.Button();
okButton.Text = "OK";
okButton.IsDefault = true;
okButton.Accepting += OnOk;
this.AddButton(okButton);  // ← Uses AddButton() because IsDialogButton="true"

var cancelButton = new Terminal.Gui.Views.Button();
cancelButton.Text = "Cancel";
cancelButton.Accepting += OnCancel;
this.AddButton(cancelButton);  // ← Uses AddButton() because IsDialogButton="true"
```

### Mixed Buttons

You can combine both approaches:

```xml
<Dialog Title="Custom Dialog" Width="60" Height="20">
  
  <!-- Custom button manually positioned in content -->
  <FrameView Title="Actions" X="2" Y="2" Width="Dim.Fill(2)" Height="5">
    <Button Text="Custom Action" 
            X="2" 
            Y="1"
            Accepting="OnCustomAction" />
  </FrameView>
  
  <!-- Dialog buttons at bottom (auto-positioned) -->
  <Button Text="OK" 
          IsDialogButton="true"
          IsDefault="true" 
          Accepting="OnOk" />
          
  <Button Text="Cancel"
          IsDialogButton="true"
          Accepting="OnCancel" />
  
</Dialog>
```

**Generated code:**
```csharp
// FrameView with custom button
var frameView = new Terminal.Gui.Views.FrameView();
frameView.Title = "Actions";
// ...

var customButton = new Terminal.Gui.Views.Button();
customButton.Text = "Custom Action";
customButton.X = 2;
customButton.Y = 1;
customButton.Accepting += OnCustomAction;
frameView.Add(customButton);  // ← Uses Add() (default behavior)

this.Add(frameView);

// Dialog buttons
var okButton = new Terminal.Gui.Views.Button();
okButton.Text = "OK";
okButton.IsDefault = true;
okButton.Accepting += OnOk;
this.AddButton(okButton);  // ← Uses AddButton() because IsDialogButton="true"

var cancelButton = new Terminal.Gui.Views.Button();
cancelButton.Text = "Cancel";
cancelButton.Accepting += OnCancel;
this.AddButton(cancelButton);  // ← Uses AddButton() because IsDialogButton="true"
```

---

## When to Use IsDialogButton

### 🎯 Use `IsDialogButton="true"` when you want:

1. **Automatic positioning** at the bottom of the dialog
   - No need to calculate X/Y coordinates
   - Dialog handles layout automatically

2. **Automatic alignment** via `ButtonAlignment` and `ButtonAlignmentModes`
   - Center, Start, End, Fill, Justified
   - StartToEnd or EndToStart ordering

3. **Standard dialog buttons**
   - OK, Cancel, Yes, No, Apply, Close, etc.
   - Common pattern across applications

4. **Keyboard navigation**
   - Automatic Tab order
   - Esc to cancel (if configured)
   - Enter for default button

### 🎨 Use default behavior (manual positioning) when you want:

1. **Custom layouts**
   - Buttons in specific positions
   - Non-standard arrangements

2. **Buttons in content area**
   - Inside FrameView or other containers
   - Part of the dialog content, not actions

3. **Action buttons with icons or special styling**
   - Browse buttons
   - Tool buttons
   - Context-specific actions

### 📊 Decision Flow

```
Is this a standard dialog action (OK/Cancel/Yes/No)?
├─ YES → Use IsDialogButton="true"
│         (Auto-positioned, aligned, keyboard support)
│
└─ NO → Use default (manual positioning)
          (Custom layout, full control)
```

---

## Use Cases

### ✅ Dialog Buttons (AddButton with IsDialogButton="true")

Use `IsDialogButton="true"` for:
- OK/Cancel/Yes/No/Retry buttons
- Main dialog actions
- Buttons that should be automatically aligned at bottom

**Advantages:**
- Automatic positioning
- Alignment via `ButtonAlignment` and `ButtonAlignmentModes`
- Automatic keyboard handling (Tab, Esc)
- Support for `IsDefault` for the default button
- No need to specify X/Y

### ✅ Regular Buttons (Add - default behavior)

Use default behavior for:
- Buttons in dialog content (inside a FrameView, etc.)
- Secondary action buttons with manual positioning
- Buttons that need specific positioning (X, Y)

**Advantages:**
- Full control over positioning (X, Y)
- Can be in any child container
- Consistent behavior with all other controls
- No special behavior

## Dialog Properties

### `ButtonAlignment`

Controls the horizontal alignment of dialog buttons:

```xml
<Dialog Title="Aligned Buttons" 
        Width="50" 
        Height="12"
        ButtonAlignment="Center">
  
  <Button Text="OK" IsDialogButton="true" />
  <Button Text="Cancel" IsDialogButton="true" />
  
</Dialog>
```

**Possible values:**
- `Start` - Left-aligned
- `Center` - Centered (default)
- `End` - Right-aligned
- `Fill` - Stretched to fill width
- `Justified` - Evenly spaced

### `ButtonAlignmentModes`

Controls the order of buttons:

```xml
<Dialog ButtonAlignmentModes="EndToStart">
  <Button Text="OK" IsDialogButton="true" />      <!-- Will be on right -->
  <Button Text="Cancel" IsDialogButton="true" />  <!-- Will be on left -->
</Dialog>
```

**Values:**
- `StartToEnd` - Normal order (left → right)
- `EndToStart` - Reversed order (right → left)

## Complete Examples

### Simple Dialog with Auto-Positioned Buttons

```xml
<Dialog x:Class="MyApp.ConfirmDialog"
        xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Confirmation Required"
        Width="50"
        Height="10">

  <Label Text="Are you sure you want to proceed?"
         X="Pos.Center()"
         Y="2" />

  <!-- Dialog buttons automatically aligned at bottom -->
  <Button Text="Yes" 
          IsDialogButton="true"
          IsDefault="true" 
          Accepting="OnYes" />
          
  <Button Text="No"
          IsDialogButton="true"
          Accepting="OnNo" />
  
</Dialog>
```

### Dialog with Manually Positioned Buttons

```xml
<Dialog x:Class="MyApp.CustomLayoutDialog"
        xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Custom Layout"
        Width="50"
        Height="15">

  <Label Text="Make your choice:"
         X="Pos.Center()"
         Y="2" />

  <!-- Manually positioned buttons (default behavior) -->
  <Button Text="OK"
          X="Pos.Center() - 10"
          Y="Pos.Bottom() - 3"
          IsDefault="true"
          Accepting="OnOk" />
          
  <Button Text="Cancel"
          X="Pos.Center() + 2"
          Y="Pos.Bottom() - 3"
          Accepting="OnCancel" />
  
</Dialog>
```

### Complex Dialog with Mixed Buttons

```xml
<Dialog x:Class="MyApp.AdvancedDialog"
        xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Advanced Options"
        Width="60"
        Height="20"
        ButtonAlignment="End">

  <!-- Main content with integrated buttons -->
  <FrameView Title="File Operations" 
             X="2" 
             Y="2" 
             Width="Dim.Fill(2)" 
             Height="8">
    
    <Label Text="Selected file: example.txt" X="2" Y="1" />
    
    <!-- Custom button in content (default Add behavior) -->
    <Button Text="Browse..." 
            X="2" 
            Y="3"
            Accepting="OnBrowse" />
    
  </FrameView>

  <!-- Standard dialog buttons at bottom (auto-positioned) -->
  <Button Text="Apply"
          IsDialogButton="true"
          IsDefault="true" 
          Accepting="OnApply" />
          
  <Button Text="Cancel"
          IsDialogButton="true"
          Accepting="OnCancel" />
  
</Dialog>
```

### Corresponding Code-Behind

```csharp
namespace MyApp;

public partial class AdvancedDialog : Terminal.Gui.Views.Dialog
{
    public AdvancedDialog()
    {
        InitializeComponent();
    }

    private void OnBrowse(object? sender, EventArgs e)
    {
        // Open FileDialog to select a file
        var fileDialog = new OpenDialog();
        Application.Run(fileDialog);
        
        if (!fileDialog.Canceled)
        {
            // Process selected file
        }
    }

    private void OnApply(object? sender, EventArgs e)
    {
        // Apply changes
        RequestStop();
    }

    private void OnCancel(object? sender, EventArgs e)
    {
        // Cancel and close
        RequestStop();
    }
}
```

## Best Practices

### ✅ DO

- Use `IsDialogButton="true"` for OK/Cancel/Yes/No buttons that should be auto-positioned
- Use `IsDefault="true"` on the main action button
- Leave default behavior (`Add`) for manually positioned buttons or buttons in content
- Don't specify X/Y on buttons with `IsDialogButton="true"`

### ❌ DON'T

- Don't put dialog buttons inside a FrameView or other container
- Don't specify X/Y on buttons marked `IsDialogButton="true"` (they are auto-positioned)
- Don't use `IsDialogButton="false"` explicitly (it's the default behavior)

---

## Migration Guide

### Migrating from Earlier Versions

If you're upgrading from a version with automatic `IsDialogButton="true"` behavior:

#### Scenario 1: You had manually positioned buttons

**Before** (had to explicitly disable):
```xml
<Dialog>
  <Button Text="OK" 
          X="10" 
          Y="5"
          IsDialogButton="false" />  <!-- Had to specify -->
</Dialog>
```

**After** (default behavior):
```xml
<Dialog>
  <Button Text="OK" 
          X="10" 
          Y="5" />  <!-- No longer needed, it's the default -->
</Dialog>
```

✅ **Action**: Remove `IsDialogButton="false"` - it's now the default

#### Scenario 2: You had auto-positioned dialog buttons

**Before** (implicit):
```xml
<Dialog>
  <Button Text="OK" />  <!-- Automatically AddButton() -->
</Dialog>
```

**After** (explicit):
```xml
<Dialog>
  <Button Text="OK" IsDialogButton="true" />  <!-- Now explicit -->
</Dialog>
```

⚠️ **Action**: Add `IsDialogButton="true"` to buttons that should be dialog buttons

#### Scenario 3: Mixed buttons

**Before**:
```xml
<Dialog>
  <FrameView>
    <Button Text="Browse" IsDialogButton="false" />
  </FrameView>
  <Button Text="OK" />  <!-- Implicit dialog button -->
  <Button Text="Cancel" />  <!-- Implicit dialog button -->
</Dialog>
```

**After**:
```xml
<Dialog>
  <FrameView>
    <Button Text="Browse" />  <!-- Default behavior, remove attribute -->
  </FrameView>
  <Button Text="OK" IsDialogButton="true" />  <!-- Explicit -->
  <Button Text="Cancel" IsDialogButton="true" />  <!-- Explicit -->
</Dialog>
```

✅ **Action**:
- Remove `IsDialogButton="false"` from content buttons
- Add `IsDialogButton="true"` to dialog buttons

### Quick Migration Steps

1. **Search** for all `<Button` elements in `<Dialog>` elements
2. **Identify** which buttons should be dialog buttons (typically OK/Cancel/Yes/No at bottom)
3. **Add** `IsDialogButton="true"` to those buttons
4. **Remove** any `IsDialogButton="false"` attributes (now default)
5. **Test** your dialogs to ensure correct behavior

### Migration Script Example

PowerShell script to help identify Dialogs that may need updates:

```powershell
# Find all .tui.xaml files with Dialog elements
Get-ChildItem -Recurse -Filter "*.tui.xaml" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match '<Dialog.*?>') {
        Write-Host "Found Dialog in: $($_.FullName)"

        # Check for Button elements without IsDialogButton
        if ($content -match '<Button[^>]*>' -and $content -notmatch 'IsDialogButton') {
            Write-Host "  → Contains Button elements that may need IsDialogButton attribute" -ForegroundColor Yellow
        }
    }
}
```

---

## API Reference

### Dialog Properties

| Property | Type | Description |
|----------|------|-------------|
| `ButtonAlignment` | `Alignment` | Horizontal alignment of buttons (Start, Center, End, Fill, Justified) |
| `ButtonAlignmentModes` | `AlignmentModes` | Alignment mode (StartToEnd, EndToStart) |
| `Buttons` | `List<Button>` | Dialog button collection (read-only) |

### Button Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsDefault` | `bool` | Default button (activated by Enter) |
| `IsDialogButton` | `bool` (meta) | If true, added via AddButton() (auto-positioned), otherwise via Add(). Default: false |

### Dialog Methods

| Method | Description |
|--------|-------------|
| `AddButton(Button)` | Adds a button to the Buttons collection (auto-positioned) |
| `Add(View)` | Adds a control to the dialog content |

## See Also

- [Terminal.Gui Dialog API](https://gui-cs.github.io/Terminal.Gui/api/Terminal.Gui.Views.Dialog.html)
- [Terminal.Gui Button API](https://gui-cs.github.io/Terminal.Gui/api/Terminal.Gui.Views.Button.html)
- [XAML Format Guide](format.md)
- [Quick Reference](QUICK_REFERENCE.md)
- [Binding Implementation](BINDING_IMPLEMENTATION.md)
