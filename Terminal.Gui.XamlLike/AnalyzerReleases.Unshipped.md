; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
TUI001 | Terminal.Gui.XamlLike | Error | Empty XAML file
TUI002 | Terminal.Gui.XamlLike | Error | Invalid XML
TUI003 | Terminal.Gui.XamlLike | Error | Missing x:Class attribute
TUI004 | Terminal.Gui.XamlLike | Error | Unknown control type
TUI005 | Terminal.Gui.XamlLike | Error | Invalid binding expression
TUI006 | Terminal.Gui.XamlLike | Error | Unsupported TwoWay binding
TUI007 | Terminal.Gui.XamlLike | Warning | Unknown event
TUI008 | Terminal.Gui.XamlLike | Error | Empty event handler
TUI009 | Terminal.Gui.XamlLike | Error | Parse error
TUI010 | Terminal.Gui.XamlLike | Error | Missing ViewModel property
TUI011 | Terminal.Gui.XamlLike | Error | Duplicate x:Name
TUI012 | Terminal.Gui.XamlLike | Error | Obsolete event
TUI013 | Terminal.Gui.XamlLike | Warning | Invalid property value
TUI014 | Debug | Info | Generator processing file (debug)
TUI015 | Debug | Info | Generated code length (debug)
TUI016 | Terminal.Gui.XamlLike | Error | Unknown root element type
TUI017 | Terminal.Gui.XamlLike | Error | Root element must be container
TUI018 | Terminal.Gui.XamlLike | Error | Unknown control type in generation
