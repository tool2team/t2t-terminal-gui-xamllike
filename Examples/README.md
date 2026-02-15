# Terminal.Gui XamlLike - Sample Projects

This folder contains four sample projects demonstrating different architectural approaches with Terminal.Gui XamlLike framework.

## 📁 Project Structure

### 1. SimpleApp - No ViewModel Pattern
**Path**: `src/samples/SimpleApp/`

- **Architecture**: Direct UI logic in views
- **Best for**: Simple applications, prototypes, learning basics
- **Features**:
  - Counter functionality
  - User input handling
  - Direct event handling
  - No data binding complexity

**Key Files**:
- `Program.cs` - Application entry point
- `Views/MainView.cs` - View with embedded logic
- `Views/MainView.tui.xaml` - XAML UI definition

### 2. MvvmApp - Custom MVVM Pattern
**Path**: `src/samples/MvvmApp/`

- **Architecture**: Custom ViewModels with INotifyPropertyChanged
- **Best for**: Medium complexity apps, custom MVVM requirements
- **Features**:
  - Custom BaseViewModel class
  - RelayCommand implementation
  - Property change notifications
  - Command pattern with CanExecute
  - Data binding examples

**Key Files**:
- `ViewModels/BaseViewModel.cs` - Base class for ViewModels
- `ViewModels/RelayCommand.cs` - ICommand implementation
- `ViewModels/MainViewModel.cs` - Main ViewModel with business logic
- `Views/MainView.cs` - View connected to ViewModel

### 3. CommunityMvvmApp - CommunityToolkit.Mvvm
**Path**: `src/samples/CommunityMvvmApp/`

- **Architecture**: Using CommunityToolkit.Mvvm with source generators
- **Best for**: Complex applications, modern MVVM practices
- **Features**:
  - Source generators for boilerplate reduction
  - `[ObservableProperty]` attributes
  - `[RelayCommand]` attributes  
  - Async command support
  - Advanced data binding
  - Automatic property notifications

**Key Files**:
- `ViewModels/MainViewModel.cs` - ViewModel using CommunityToolkit attributes
- `Views/MainView.cs` - Advanced view with comprehensive UI updates

### 4. ViewShowcaseApp - Complete View Gallery
**Path**: `src/samples/ViewShowcaseApp/`

- **Architecture**: CommunityToolkit.Mvvm with comprehensive UI showcase
- **Best for**: Learning all Terminal.Gui controls, testing views, reference
- **Features**:
  - Interactive browser for all Terminal.Gui views
  - Category-organized view selection
  - Menu bar with File and View menus
  - Status bar with real-time feedback
  - Dynamic view loading and testing
  - Comprehensive view coverage (25+ controls)

**Key Files**:
- `Models/ViewInfo.cs` - View metadata and factory methods
- `ViewModels/MainViewModel.cs` - Main application logic
- `Views/MainWindow.tui.xaml` - Complex layout with menu, panels, and status bar

## 🚀 How to Run

### Prerequisites
- .NET 8.0 SDK
- Terminal that supports Terminal.Gui

### Running Each Sample

```bash
# Simple App (No ViewModel)
cd src/samples/SimpleApp
dotnet run

# MVVM App (Custom ViewModels)  
cd src/samples/MvvmApp
dotnet run

# CommunityMvvm App (CommunityToolkit.Mvvm)
cd src/samples/CommunityMvvmApp
dotnet run

# View Showcase App (Complete View Gallery)
cd src/samples/ViewShowcaseApp
dotnet run
```

## 🎯 Feature Comparison

| Feature | SimpleApp | MvvmApp | CommunityMvvmApp | ViewShowcaseApp |
|---------|-----------|---------|------------------|------------------|
| **Complexity** | Low | Medium | High | High |
| **Data Binding** | Manual | INotifyPropertyChanged | Full MVVM with source generators | Full MVVM with source generators |
| **Commands** | Event handlers | Custom RelayCommand | Auto-generated commands | Auto-generated commands |
| **Async Support** | Basic | Manual implementation | Built-in async commands | Built-in async commands |
| **Boilerplate Code** | High | Medium | Low (source generators) | Low (source generators) |
| **Learning Curve** | Easy | Moderate | Steeper initially, easier long-term | Moderate (comprehensive reference) |
| **Testability** | Low | Good | Excellent | Excellent |
| **Maintainability** | Limited | Good | Excellent | Excellent |
| **UI Layout** | Simple | Simple | Medium | Complex (menu, panels, status) |
| **View Coverage** | 1 basic | 1 basic | 1 intermediate | 25+ comprehensive |

## 📚 Learning Path

1. **Start with SimpleApp** - Understand basic Terminal.Gui XamlLike concepts
2. **Move to MvvmApp** - Learn MVVM principles and data binding
3. **Explore CommunityMvvmApp** - See modern MVVM with advanced features
4. **Use ViewShowcaseApp** - Comprehensive reference for all available controls and layouts

## 🔧 Key Concepts Demonstrated

### Data Binding Syntax
```xml
<!-- One-way binding -->
<Label Text="{Bind ViewModel.WelcomeMessage}" />

<!-- Two-way binding -->
<TextField Text="{Bind ViewModel.UserName, TwoWay}" />

<!-- Property path binding -->
<Label Text="{Bind ViewModel.Status}" />
```

### Event Handling
```xml
<!-- Direct event handler -->
<Button Text="Click Me" Clicked="OnButtonClicked" />
```

### Layout and Positioning
```xml
<!-- Absolute positioning -->
<Button X="2" Y="9" Text="Button" />

<!-- Relative positioning -->
<Button X="Pos.Right(Root) - 10" Y="Pos.Bottom(Root) - 3" Text="Exit" />
```

## 🧪 Extending the Samples

Each sample can be extended to explore:

- **Custom Controls**: Create reusable UI components
- **Advanced Layouts**: Experiment with different positioning
- **Data Validation**: Add input validation logic
- **Navigation**: Implement multi-view applications
- **Services**: Add dependency injection and services
- **Themes**: Customize appearance and styling

## 📖 Additional Resources

- [Terminal.Gui Documentation](https://gui-cs.github.io/Terminal.Gui/)
- [CommunityToolkit.Mvvm Documentation](https://docs.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [MVVM Pattern Guide](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm)

---

Choose the sample that best fits your project's complexity and requirements!