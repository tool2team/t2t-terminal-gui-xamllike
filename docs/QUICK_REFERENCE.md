# Quick Reference - Terminal.Gui.XamlLike

Guide de référence rapide pour la syntaxe XAML-like.

## 🎯 Structure de base

```xml
<Window x:Class="Namespace.ClassName"
        x:Name="Root"
        x:DataType="ViewModel"
        Title="Titre"
        Width="Dim.Fill()"
        Height="Dim.Fill()">
  <!-- Contrôles -->
</Window>
```

## 📋 Attributs essentiels

| Attribut | Obligatoire | Description | Exemple |
|----------|-------------|-------------|---------|
| `x:Class` | ✅ | Nom complet de la classe C# | `"MonApp.Views.MainView"` |
| `x:Name` | ❌ | Nom du champ généré | `"Root"`, `"BtnSave"` |
| `x:DataType` | ❌ | Contexte de binding par défaut | `"ViewModel"` |

## 🎨 Contrôles

### Conteneurs

```xml
<Window Title="..." />
<FrameView Title="..." />
<ScrollView />
```

### Affichage

```xml
<Label Text="..." />
```

### Saisie

```xml
<TextField Text="..." Width="20" />
<TextView Text="..." Width="40" Height="10" />
<CheckBox Text="..." Checked="true" />
```

### Actions

```xml
<Button Text="..." Clicked="OnClick" />
```

### Listes

```xml
<RadioGroup RadioLabels="Item1,Item2,Item3" SelectedItem="0" />
<ListView />
```

## 📐 Positionnement

```xml
<!-- Valeurs fixes -->
<Label X="10" Y="5" Width="20" Height="1" />

<!-- Expressions Terminal.Gui -->
<Label X="Pos.Center()" Y="Pos.Bottom(otherView) + 1" />
<TextField Width="Dim.Fill(2)" Height="Dim.Percent(50)" />
```

## 🔗 Binding de données

### Syntaxe avec x:DataType (Recommandé)

```xml
<Window x:DataType="ViewModel">
  <!-- OneWay (défaut) -->
  <Label Text="{Bind Status}" />
  <Button Enabled="{Bind CanSave}" />
  
  <!-- TwoWay -->
  <TextField Text="{Bind UserName, Mode=TwoWay}" />
  <CheckBox Checked="{Bind IsEnabled, Mode=TwoWay}" />
</Window>
```

### Syntaxe explicite (Sans x:DataType)

```xml
<Label Text="{Bind ViewModel.Status}" />
<TextField Text="{Bind ViewModel.UserName, Mode=TwoWay}" />
```

### Propriétés imbriquées

```xml
<Label Text="{Bind User.Name}" />
<Label Text="{Bind Config.Display.Title}" />
```

### Binding vers collections

```xml
<RadioGroup RadioLabels="{Bind AvailableItems}" />
```

## ⚡ Événements

```xml
<Button Clicked="OnSaveClicked" />
<TextField TextChanged="OnTextChanged" Accept="OnAccept" />
<CheckBox Toggled="OnToggled" />
<Window Loaded="OnLoaded" Closing="OnClosing" />
```

## 🏗️ Template de ViewModel

```csharp
using System.ComponentModel;

public class MainViewModel : INotifyPropertyChanged
{
    private string _myProperty = "";
    
    public string MyProperty
    {
        get => _myProperty;
        set
        {
            if (_myProperty != value)
            {
                _myProperty = value;
                OnPropertyChanged(nameof(MyProperty));
            }
        }
    }
    
    // Propriété calculée
    public string ComputedProperty => $"Computed: {MyProperty}";
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

## 🏗️ Template de Vue

```csharp
using Terminal.Gui;

namespace MonApp.Views
{
    public partial class MainView : Window
    {
        public MainViewModel ViewModel { get; }
        
        public MainView()
        {
            ViewModel = new MainViewModel();
            InitializeComponent(); // Généré automatiquement
        }
        
        // Event handlers
        private void OnSaveClicked(object? sender, EventArgs e)
        {
            // Logique
        }
    }
}
```

## 🎯 Modes de Binding

| Mode | Symbole | Description | Usage |
|------|---------|-------------|-------|
| `OneWay` | → | ViewModel → UI | Affichage (défaut) |
| `TwoWay` | ↔️ | ViewModel ↔️ UI | Saisie utilisateur |

## 📦 Propriétés supportant TwoWay

| Contrôle | Propriété | Événement |
|----------|-----------|-----------|
| `TextField` | `Text` | `TextChanged` |
| `TextView` | `Text` | `TextChanged` |
| `CheckBox` | `Checked` | `Toggled` |

## 💡 Exemples complets

### Simple (Sans ViewModel)

```xml
<Window x:Class="SimpleView" Title="Simple">
  <Label Text="Hello World" />
  <Button Text="Click" Clicked="OnClick" />
</Window>
```

```csharp
public partial class SimpleView : Window
{
    public SimpleView() => InitializeComponent();
    
    private void OnClick(object? sender, EventArgs e)
    {
        // Logique
    }
}
```

### MVVM (Avec ViewModel)

```xml
<Window x:Class="MvvmView" x:DataType="ViewModel">
  <Label Text="{Bind Message}" />
  <TextField Text="{Bind Input, Mode=TwoWay}" />
  <Button Text="Save" Clicked="OnSave" Enabled="{Bind CanSave}" />
</Window>
```

```csharp
public partial class MvvmView : Window
{
    public MainViewModel ViewModel { get; }
    
    public MvvmView()
    {
        ViewModel = new MainViewModel();
        InitializeComponent();
    }
    
    private void OnSave(object? sender, EventArgs e)
    {
        ViewModel.Save();
    }
}
```

## 🚫 Pièges courants

### ❌ Oublier INotifyPropertyChanged

```csharp
// ❌ MAUVAIS - Pas de mise à jour UI
public string Status { get; set; }

// ✅ BON
public string Status
{
    get => _status;
    set
    {
        _status = value;
        OnPropertyChanged(nameof(Status));
    }
}
```

### ❌ Initialiser ViewModel après InitializeComponent

```csharp
// ❌ MAUVAIS - Exception
public MainView()
{
    InitializeComponent();
    ViewModel = new MainViewModel();
}

// ✅ BON
public MainView()
{
    ViewModel = new MainViewModel();
    InitializeComponent();
}
```

### ❌ Ne pas notifier les propriétés calculées

```csharp
public string FirstName
{
    set
    {
        _firstName = value;
        OnPropertyChanged(nameof(FirstName));
        // ❌ OUBLIÉ : OnPropertyChanged(nameof(FullName));
    }
}

public string FullName => $"{FirstName} {LastName}";
```

## 📚 Documentation complète

- [Format XAML](docs/format.md)
- [Guide MVVM](docs/mvvm-guide.md)
- [Implémentation Binding](BINDING_IMPLEMENTATION.md)

## 🆘 Aide rapide

```bash
# Compiler
dotnet build

# Exécuter les exemples
dotnet run --project samples/SimpleApp
dotnet run --project samples/MvvmApp
dotnet run --project samples/CommunityMvvmApp

# Voir les fichiers générés
# Chercher dans obj/Debug/net8.0/Terminal.Gui.XamlLike/
```

## 🔗 Liens utiles

- [Terminal.Gui Documentation](https://gui-cs.github.io/Terminal.Gui/)
- [INotifyPropertyChanged (Microsoft)](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged)
- [Pattern MVVM (Microsoft)](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
