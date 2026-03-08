# XamlParserTests - Documentation

Unit tests for `XamlParser.cs` - parsing and validation of `.tui.xaml` files.

## Summary

| Category | Tests |
|----------|-------|
| Parse - Valid XAML | 8 |
| Parse - Invalid XAML | 8 |
| Validate - Valid cases | 5 |
| Validate - Invalid cases | 7 |
| ParseResult | 3 |
| Edge cases | 4 |
| **Total** | **35** |

## Tested Methods

- `Parse(string, string)` - XAML parsing
- `Validate(XamlDocument)` - Semantic validation
- `ParseResult<T>` - Result wrapper

## Execution

```bash
# All tests
dotnet test --filter "FullyQualifiedName~XamlParserTests"

# Parse only
dotnet test --filter "FullyQualifiedName~XamlParserTests.Parse_"

# Validate only
dotnet test --filter "FullyQualifiedName~XamlParserTests.Validate_"
```

## Test Examples

### Parse - Valid case
```csharp
[Fact]
public void Parse_ValidSimpleXaml_ReturnsSuccess()
{
    var xaml = """<Button x:Class="Test.Button" Text="Click" />""";
    var result = XamlParser.Parse(xaml, "test.xaml");
    Assert.True(result.IsSuccess);
}
```

### Parse - Invalid case
```csharp
[Fact]
public void Parse_MissingXClass_ReturnsError()
{
    var xaml = """<Button Text="No Class" />""";
    var result = XamlParser.Parse(xaml, "test.xaml");
    Assert.False(result.IsSuccess);
}
```

### Validate - Error detection
```csharp
[Fact]
public void Validate_UnknownControl_ReturnsDiagnostic()
{
    var xaml = """<InvalidControl x:Class="Test.Custom" />""";
    var parseResult = XamlParser.Parse(xaml, "test.xaml");
    var diagnostics = XamlParser.Validate(parseResult.Value!);
    Assert.NotEmpty(diagnostics);
}
```

## Coverage

- ✅ Parsing complex hierarchies
- ✅ Attribute extraction (x:Class, x:DataType, x:Name)
- ✅ Binding detection ({Bind})
- ✅ Event handling
- ✅ XML errors (malformed, unclosed tags, etc.)
- ✅ Control type validation
- ✅ TwoWay binding validation
- ✅ Event validation

## Notes

- Execution time: < 2 seconds
- No external dependencies
- In-memory tests only


