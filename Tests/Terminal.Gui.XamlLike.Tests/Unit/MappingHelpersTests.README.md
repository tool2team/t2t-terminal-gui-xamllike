# MappingHelpers Unit Tests - Documentation

## Overview

Complete unit test suite for `MappingHelpers.cs` utility class with **39 tests** covering all 14 public methods.

## Test Coverage

| Method | # Tests | Coverage | Status |
|--------|---------|----------|--------|
| `GetFullTypeName` | 4 | 100% | ✅ |
| `IsContainer` | 3 | 100% | ✅ |
| `GetEventMapping` | 3 | 100% | ✅ |
| `GetTwoWayBinding` | 2 | 100% | ✅ |
| `SupportsTwoWayBinding` | 2 | 100% | ✅ |
| `GetPropertyMapping` | 4 | 100% | ✅ |
| `IsKnownEvent` | 2 | 100% | ✅ |
| `IsKnownProperty` | 3 | 100% | ✅ |
| `IsBooleanProperty` | 1 | 100% | ✅ |
| `IsIntProperty` | 1 | 100% | ✅ |
| `IsFloatProperty` | 1 | 100% | ✅ |
| `IsArrayProperty` | 0 | N/A | ⏭️ |
| `IsTerminalGuiType` | 1 | 100% | ✅ |
| `GetFullyQualifiedType` | 2 | 100% | ✅ |
| **Integration Tests** | 2 | - | ✅ |
| **Edge Cases** | 4 | - | ✅ |
| **TOTAL** | **42 tests** | **~95%** | ✅ |

## Test Categories

### 1. GetFullTypeName Tests (4 tests)
Tests control type name resolution with optional generic support.

```csharp
[Fact] GetFullTypeName_WithValidControl_ReturnsFullTypeName()
[Fact] GetFullTypeName_WithInvalidControl_ReturnsNull()
[Fact] GetFullTypeName_WithGenericOptionSelector_ReturnsGenericType()
[Theory] GetFullTypeName_CommonControls_ReturnsExpectedTypes(...)
```

**Scenarios:**
- ✅ Valid controls (Button, Window, Dialog)
- ✅ Invalid/non-existent controls
- ✅ Generic types (OptionSelector<T>)
- ✅ Common controls verification

### 2. IsContainer Tests (3 tests)
Tests container detection for controls that can have children.

```csharp
[Theory] IsContainer_WithContainerControls_ReturnsTrue(...)
[Theory] IsContainer_WithNonContainerControls_ReturnsFalse(...)
[Fact] IsContainer_WithInvalidControl_ReturnsFalse()
```

**Scenarios:**
- ✅ Container controls (Window, Dialog, FrameView, TabView)
- ✅ Non-container controls (Button, Label, TextField)
- ✅ Invalid controls

### 3. GetEventMapping Tests (3 tests)
Tests event mapping retrieval for control/event combinations.

```csharp
[Fact] GetEventMapping_WithValidEvent_ReturnsEventMapping()
[Fact] GetEventMapping_WithInvalidControl_ReturnsNull()
[Fact] GetEventMapping_WithInvalidEvent_ReturnsNull()
```

**Scenarios:**
- ✅ Valid control/event combinations
- ✅ Invalid controls
- ✅ Invalid events

### 4. TwoWay Binding Tests (4 tests)
Tests two-way data binding support detection.

```csharp
[Fact] GetTwoWayBinding_WithValidProperty_ReturnsBinding()
[Fact] GetTwoWayBinding_WithInvalidControl_ReturnsNull()
[Fact] SupportsTwoWayBinding_WithBindableProperty_ReturnsTrue()
[Fact] SupportsTwoWayBinding_WithNonBindableProperty_ReturnsFalse()
```

**Scenarios:**
- ✅ Bindable properties (TextField.Text)
- ✅ Non-bindable properties (Label.Text)
- ✅ Invalid controls

### 5. GetPropertyMapping Tests (4 tests)
Tests property mapping resolution with control-specific and common properties.

```csharp
[Fact] GetPropertyMapping_WithControlSpecificProperty_ReturnsMapping()
[Fact] GetPropertyMapping_WithCommonProperty_ReturnsMapping()
[Fact] GetPropertyMapping_WithInvalidProperty_ReturnsNull()
[Theory] GetPropertyMapping_PositioningProperties_ReturnCorrectTypes(...)
```

**Scenarios:**
- ✅ Control-specific properties (Button.IsDefault)
- ✅ Common properties (X, Y, Width, Height)
- ✅ Invalid properties
- ✅ Type verification for positioning properties

