using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

namespace Terminal.Gui.XamlLike.Tests;

internal static class GeneratorTestHost
{
    public static GeneratorDriverRunResult Run(
        IIncrementalGenerator incrementalGenerator,
        IEnumerable<(string filename, string content)> sources,
        IEnumerable<(string filename, string content)> additionalFiles)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.CSharp10);

        var syntaxTrees = sources.Select(s =>
            CSharpSyntaxTree.ParseText(s.content, parseOptions, path: s.filename));

        // Références minimales : mscorlib + System.Runtime + LINQ
        var refs = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.GCSettings).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        };

        var compilation = CSharpCompilation.Create(
            assemblyName: "GeneratorTests",
            syntaxTrees: syntaxTrees,
            references: refs,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var addTexts = additionalFiles
            .Select(f => (AdditionalText)new InMemoryAdditionalText(f.filename, f.content))
            .ToImmutableArray();

        // Adapter IIncrementalGenerator -> ISourceGenerator
        ISourceGenerator sourceGenerator = incrementalGenerator.AsSourceGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: new[] { sourceGenerator },
            additionalTexts: addTexts,
            parseOptions: parseOptions);

        driver = driver.RunGenerators(compilation);

        return driver.GetRunResult();
    }
}
