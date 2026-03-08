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

    #region IsContainer Tests (3 tests)

    [Theory]
    [InlineData("Window")]
    [InlineData("Dialog")]
    [InlineData("FrameView")]
    [InlineData("TabView")]
    public void IsContainer_WithContainerControls_ReturnsTrue(string controlName)
    {
        // Act
        var result = MappingHelpers.IsContainer(controlName);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("Button")]
    [InlineData("Label")]
    [InlineData("TextField")]
    [InlineData("ProgressBar")]
    public void IsContainer_WithNonContainerControls_ReturnsFalse(string controlName)
    {
        // Act
        var result = MappingHelpers.IsContainer(controlName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsContainer_WithInvalidControl_ReturnsFalse()
    {
        // Act
        var result = MappingHelpers.IsContainer("NonExistentControl");

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetEventMapping Tests (3 tests)

    [Fact]
    public void GetEventMapping_WithValidEvent_ReturnsEventMapping()
    {
        // Act
        var result = MappingHelpers.GetEventMapping("Button", "Accepting");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Accepting", result.EventName);
    }

    [Fact]
    public void GetEventMapping_WithInvalidControl_ReturnsNull()
    {
        // Act
        var result = MappingHelpers.GetEventMapping("NonExistentControl", "Accepting");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetEventMapping_WithInvalidEvent_ReturnsNull()
    {
        // Act
        var result = MappingHelpers.GetEventMapping("Button", "NonExistentEvent");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region TwoWay Binding Tests (4 tests)

    [Fact]
    public void GetTwoWayBinding_WithValidProperty_ReturnsBinding()
    {
        // Act
        var result = MappingHelpers.GetTwoWayBinding("TextField", "Text");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Text", result.PropertyName);
    }

    [Fact]
    public void GetTwoWayBinding_WithInvalidControl_ReturnsNull()
    {
        // Act
        var result = MappingHelpers.GetTwoWayBinding("NonExistentControl", "Text");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SupportsTwoWayBinding_WithBindableProperty_ReturnsTrue()
    {
        // Act
        var result = MappingHelpers.SupportsTwoWayBinding("TextField", "Text");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void SupportsTwoWayBinding_WithNonBindableProperty_ReturnsFalse()
    {
        // Act
        var result = MappingHelpers.SupportsTwoWayBinding("Label", "Text");

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetPropertyMapping Tests (4 tests)

    [Fact]
    public void GetPropertyMapping_WithControlSpecificProperty_ReturnsMapping()
    {
        // Act
        var result = MappingHelpers.GetPropertyMapping("IsDefault", "Button");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("IsDefault", result.PropertyName);
    }

    [Fact]
    public void GetPropertyMapping_WithCommonProperty_ReturnsMapping()
    {
        // Act
        var result = MappingHelpers.GetPropertyMapping("X");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("X", result.PropertyName);
        Assert.Equal("Terminal.Gui.ViewBase.Pos", result.TargetType);
    }

    [Fact]
    public void GetPropertyMapping_WithInvalidProperty_ReturnsNull()
    {
        // Act
        var result = MappingHelpers.GetPropertyMapping("NonExistentProperty");

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("X", "Terminal.Gui.ViewBase.Pos")]
    [InlineData("Y", "Terminal.Gui.ViewBase.Pos")]
    [InlineData("Width", "Terminal.Gui.ViewBase.Dim")]
    [InlineData("Height", "Terminal.Gui.ViewBase.Dim")]
    public void GetPropertyMapping_PositioningProperties_ReturnCorrectTypes(string propertyName, string expectedType)
    {
        // Act
        var result = MappingHelpers.GetPropertyMapping(propertyName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedType, result.TargetType);
    }

    #endregion

    #region IsKnownEvent Tests (2 tests)

    [Theory]
    [InlineData("Accepting", true)]
    [InlineData("Activated", true)]
    [InlineData("NonExistentEvent", false)]
    public void IsKnownEvent_VariousEvents_ReturnsExpectedResult(string eventName, bool expected)
    {
        // Act
        var result = MappingHelpers.IsKnownEvent(eventName);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsKnownEvent_EmptyString_ReturnsFalse()
    {
        // Act
        var result = MappingHelpers.IsKnownEvent("");

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Type Checking Tests (10 tests)

    [Theory]
    [InlineData("Enabled", true)]
    [InlineData("Visible", true)]
    [InlineData("CanFocus", true)]
    [InlineData("Text", false)]
    [InlineData("X", false)]
    public void IsBooleanProperty_VariousProperties_ReturnsExpectedResult(string propertyName, bool expected)
    {
        // Act
        var result = MappingHelpers.IsBooleanProperty(propertyName);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("SelectedItem", true)]
    [InlineData("TabStop", true)]
    [InlineData("Text", false)]
    [InlineData("Fraction", false)]
    public void IsIntProperty_VariousProperties_ReturnsExpectedResult(string propertyName, bool expected)
    {
        // Act
        var result = MappingHelpers.IsIntProperty(propertyName);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Fraction", true)]
    [InlineData("Text", false)]
    [InlineData("X", false)]
    public void IsFloatProperty_VariousProperties_ReturnsExpectedResult(string propertyName, bool expected)
    {
        // Act
        var result = MappingHelpers.IsFloatProperty(propertyName);

        // Assert
        Assert.Equal(expected, result);
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

    [Fact]
    public void GetFullyQualifiedType_WithValidProperty_ReturnsFullType()
    {
        // Act
        var result = MappingHelpers.GetFullyQualifiedType("X");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Terminal.Gui.ViewBase.Pos", result);
    }

    [Fact]
    public void GetFullyQualifiedType_WithInvalidProperty_ReturnsNull()
    {
        // Act
        var result = MappingHelpers.GetFullyQualifiedType("NonExistentProperty");

        // Assert
        Assert.Null(result);
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

    [Fact]
    public void GetPropertyMapping_WithEmptyString_ReturnsNull()
    {
        // Act
        var result = MappingHelpers.GetPropertyMapping("");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetEventMapping_WithEmptyStrings_ReturnsNull()
    {
        // Act
        var result = MappingHelpers.GetEventMapping("", "");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetTwoWayBinding_WithEmptyStrings_ReturnsNull()
    {
        // Act
        var result = MappingHelpers.GetTwoWayBinding("", "");

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
        var fullTypeName = MappingHelpers.GetFullTypeName(controlName);
        var isContainer = MappingHelpers.IsContainer(controlName);
        var eventMapping = MappingHelpers.GetEventMapping(controlName, eventName);
        var propertyMapping = MappingHelpers.GetPropertyMapping(propertyName, controlName);
        var isKnownEvent = MappingHelpers.IsKnownEvent(eventName);

        // Assert
        Assert.NotNull(fullTypeName);
        Assert.Equal("Terminal.Gui.Views.Button", fullTypeName);
        Assert.False(isContainer);
        Assert.NotNull(eventMapping);
        Assert.NotNull(propertyMapping);
        Assert.True(isKnownEvent);
    }

    [Fact]
    public void CompleteWorkflow_TextField_TwoWayBindingWorks()
    {
        // Arrange
        var controlName = "TextField";
        var propertyName = "Text";

        // Act
        var fullTypeName = MappingHelpers.GetFullTypeName(controlName);
        var supportsTwoWay = MappingHelpers.SupportsTwoWayBinding(controlName, propertyName);
        var twoWayBinding = MappingHelpers.GetTwoWayBinding(controlName, propertyName);
        var propertyMapping = MappingHelpers.GetPropertyMapping(propertyName, controlName);

        // Assert
        Assert.NotNull(fullTypeName);
        Assert.True(supportsTwoWay);
        Assert.NotNull(twoWayBinding);
        Assert.NotNull(propertyMapping);
        Assert.NotNull(twoWayBinding.ChangeEventName);
    }

    #endregion
}
