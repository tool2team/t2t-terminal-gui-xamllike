using Microsoft.CodeAnalysis;

namespace Terminal.Gui.XamlLike
{

/// <summary>
/// Contains all diagnostic descriptors for the TerminalGui XAML generator
/// </summary>
public static class TuiDiagnostics
{
    /// <summary>
    /// Category for all TUI XAML diagnostics
    /// </summary>
    public const string Category = "Terminal.Gui.XamlLike";

    /// <summary>
    /// Empty or whitespace-only XAML file
    /// </summary>
    public static readonly DiagnosticDescriptor EmptyXamlFile = new(
        id: "TUI001",
        title: "Empty XAML file",
        messageFormat: "The .tui.xaml file is empty or contains only whitespace",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "XAML files must contain valid XML content with a root element.");

    /// <summary>
    /// Invalid XML content
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidXml = new(
        id: "TUI002",
        title: "Invalid XML",
        messageFormat: "Invalid XML content in .tui.xaml file: {0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The XAML file contains invalid XML syntax that cannot be parsed.");

    /// <summary>
    /// Missing x:Class attribute on root element
    /// </summary>
    public static readonly DiagnosticDescriptor MissingXClass = new(
        id: "TUI003",
        title: "Missing x:Class attribute",
        messageFormat: "The root element must have an x:Class attribute specifying the full class name",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Every XAML file must specify x:Class on the root element to generate the corresponding partial class.");

    /// <summary>
    /// Unknown control type
    /// </summary>
    public static readonly DiagnosticDescriptor UnknownControlType = new(
        id: "TUI004",
        title: "Unknown control type",
        messageFormat: "Unknown control type '{0}'. Supported types: Window, Label, Button, TextField, TextView, CheckBox, OptionSelector, ListView, FrameView, ScrollView, TabView, MenuBar, StatusBar.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The specified control type is not supported by Terminal.Gui v2 or this generator.");

    /// <summary>
    /// Invalid binding expression
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidBinding = new(
        id: "TUI005",
        title: "Invalid binding expression",
        messageFormat: "Invalid binding expression '{0}'. Expected format: {{Bind PropertyPath}} or {{Bind PropertyPath, Mode=TwoWay}}.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Binding expressions must follow the correct syntax and reference valid property paths.");

    /// <summary>
    /// TwoWay binding not supported on this control/property combination
    /// </summary>
    public static readonly DiagnosticDescriptor UnsupportedTwoWayBinding = new(
        id: "TUI006",
        title: "Unsupported TwoWay binding",
        messageFormat: "TwoWay binding is not supported on {0}.{1}. Supported TwoWay bindings: TextField.Text, TextView.Text, CheckBox.Checked.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "TwoWay binding is only supported on specific control/property combinations that have change notification events.");

    /// <summary>
    /// Unknown event name for control type
    /// </summary>
    public static readonly DiagnosticDescriptor UnknownEvent = new(
        id: "TUI007",
        title: "Unknown event",
        messageFormat: "Unknown event '{1}' for control type '{0}'. Check Terminal.Gui v2 documentation for available events.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "The specified event name may not exist on the control type or may have changed in Terminal.Gui v2.");

    /// <summary>
    /// Empty event handler name
    /// </summary>
    public static readonly DiagnosticDescriptor EmptyEventHandler = new(
        id: "TUI008",
        title: "Empty event handler",
        messageFormat: "Event '{0}' has an empty handler name. Specify a method name like 'OnButtonClick'.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Event handlers must specify a valid method name that will be called when the event occurs.");

    /// <summary>
    /// General parsing error
    /// </summary>
    public static readonly DiagnosticDescriptor ParseError = new(
        id: "TUI009",
        title: "Parse error",
        messageFormat: "Error parsing XAML file: {0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "An unexpected error occurred while parsing the XAML file.");

    /// <summary>
    /// Missing ViewModel property
    /// </summary>
    public static readonly DiagnosticDescriptor MissingViewModel = new(
        id: "TUI010",
        title: "Missing ViewModel property",
        messageFormat: "Class '{0}' has data bindings but no 'Vm' property implementing INotifyPropertyChanged",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Classes with data bindings must expose a 'Vm' property that implements INotifyPropertyChanged.");

    /// <summary>
    /// Duplicate x:Name values
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateXName = new(
        id: "TUI011",
        title: "Duplicate x:Name",
        messageFormat: "Duplicate x:Name '{0}' found. Each control must have a unique name within the same XAML file.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "x:Name values must be unique within each XAML file to avoid naming conflicts in generated code.");

    /// <summary>
    /// Obsolete event used
    /// </summary>
    public static readonly DiagnosticDescriptor ObsoleteEvent = new(
        id: "TUI012",
        title: "Obsolete event",
        messageFormat: "Event '{0}' on control '{1}' is obsolete. {2}. The event will not be generated.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "This event has been deprecated in Terminal.Gui v2 and will not be included in the generated code. Please migrate to the recommended event.");

    /// <summary>
    /// Invalid property value
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyValue = new(
        id: "TUI013",
        title: "Invalid property value",
        messageFormat: "Invalid value '{1}' for property '{0}'. {2}.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "The property value may not be compatible with the expected type or format.");

    /// <summary>
    /// Generator debug - Processing file
    /// </summary>
    public static readonly DiagnosticDescriptor GeneratorProcessingFile = new(
        id: "TUI014",
        title: "Generator Debug",
        messageFormat: "Processing file: {0}",
        category: "Debug",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Informational message indicating which file is being processed by the generator.");

    /// <summary>
    /// Generator debug - Generated code length
    /// </summary>
    public static readonly DiagnosticDescriptor GeneratorCodeLength = new(
        id: "TUI015",
        title: "Generator Debug",
        messageFormat: "Generated code length: {0}",
        category: "Debug",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Informational message showing the length of generated code.");

    /// <summary>
    /// Unknown root element type
    /// </summary>
    public static readonly DiagnosticDescriptor UnknownRootElementType = new(
        id: "TUI016",
        title: "Unknown root element type",
        messageFormat: "Element '{0}' is not a recognized Terminal.Gui control type. Add it to Mappings.ControlMappings.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The root element must be a valid Terminal.Gui control type.");

    /// <summary>
    /// Root element must be a container
    /// </summary>
    public static readonly DiagnosticDescriptor RootElementNotContainer = new(
        id: "TUI017",
        title: "Root element type must be a container",
        messageFormat: "Element '{0}' is not a container type",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The root element must be a container control that can have children (Window, View, FrameView, etc.).");

    /// <summary>
    /// Unknown control type in code generation
    /// </summary>
    public static readonly DiagnosticDescriptor UnknownControlTypeInGeneration = new(
        id: "TUI018",
        title: "Unknown control type",
        messageFormat: "Element '{0}' is not a recognized Terminal.Gui control type. Add it to Mappings.ControlMappings.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The control type is not recognized during code generation.");
}

    public static class DiagnosticExtensions
    {
        public static TuiDiagnostic Create(this DiagnosticDescriptor descriptor, string filePath, params object[] args) =>
            new(descriptor, filePath, string.Format(descriptor.MessageFormat.ToString(), args));
    }
}