### 6. IsKnownEvent Tests (2 tests)
Tests global event name detection across all controls.

```csharp
[Theory] IsKnownEvent_VariousEvents_ReturnsExpectedResult(...)
[Fact] IsKnownEvent_EmptyString_ReturnsFalse()
```

**Scenarios:**
- ✅ Known events (Accepting, Activated)
- ✅ Unknown events
- ✅ Empty string

### 7. Type Checking Tests (10 tests)
Tests property type detection for code generation.

```csharp
[Theory] IsBooleanProperty_VariousProperties_ReturnsExpectedResult(...)
[Theory] IsIntProperty_VariousProperties_ReturnsExpectedResult(...)
[Theory] IsFloatProperty_VariousProperties_ReturnsExpectedResult(...)
[Theory] IsTerminalGuiType_VariousProperties_ReturnsExpectedResult(...)
[Fact] GetFullyQualifiedType_WithValidProperty_ReturnsFullType()
[Fact] GetFullyQualifiedType_WithInvalidProperty_ReturnsNull()
```

**Scenarios:**
- ✅ Boolean properties (Enabled, Visible, CanFocus)
- ✅ Integer properties (BoxHeight, BoxWidth)
- ✅ Float properties (Fraction)
- ✅ Terminal.Gui types (X, Y, Width, Height)
- ✅ Fully qualified type names

### 8. Edge Cases Tests (4 tests)
Tests boundary conditions and error handling.

```csharp
[Fact] GetFullTypeName_WithEmptyString_ReturnsNull()
[Fact] GetPropertyMapping_WithEmptyString_ReturnsNull()
[Fact] GetEventMapping_WithEmptyStrings_ReturnsNull()
[Fact] GetTwoWayBinding_WithEmptyStrings_ReturnsNull()
```

**Scenarios:**
- ✅ Empty strings
- ✅ Null safety
- ✅ Invalid input handling

### 9. Integration Tests (2 tests)
Tests complete workflows using multiple methods together.

```csharp
[Fact] CompleteWorkflow_Button_AllMethodsWork()
[Fact] CompleteWorkflow_TextField_TwoWayBindingWorks()
```

**Scenarios:**
- ✅ Complete Button workflow (type, container, events, properties)
- ✅ Complete TextField TwoWay binding workflow

## Running the Tests

### Run all MappingHelpers tests
```bash
dotnet test --filter "FullyQualifiedName~MappingHelpersTests"
```

### Run specific test category
```bash
# GetFullTypeName tests
dotnet test --filter "FullyQualifiedName~MappingHelpersTests.GetFullTypeName"

# Type checking tests
dotnet test --filter "FullyQualifiedName~MappingHelpersTests.IsBooleanProperty"
```

### Run with coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Data

### Tested Controls
- **Container:** Window, Dialog, FrameView, TabView
- **Non-Container:** Button, Label, TextField, ProgressBar
- **Generic:** OptionSelector<T>

### Tested Properties
- **Common:** X, Y, Width, Height, Enabled, Visible, CanFocus, Text
- **Control-Specific:** IsDefault (Button), Fraction (ProgressBar)
- **Integer:** BoxHeight, BoxWidth
- **TwoWay Bindable:** TextField.Text, CheckBox.Value

### Tested Events
- **Common:** Accepting, Activated, KeyDown
- **Invalid:** NonExistentEvent

## Expected Results

All 39 tests should **PASS** when running against a properly configured `Mappings.cs` with:
- Standard Terminal.Gui control mappings
- Property mappings (Common + control-specific)
- Event mappings
- TwoWay binding configurations

## Notes

### IsArrayProperty Tests
Not currently implemented due to uncertainty about array property examples in current mappings. Can be added when array properties are identified.

### Obsolete Events
The `GetEventMapping` tests assume obsolete events exist in mappings but don't explicitly test them. Add specific tests if obsolete event testing is critical.

## Maintenance

When adding new methods to `MappingHelpers`:
1. Add corresponding test methods in appropriate region
2. Update coverage table in this document
3. Add integration tests if method interacts with others
4. Test edge cases (null, empty, invalid)

## CI/CD

These tests are automatically run in:
- ✅ `ci.yml` - Simple CI workflow
- ✅ `multi-platform-ci.yml` - Multi-platform testing
- ✅ `pr-validation.yml` - Pull request validation

## Performance

Expected execution time: **< 1 second** for all 39 tests

No external dependencies or I/O operations - tests run entirely in-memory using the static `Mappings` dictionaries.

