using Xunit;
using Terminal.Gui.XamlLike;

namespace Terminal.Gui.XamlLike.Tests.Unit;

/// <summary>
/// Unit tests for XamlParser - parsing and validation of .tui.xaml files
/// </summary>
public class XamlParserTests
{
    #region Parse Tests - Valid XAML (8 tests)

    [Fact]
    public void Parse_ValidSimpleXaml_ReturnsSuccess()
    {
        // Arrange
        var xaml = """
            <Button x:Class="Test.ButtonView"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    Text="Click Me" />
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Null(result.Error);
        Assert.Equal("Button", result.Value.RootElement.Name);
        Assert.Equal("Test.ButtonView", result.Value.ClassName);
    }

    [Fact]
    public void Parse_XamlWithChildren_ParsesHierarchy()
    {
        // Arrange
        var xaml = """
            <Window x:Class="Test.MainWindow"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Label Text="Hello" />
                <Button Text="OK" />
            </Window>
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.RootElement.Children.Count);
        Assert.Equal("Label", result.Value.RootElement.Children[0].Name);
        Assert.Equal("Button", result.Value.RootElement.Children[1].Name);
    }

    [Fact]
    public void Parse_XamlWithAttributes_ParsesAllAttributes()
    {
        // Arrange
        var xaml = """
            <TextField x:Class="Test.Input"
                       xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                       xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                       x:Name="MyField"
                       Text="Sample"
                       Width="30"
                       Height="5" />
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.True(result.IsSuccess);
        var element = result.Value!.RootElement;
        Assert.Equal("MyField", element.XName);
        Assert.Equal("Sample", element.PropertyAttributes["Text"]);
        Assert.Equal("30", element.PropertyAttributes["Width"]);
        Assert.Equal("5", element.PropertyAttributes["Height"]);
    }

    [Fact]
    public void Parse_XamlWithDataType_ExtractsDataType()
    {
        // Arrange
        var xaml = """
            <Window x:Class="Test.MainView"
                    x:DataType="Test.MainViewModel"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" />
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Test.MainViewModel", result.Value!.DataType);
    }

    [Fact]
    public void Parse_XamlWithBindings_ParsesBindingExpressions()
    {
        // Arrange
        var xaml = """
            <TextField x:Class="Test.Input"
                       xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                       xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                       Text="{Bind UserName, Mode=TwoWay}" />
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("Text", result.Value!.RootElement.PropertyAttributes.Keys);
        Assert.Contains("{Bind UserName, Mode=TwoWay}", result.Value.RootElement.PropertyAttributes["Text"]);
    }

    [Fact]
    public void Parse_XamlWithEvents_ParsesEventHandlers()
    {
        // Arrange
        var xaml = """
            <Button x:Class="Test.ButtonView"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    Accepting="OnAccepting"
                    Activated="OnActivated" />
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.RootElement.EventAttributes.Count);
        Assert.Equal("OnAccepting", result.Value.RootElement.EventAttributes["Accepting"]);
        Assert.Equal("OnActivated", result.Value.RootElement.EventAttributes["Activated"]);
    }

    [Fact]
    public void Parse_XamlWithNestedChildren_ParsesDeepHierarchy()
    {
        // Arrange
        var xaml = """
            <Window x:Class="Test.MainWindow"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <FrameView Title="Container">
                    <Label Text="Level 2" />
                    <FrameView Title="Nested">
                        <Label Text="Level 3" />
                    </FrameView>
                </FrameView>
            </Window>
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.True(result.IsSuccess);
        var root = result.Value!.RootElement;
        Assert.Single(root.Children);
        var frameView = root.Children[0];
        Assert.Equal(2, frameView.Children.Count);
        var nestedFrame = frameView.Children[1];
        Assert.Single(nestedFrame.Children);
    }

    [Fact]
    public void Parse_XamlPreservesLineNumbers()
    {
        // Arrange
        var xaml = """
            <Window x:Class="Test.MainWindow"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Label Text="First" />
                <Button Text="Second" />
            </Window>
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.RootElement.LineNumber > 0);
    }

    #endregion

    #region Parse Tests - Invalid XAML (8 tests)

    [Fact]
    public void Parse_EmptyString_ReturnsError()
    {
        // Act
        var result = XamlParser.Parse("", "test.tui.xaml");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsError()
    {
        // Act
        var result = XamlParser.Parse("   \n\t  ", "test.tui.xaml");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Parse_MissingXClass_ReturnsError()
    {
        // Arrange
        var xaml = """
            <Button xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    Text="No Class" />
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Parse_InvalidXml_ReturnsError()
    {
        // Arrange
        var xaml = "<Button x:Class='Test' <invalid>";

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Parse_UnclosedTag_ReturnsError()
    {
        // Arrange
        var xaml = """
            <Window x:Class="Test.Window"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Label Text="Unclosed"
            </Window>
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Parse_MismatchedTags_ReturnsError()
    {
        // Arrange
        var xaml = """
            <Window x:Class="Test.Window"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Label Text="Test" />
            </Button>
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Parse_InvalidAttributeQuotes_ReturnsError()
    {
        // Arrange
        var xaml = """
            <Button x:Class="Test.Button"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    Text='Mismatched" />
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Parse_NoRootElement_ReturnsError()
    {
        // Arrange
        var xaml = "<!-- Just a comment -->";

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Validate Tests - Valid Cases (5 tests)

    [Fact]
    public void Validate_ValidDocument_ReturnsNoDiagnostics()
    {
        // Arrange
        var xaml = """
            <Button x:Class="Test.ButtonView"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    Text="Click Me" />
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");
        Assert.True(parseResult.IsSuccess);

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public void Validate_ValidTwoWayBinding_ReturnsNoDiagnostics()
    {
        // Arrange
        var xaml = """
            <TextField x:Class="Test.Input"
                       xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                       xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                       Text="{Bind UserName, Mode=TwoWay}" />
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public void Validate_ValidEvent_ReturnsNoDiagnostics()
    {
        // Arrange
        var xaml = """
            <Button x:Class="Test.ButtonView"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    Accepting="OnAccepting" />
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public void Validate_KnownControlWithChildren_ReturnsNoDiagnostics()
    {
        // Arrange
        var xaml = """
            <Window x:Class="Test.MainWindow"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Label Text="Valid" />
                <Button Text="OK" />
            </Window>
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public void Validate_ValidBindingWithDataType_ReturnsNoDiagnostics()
    {
        // Arrange
        var xaml = """
            <Window x:Class="Test.MainView"
                    x:DataType="Test.MainViewModel"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Label Text="{Bind Status}" />
            </Window>
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.Empty(diagnostics);
    }

    #endregion

    #region Validate Tests - Invalid Cases (7 tests)

    [Fact]
    public void Validate_UnknownControl_ReturnsDiagnostic()
    {
        // Arrange
        var xaml = """
            <NonExistentControl x:Class="Test.Custom"
                                xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" />
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.NotEmpty(diagnostics);
        Assert.Contains(diagnostics, d => d.Descriptor.Id.Contains("TUI"));
    }

    [Fact]
    public void Validate_InvalidBinding_ReturnsDiagnostic()
    {
        // Arrange
        var xaml = """
            <Label x:Class="Test.LabelView"
                   xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                   xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                   Text="{Bind InvalidSyntax" />
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.NotEmpty(diagnostics);
    }

    [Fact]
    public void Validate_UnsupportedTwoWayBinding_ReturnsDiagnostic()
    {
        // Arrange
        var xaml = """
            <Label x:Class="Test.LabelView"
                   xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                   xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                   Text="{Bind Message, Mode=TwoWay}" />
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.NotEmpty(diagnostics);
    }

    [Fact]
    public void Validate_UnknownEvent_ReturnsDiagnostic()
    {
        // Arrange
        var xaml = """
            <Button x:Class="Test.ButtonView"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    NonExistentEvent="Handler" />
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.NotEmpty(diagnostics);
    }

    [Fact]
    public void Validate_EmptyEventHandler_ReturnsDiagnostic()
    {
        // Arrange
        var xaml = """
            <Button x:Class="Test.ButtonView"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    Accepting="" />
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.NotEmpty(diagnostics);
    }

    [Fact]
    public void Validate_ChildrenWithUnknownControls_ReturnsDiagnostics()
    {
        // Arrange
        var xaml = """
            <Window x:Class="Test.MainWindow"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <UnknownControl1 />
                <UnknownControl2 />
            </Window>
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.True(diagnostics.Count >= 2);
    }

    [Fact]
    public void Validate_NestedInvalidElements_ReturnsDiagnostics()
    {
        // Arrange
        var xaml = """
            <Window x:Class="Test.MainWindow"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <FrameView>
                    <InvalidControl />
                </FrameView>
            </Window>
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.NotEmpty(diagnostics);
    }

    #endregion

    #region ParseResult Tests (3 tests)

    [Fact]
    public void ParseResult_Success_HasCorrectState()
    {
        // Act
        var result = ParseResult<string>.Success("test");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("test", result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void ParseResult_Error_HasCorrectState()
    {
        // Act
        var result = ParseResult<string>.CreateError(
            TuiDiagnostics.EmptyXamlFile,
            "test.xaml");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void ParseResult_ErrorWithMessage_IncludesMessage()
    {
        // Act
        var result = ParseResult<string>.CreateError(
            TuiDiagnostics.ParseError,
            "test.xaml",
            "Custom error message");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Edge Cases and Special Scenarios (4 tests)

    [Fact]
    public void Parse_XamlWithSpecialCharacters_ParsesCorrectly()
    {
        // Arrange
        var xaml = """
            <Label x:Class="Test.LabelView"
                   xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                   xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                   Text="Special &lt;chars&gt; &amp; quotes" />
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Parse_LargeXaml_ParsesSuccessfully()
    {
        // Arrange - Generate large XAML with many children
        var children = string.Concat(Enumerable.Range(0, 100)
            .Select(i => $"<Label Text=\"Item{i}\" />"));
        
        var xaml = $"""
            <Window x:Class="Test.LargeWindow"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                {children}
            </Window>
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(100, result.Value!.RootElement.Children.Count);
    }

    [Fact]
    public void Parse_XamlWithNamespacePrefix_ParsesCorrectly()
    {
        // Arrange
        var xaml = """
            <tui:Button x:Class="Test.ButtonView"
                        xmlns:tui="http://schemas.gui-cs.github.io/tui/2026/xaml"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        Text="Prefixed" />
            """;

        // Act
        var result = XamlParser.Parse(xaml, "test.tui.xaml");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Button", result.Value!.RootElement.Name);
    }

    [Fact]
    public void Validate_MixedValidAndInvalid_ReturnsAllErrors()
    {
        // Arrange
        var xaml = """
            <Window x:Class="Test.MainWindow"
                    xmlns="http://schemas.gui-cs.github.io/tui/2026/xaml"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Label Text="Valid" />
                <InvalidControl />
                <Button InvalidEvent="Handler" />
            </Window>
            """;
        var parseResult = XamlParser.Parse(xaml, "test.tui.xaml");

        // Act
        var diagnostics = XamlParser.Validate(parseResult.Value!);

        // Assert
        Assert.True(diagnostics.Count >= 2); // At least 2 errors
    }

    #endregion
}
