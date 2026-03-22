# TODO - Terminal.Gui.XamlLike

## Architecture and Future Improvements

### 🎯 Mapping Versioning (HIGH PRIORITY)

**Current Problem:**
- Mappings are static (single `Mappings.cs` file)
- No handling of API changes between Terminal.Gui versions
- Difficult to know which properties are available in which version
- String-based types make correspondence complicated

**Proposed Solution: Versioned Mappings**

#### Architecture
```
Terminal.Gui.MetadataExtractor (net10.0)
  → References Terminal.Gui
  → Extracts metadata with typeof()
  → Generates Mappings.cs with versioning metadata

Terminal.Gui.XamlLike Generator (netstandard2.0)
  → Reads Mappings.cs
  → Detects Terminal.Gui version via IAssemblySymbol
  → Filters properties according to detected version
  → Generates diagnostics if property not available
```

#### Proposed Data Model

```csharp
public class PropertyMapping
{
    public string PropertyName { get; }
    public string TargetType { get; }
    public string TargetTypeFullName { get; }        // For validation
    public bool IsNullable { get; }
    public string? MinVersion { get; }                // "2.0", "2.1", etc.
    public string? MaxVersion { get; }                // null = always valid
    public bool IsObsolete { get; }
    public string? ObsoleteMessage { get; }

    public bool IsAvailableInVersion(Version version)
    {
        if (MinVersion != null)
        {
            var min = Version.Parse(MinVersion);
            if (version < min) return false;
        }

        if (MaxVersion != null)
        {
            var max = Version.Parse(MaxVersion);
            if (version >= max) return false;
        }

        return true;
    }
}
```

#### Version Detection in Generator

```csharp
public static class VersionDetector
{
    public static Version? DetectTerminalGuiVersion(Compilation compilation)
    {
        var terminalGuiAssembly = compilation.References
            .Select(r => compilation.GetAssemblyOrModuleSymbol(r))
            .OfType<IAssemblySymbol>()
            .FirstOrDefault(a => a.Name == "Terminal.Gui");

        return terminalGuiAssembly?.Identity.Version;
    }

    public static ApiCapabilities DetectCapabilities(IAssemblySymbol assembly)
    {
        return new ApiCapabilities
        {
            HasCheckState = assembly.GetTypeByMetadataName("Terminal.Gui.Views.CheckState") != null,
            HasCommand = assembly.GetTypeByMetadataName("Terminal.Gui.Button")
                ?.GetMembers("Command").Any() ?? false,
            HasBindingContext = assembly.GetTypeByMetadataName("Terminal.Gui.View")
                ?.GetMembers("BindingContext").Any() ?? false
        };
    }
}
```

#### New Diagnostics to Add

```csharp
// TUI013: Property not available in this version
public static readonly DiagnosticDescriptor PropertyNotAvailableInVersion = new(
    id: "TUI013",
    title: "Property not available in Terminal.Gui version",
    messageFormat: "Property '{0}' is not available in Terminal.Gui version {1}. Minimum version required: {2}",
    category: Category,
    defaultSeverity: DiagnosticSeverity.Error,
    isEnabledByDefault: true);

// TUI014: Property obsolete
public static readonly DiagnosticDescriptor PropertyObsolete = new(
    id: "TUI014",
    title: "Property is obsolete",
    messageFormat: "Property '{0}' is obsolete in Terminal.Gui version {1}: {2}",
    category: Category,
    defaultSeverity: DiagnosticSeverity.Warning,
    isEnabledByDefault: true);
```

#### Implementation Tasks

1. [ ] Modify `PropertyMapping` to add versioning fields
2. [ ] Update MetadataExtractor to detect version information
3. [ ] Create `VersionDetector` in generator
4. [ ] Add diagnostics TUI013, TUI014
5. [ ] Filter properties by version in `Generator.cs`
6. [ ] Add tests for different versions
7. [ ] Document the versioning system

---

## Other TODOs

### Missing Features

- [ ] Support for implicit collections (add children without Content tag)
- [ ] Support for x:Type for generic types
- [ ] Support for ResourceDictionary
- [ ] Support for Styles
- [ ] Support for Triggers
- [ ] Improve binding validation (type checking)
- [ ] Support for DataTemplates
- [ ] Support for converters in bindings
- [ ] Support for MultiBinding
- [ ] Support for attached properties

### Code Quality

- [ ] Add more edge case tests
- [ ] Improve diagnostic messages
- [ ] Add logging/tracing system
- [ ] Performance profiling on large files

### Documentation

- [ ] Complete user guide
- [ ] Developer guide
- [ ] Migration guides
- [ ] Advanced examples

### Tooling

- [ ] PowerShell script for automatic mapping regeneration
- [ ] CI/CD for NuGet publishing
- [ ] Terminal.Gui version compatibility analyzer

---

## Examples and Testing

### Example XAML Files

- [ ] Create comprehensive example showing all supported features
- [ ] Add examples for different Terminal.Gui versions
- [ ] Examples for common patterns (dialogs, forms, menus)
- [ ] Performance test with large XAML files (1000+ elements)

### Integration Testing

- [ ] Test with real Terminal.Gui applications
- [ ] Test generator output compiles correctly
- [ ] Test generated code matches expected behavior
- [ ] Test error messages are helpful

---

## Known Issues

### Current Limitations

1. **No version detection yet** - All Terminal.Gui versions treated equally
2. **Command property issues** - Examples fail with Command property errors (Terminal.Gui 2.1+ required)
3. **Limited type validation** - String-based types make validation harder
4. **No implicit collections** - Must use explicit Content tags

### Planned Fixes

- [ ] Implement versioning system (see HIGH PRIORITY section above)
- [ ] Fix Command property examples
- [ ] Improve type validation with ITypeSymbol
- [ ] Add support for implicit collections

---

## Performance and Optimization

### Current Performance Characteristics

- **Build-time generation** - No runtime overhead
- **Incremental generation** - Only regenerates changed files
- **Caching** - Roslyn caches parsed syntax trees

### Optimization Opportunities

- [ ] Cache PropertyMapping lookups
- [ ] Optimize string concatenation in code generation
- [ ] Reduce allocations in parsing
- [ ] Profile with BenchmarkDotNet
- [ ] Measure generator execution time

### Benchmarking Plan

1. **Small XAML files** (<100 lines) - Baseline performance
2. **Medium XAML files** (100-500 lines) - Typical usage
3. **Large XAML files** (500-2000 lines) - Stress test
4. **Many XAML files** (100+ files) - Solution-level performance

Target: <100ms per file, <1s for full solution

---

