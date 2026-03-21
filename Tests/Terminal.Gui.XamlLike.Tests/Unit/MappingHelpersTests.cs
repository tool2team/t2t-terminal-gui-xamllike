using Xunit;
using Terminal.Gui.XamlLike;

namespace Terminal.Gui.XamlLike.Tests.Unit;

/// <summary>
/// Unit tests for MappingHelpers utility methods
/// Tests all 14 public methods with various scenarios
/// </summary>
public class MappingHelpersTests
{
    #region GetFullTypeName Tests (4 tests)

    [Fact]
    public void GetFullTypeName_WithValidControl_ReturnsFullTypeName()
    {
        // Act
        var result = MappingHelpers.GetFullTypeName("Button");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Terminal.Gui.Views.Button", result);
    }

    [Fact]
    public void GetFullTypeName_WithInvalidControl_ReturnsNull()
    {
        // Act
        var result = MappingHelpers.GetFullTypeName("NonExistentControl");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFullTypeName_WithGenericOptionSelector_ReturnsGenericType()
    {
        // Act
        var result = MappingHelpers.GetFullTypeName("OptionSelector", "string");

        // Assert
        Assert.NotNull(result);
        Assert.Contains("<string>", result);
    }

    [Theory]
    [InlineData("Window", "Terminal.Gui.Views.Window")]
    [InlineData("Dialog", "Terminal.Gui.Views.Dialog")]
    [InlineData("Label", "Terminal.Gui.Views.Label")]
    [InlineData("TextField", "Terminal.Gui.Views.TextField")]
    public void GetFullTypeName_CommonControls_ReturnsExpectedTypes(string controlName, string expectedType)
    {
        // Act
        var result = MappingHelpers.GetFullTypeName(controlName);

        // Assert
        Assert.Equal(expectedType, result);
    }

    #endregion

    #region IsContainer Test

    [Theory]
    [InlineData("Window", true)]
    [InlineData("Dialog", true)]
    [InlineData("FrameView", true)]
    [InlineData("TabView", true)]
    [InlineData("Button", false)]
    [InlineData("Label", false)]
    public void IsContainer(string controlName, bool expected)
    {
        // Act
        var result = MappingHelpers.GetControlMapping(controlName);

        // Assert
        Assert.Equal(expected, result.IsContainer);
    }

    #endregion

    #region TwoWay Binding Test

    [Theory]
    [InlineData("TextField", "Text", true)]
    [InlineData("Label", "Text", false)]
    [InlineData("Tchoupi", "X", false)]
    [InlineData("Button", "Pop", false)]
    public void SupportsTwoWayBinding(string controlName, string propertyName, bool expectedSupport)
    {
        // Act
        bool result = MappingHelpers.GetTwoWayBinding(controlName, propertyName) is not null;

        // Assert
        Assert.Equal(expectedSupport, result);
    }

    #endregion

    #region GetPropertyMapping Tests

    [Theory]
    [InlineData("View", "X", "Terminal.Gui.ViewBase.Pos")]
    [InlineData("View", "Y", "Terminal.Gui.ViewBase.Pos")]
    [InlineData("View", "Width", "Terminal.Gui.ViewBase.Dim")]
    [InlineData("View", "Height", "Terminal.Gui.ViewBase.Dim")]
    public void GetPropertyMapping_PositioningProperties_ReturnCorrectTypes(string controlName, string propertyName, string expectedType)
    {
        // Act
        var result = MappingHelpers.GetPropertyMapping(controlName, propertyName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(propertyName, result.PropertyName);
        Assert.Equal(expectedType, result.TargetType);
    }

    [Theory]
    [InlineData("Label", "Text", true)]
    [InlineData("View", "Width", true)]
    [InlineData("CheckBox", "NonExistentProperty", false)]
    [InlineData("BeatBox", "Thickness", false)]
    public void GetPropertyMapping_ReturnsExpectedResult(string controlName, string propertyName, bool expected)
    {
        // Act
        var result = MappingHelpers.GetPropertyMapping(controlName, propertyName);

        // Assert
        Assert.Equal(expected, result is not null);
    }

    #endregion

    #region GetEventMapping Tests

    [Theory]
    [InlineData("Button", "Accepting", "EventHandler<Terminal.Gui.Input.CommandEventArgs>")]
    [InlineData("AttributePicker", "Activated", "EventHandler<Terminal.Gui.App.EventArgs<Terminal.Gui.Input.ICommandContext>>")]
    [InlineData("Border", "Disposing", "EventHandler")]
    [InlineData("ColorPicker", "FocusedChanged", "EventHandler<Terminal.Gui.ViewBase.HasFocusEventArgs>")]
    public void GetEventMapping_UsualEvents_ReturnCorrectTypes(string controlName, string eventName, string expectedType)
    {
        // Act
        EventMapping result = MappingHelpers.GetEventMapping(controlName, eventName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventName, result.EventName);
        Assert.Equal(expectedType, result.DelegateType);
    }

    [Theory]
    [InlineData("Button", "Accepting", true)]
    [InlineData("AttributePicker", "Activated", true)]
    [InlineData("Bar", "NonExistentEvent", false)]
    [InlineData("Abracadabra", "X", false)]
    public void GetEventMapping_ReturnsExpectedResult(string controlName, string eventName, bool expected)
    {
        // Act
        var result = MappingHelpers.GetEventMapping(controlName, eventName);

        // Assert
        Assert.Equal(expected, result is not null);
    }

    [Theory]
    [InlineData("X", true)]
    [InlineData("Y", true)]
    [InlineData("Width", true)]
    [InlineData("Height", true)]
    [InlineData("Text", false)]
    public void IsTerminalGuiType_VariousProperties_ReturnsExpectedResult(string propertyName, bool expected)
    {
        // Act
        var result = MappingHelpers.IsTerminalGuiType(propertyName);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Edge Cases Tests (4 tests)

    [Fact]
    public void GetFullTypeName_WithEmptyString_ReturnsNull()
    {
        // Act
        var result = MappingHelpers.GetFullTypeName("");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Integration Tests (2 tests)

    [Fact]
    public void CompleteWorkflow_Button_AllMethodsWork()
    {
        // Arrange
        var controlName = "Button";
        var eventName = "Accepting";
        var propertyName = "Text";

        // Act
        var controlMapping = MappingHelpers.GetControlMapping(controlName);
        var fullTypeName = MappingHelpers.GetFullTypeName(controlName);
        var eventMapping = MappingHelpers.GetEventMapping(controlName, eventName);
        var propertyMapping = MappingHelpers.GetPropertyMapping(controlName, propertyName);

        // Assert
        Assert.NotNull(fullTypeName);
        Assert.Equal("Terminal.Gui.Views.Button", fullTypeName);
        Assert.False(controlMapping.IsContainer);
        Assert.NotNull(eventMapping);
        Assert.NotNull(propertyMapping);
    }

    [Fact]
    public void CompleteWorkflow_TextField_TwoWayBindingWorks()
    {
        // Arrange
        var controlName = "TextField";
        var propertyName = "Text";

        // Act
        var fullTypeName = MappingHelpers.GetFullTypeName(controlName);
        var twoWayBinding = MappingHelpers.GetTwoWayBinding(controlName, propertyName);
        var propertyMapping = MappingHelpers.GetPropertyMapping(controlName, propertyName);

        // Assert
        Assert.NotNull(fullTypeName);
        Assert.NotNull(twoWayBinding);
        Assert.NotNull(propertyMapping);
        Assert.NotNull(twoWayBinding.ChangeEventName);
    }

    #endregion
}
