# Terminal.Gui.XamlLike

Un générateur de source (Source Generator) pour **Terminal.Gui v2** qui permet d'utiliser une syntaxe XAML-like pour définir les interfaces utilisateur de terminal.

## ✨ Fonctionnalités

- 🏗️ **Génération de code statique** - Aucun runtime XAML, code C# généré à la compilation
- ⚡ **Compatible AOT/Trimming** - Fonctionne avec les optimisations .NET modernes
- 🔄 **MVVM avec bindings** - Support des bindings OneWay et TwoWay avec `INotifyPropertyChanged`
- 🎯 **Terminal.Gui v2** - Ciblé exclusivement pour l'API Terminal.Gui v2
- 🔧 **Diagnostics intégrés** - Erreurs de build claires pour les problèmes de syntaxe

## 🚀 Installation

1. Ajoutez le package NuGet à votre projet :
```xml
<ProjectReference Include="Terminal.Gui.XamlLike" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
```

2. Incluez vos fichiers `.tui.xaml` :
```xml
<AdditionalFiles Include="Views/**/*.tui.xaml" />
```

## � Samples et Exemples

Ce projet inclut **3 projets d'exemple** dans le dossier `src/samples/` :

### 🎯 [SimpleApp](src/samples/SimpleApp/) - Sans ViewModel
- **Architecture** : Logique directe dans les vues
- **Idéal pour** : Applications simples, prototypes, apprentissage
- **Fonctionnalités** : Compteur, saisie utilisateur, gestion d'événements

### 🏗️ [MvvmApp](src/samples/MvvmApp/) - MVVM Custom
- **Architecture** : ViewModels personnalisés avec `INotifyPropertyChanged`
- **Idéal pour** : Applications moyennes, besoins MVVM spécifiques
- **Fonctionnalités** : BaseViewModel, RelayCommand, bindings avancés

### ⚡ [CommunityMvvmApp](src/samples/CommunityMvvmApp/) - CommunityToolkit.Mvvm
- **Architecture** : CommunityToolkit.Mvvm avec source generators
- **Idéal pour** : Applications complexes, MVVM moderne
- **Fonctionnalités** : `[ObservableProperty]`, `[RelayCommand]`, async commands

```bash
# Exécuter les exemples
cd src/samples/SimpleApp && dotnet run
cd src/samples/MvvmApp && dotnet run  
cd src/samples/CommunityMvvmApp && dotnet run
```

> 📚 Voir [src/samples/README.md](src/samples/README.md) pour un guide complet des exemples.

## 📝 Exemple d'usage rapide

### 1. Créer un fichier XAML (`MainView.tui.xaml`)

```xml
<Window x:Class="MyApp.Views.MainView"
        x:Name="Root"
        Title="Mon App"
        Width="Dim.Fill()"
        Height="Dim.Fill()">

  <Label x:Name="LblStatus"
         X="1" Y="1"
         Text="{Bind Status}" />

  <TextField x:Name="TxtName"
             X="1" Y="3"
             Width="30"
             Text="{Bind UserName, Mode=TwoWay}" />

  <Button x:Name="BtnSave"
          X="1" Y="5"
          Text="Sauvegarder"
          Clicked="OnSaveClicked" />
</Window>
```

### 2. Créer la classe partielle (`MainView.cs`)

```csharp
public partial class MainView : Window
{
    public MainViewModel Vm { get; }

    public MainView()
    {
        Vm = new MainViewModel();
        InitializeComponent(); // Généré automatiquement
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
        // Logique de sauvegarde
        Vm.Status = "Sauvegardé !";
    }

    partial void InitializeComponent(); // Implémenté par le générateur
}
```

### 3. Créer le ViewModel

```csharp
public class MainViewModel : INotifyPropertyChanged
{
    private string _status = "Prêt";
    private string _userName = "";

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    // INotifyPropertyChanged implementation...
}
```

## 📋 Contrôles supportés

| Élément XAML | Type Terminal.Gui | Description |
|--------------|-------------------|-------------|
| `Window` | `Terminal.Gui.Window` | Fenêtre principale |
| `Label` | `Terminal.Gui.Label` | Texte d'affichage |
| `Button` | `Terminal.Gui.Button` | Bouton cliquable |
| `TextField` | `Terminal.Gui.TextField` | Champ de saisie |
| `TextView` | `Terminal.Gui.TextView` | Zone de texte multiligne |
| `CheckBox` | `Terminal.Gui.CheckBox` | Case à cocher |
| `ListView` | `Terminal.Gui.ListView` | Liste d'éléments |
| `FrameView` | `Terminal.Gui.FrameView` | Conteneur avec bordure |

## 🔗 Bindings de données

### OneWay (VM → UI)
```xml
<Label Text="{Bind Status}" />
<Label Text="{Bind User.Name}" /> <!-- Propriétés imbriquées -->
```

### TwoWay (VM ↔ UI)
```xml
<TextField Text="{Bind UserName, Mode=TwoWay}" />
<CheckBox Checked="{Bind IsEnabled, Mode=TwoWay}" />
```

**Contrôles supportant TwoWay :**
- `TextField.Text` → événement `TextChanged`
- `TextView.Text` → événement `TextChanged`  
- `CheckBox.Checked` → événement `Toggled`

## ⚡ Performance

- **Pas de réflexion** - Tout est généré statiquement
- **Invalidation ciblée** - Seuls les contrôles affectés sont mis à jour via `SetNeedsDisplay()`
- **AOT Compatible** - Fonctionne avec Native AOT
- **Trimming friendly** - Aucune dépendance runtime cachée

## 🛠️ Configuration requise

- **.NET 6.0+** (pour le développement)
- **Terminal.Gui v2.x**
- **C# 10+** (pour les records et nullable)

## 📚 Documentation complète

- [Format XAML détaillé](docs/format.md)
- [Système de refresh et binding](docs/refresh.md)

## 🐛 Diagnostics

Le générateur fournit des erreurs de compilation claires :

- `TUI001` : Fichier XAML vide
- `TUI003` : Attribut `x:Class` manquant  
- `TUI004` : Type de contrôle inconnu
- `TUI005` : Expression de binding invalide
- `TUI006` : Binding TwoWay non supporté
- `TUI007` : Contrôle avec binding sans `x:Name`

## 💡 Exemple complet

Voir le projet [SampleApp](src/SampleApp/) pour un exemple complet fonctionnel.

## 🤝 Contribution

Les contributions sont les bienvenues ! Veuillez ouvrir une issue avant de soumettre des changements majeurs.

## 📄 Licence

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus de détails.
XAML Like for Terminal UI v2
