# ViewShowcaseApp

This application demonstrates all available Terminal.Gui views in an interactive showcase. It provides a comprehensive testing environment for exploring and experimenting with different controls.

## Features

- **Menu Bar**: File and View menus with common actions
- **Left Panel**: Category-based browser for selecting views
- **Main Display Area**: Interactive preview of selected views
- **Status Bar**: Real-time feedback and keyboard shortcuts
- **Comprehensive View Collection**: Covers all major Terminal.Gui controls

## Views Included

### Basic Controls
- Label - Text display
- Button - Clickable button
- TextField - Single-line text input  
- TextView - Multi-line text area
- CheckBox - Checkbox control

### Selectors
- ListView - Scrollable list

### Data Entry
- DateField - Date input field
- TimeField - Time input field  
- NumericUpDown - Numeric spinner

### Visual Elements
- ProgressBar - Progress indicator
- SpinnerView - Activity spinner
- Line - Drawing line

### Containers
- FrameView - Container with border
- TabView - Tab container

### Data Display
- TableView - Tabular data display
- TreeView - Hierarchical tree view

### Advanced
- ColorPicker16 - 16-color picker
- CharMap - Unicode character map
- HexView - Hex editor

## Usage

1. Run the application
2. Select a category from the dropdown in the left panel
3. Choose a specific view from the list
4. See it rendered in the main area
5. Use the menu or keyboard shortcuts for additional actions

## Keyboard Shortcuts

- **F1**: Show About dialog
- **F5**: Refresh current view
- **Ctrl+Q**: Quit application

## Building and Running

```bash
cd src/Examples/ViewShowcaseApp
dotnet run
```

This example uses CommunityToolkit.Mvvm for MVVM pattern implementation and demonstrates best practices for Terminal.Gui application architecture.