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
// TUI021: Property not available in this version
public static readonly DiagnosticDescriptor PropertyNotAvailableInVersion = new(
    id: "TUI021",
    title: "Property not available in Terminal.Gui version",
    messageFormat: "Property '{0}' is not available in Terminal.Gui version {1}. Minimum version required: {2}",
    category: Category,
    defaultSeverity: DiagnosticSeverity.Error,
    isEnabledByDefault: true);

// TUI022: Property obsolete
public static readonly DiagnosticDescriptor PropertyObsolete = new(
    id: "TUI022",
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
4. [ ] Add diagnostics TUI021 and TUI022
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

## Architecture Decisions

### ✅ Decision 1: Keep netstandard2.0 for Generator

**Date:** 2026-03-16

**Reasons:**
- Maximum IDE compatibility (VS 2019/2022/2026, Rider, VSCode)
- No version conflicts with user's Terminal.Gui
- Best practice for Source Generators
- MetadataExtractor (net10.0) does real validation with typeof()

**Rejected Alternative:** Multi-target or full net10.0
- Increased build complexity
- No real benefit (Roslyn API sufficient)
- Potential version coupling issues

### ✅ Decision 2: Serialized Types in Mappings

**Date:** 2026-03-16

**Reasons:**
- Source Generators cannot load external assemblies
- ITypeSymbol from Roslyn sufficient for validation
- Enables easy versioning
- Complete independence from Terminal.Gui

**Rejected Alternative:** Use Type directly
- Impossible in netstandard2.0 Source Generator
- Would create version dependencies

### ✅ Decision 3: Roslyn Handles Message Formatting

**Date:** 2026-03-16

**Reasons:**
- Roslyn natively handles formatting via Diagnostic.Create
- No FormatException risk in our code
- Arguments preserved for inspection
- Lazy evaluation of formatting

**Consequences:**
- TuiDiagnostic stores MessageArgs
- ToDiagnostic() passes messageArgs to Roslyn
- Simpler, cleaner, more performant

---

## Session Notes (2026-03-16)

### Improvements Made Today

1. ✅ **Location Collection for Diagnostics**
   - TuiDiagnostic stores LineNumber and LinePosition
   - ToDiagnostic() creates real Roslyn Location
   - All validation diagnostics pass positions

2. ✅ **No Message Pre-formatting Architecture**
   - TuiDiagnostic stores MessageArgs instead of formatted string
   - Create() does NO formatting - just stores args
   - Roslyn does all formatting in Diagnostic.Create()

3. ✅ **Use prop.IsNullable Instead of Guessing**
   - Added isNullable parameter to manual overrides
   - CheckState correctly marked as non-nullable

4. ✅ **PropertyMetadata Cleanup**
   - Removed 4 unused properties
   - Only IsNullable retained

### Documentation Created

- ✅ DIAGNOSTIC_LOCATION_ENHANCEMENT.md
- ✅ DIAGNOSTIC_MESSAGE_FORMATTING.md
- ✅ FINAL_DIAGNOSTIC_ARCHITECTURE.md
- ✅ TODO.md (this file)

### Tests

- ✅ 36 tests in XamlParserTests (all passing)
- ✅ 42 tests in MappingHelpersTests (all passing)
- ✅ New test: Validate_DiagnosticsIncludeLocation

### Files Modified

- Terminal.Gui.XamlLike\XmlParser.cs
- Terminal.Gui.XamlLike\Diagnostics.cs
- Terminal.Gui.XamlLike\MappingModels.cs
- Tools\Terminal.Gui.MetadataExtractor\Program.cs
- Tests\Terminal.Gui.XamlLike.Tests\Unit\XamlParserTests.cs

---

## Diagnostic System

### Current Diagnostics (15 active)

| ID | Severity | Description |
|----|----------|-------------|
| TUI001 | Error | Invalid XML structure |
| TUI002 | Error | Missing x:Class attribute |
| TUI003 | Error | Unknown control type |
| TUI004 | Error | Invalid binding syntax |
| TUI005 | Error | Unsupported two-way binding |
| TUI006 | Error | Invalid property value |
| TUI007 | Error | Empty event handler |
| TUI008 | Error | Unknown property |
| TUI009 | Error | Duplicate x:Name |
| TUI011 | Warning | Empty binding path |
| TUI014 | Error | Invalid x:Name (must be valid C# identifier) |
| TUI015 | Error | Invalid property name |
| TUI016 | Error | Invalid binding mode |
| TUI018 | Error | x:Class must have at least one dot |
| TUI019 | Error | x:Class namespace cannot be empty |
| TUI020 | Error | x:Class class name cannot be empty |

### Planned Diagnostics

| ID | Severity | Description | Priority |
|----|----------|-------------|----------|
| TUI021 | Error | Property not available in Terminal.Gui version | HIGH |
| TUI022 | Warning | Property is obsolete | HIGH |
| TUI023 | Error | Property removed in Terminal.Gui version | HIGH |
| TUI024 | Warning | Property type mismatch | MEDIUM |
| TUI025 | Error | Required property missing | MEDIUM |
| TUI026 | Warning | Performance: too many bindings | LOW |

### Diagnostic Infrastructure

✅ **Completed Improvements:**
- Line-precise error reporting with LineNumber and LinePosition
- Location creation with LinePositionSpan (0-based coordinates)
- Roslyn-only message formatting (no pre-formatting)
- MessageArgs stored for inspection
- All diagnostics tested

🔧 **Future Improvements:**
- Add quick fixes for common issues
- Add code actions (e.g., "Add missing property")
- Improve diagnostic messages with suggestions
- Add telemetry for diagnostic frequency

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

## Release Planning

### Version 1.0 (Current - Pre-release)

- ✅ Basic XAML parsing
- ✅ Property and event binding
- ✅ Code generation
- ✅ 15 diagnostics with locations
- ✅ Basic type validation
- ⏳ Documentation

### Version 1.1 (Next Release)

- [ ] **Versioning system** (HIGH PRIORITY)
- [ ] TUI021, TUI022, TUI023 diagnostics
- [ ] Improved error messages
- [ ] Complete user guide
- [ ] Fix Command property examples

### Version 2.0 (Future)

- [ ] Implicit collections
- [ ] ResourceDictionary
- [ ] Styles and Triggers
- [ ] DataTemplates
- [ ] Advanced binding features

---

**Last Updated:** 2026-03-16
