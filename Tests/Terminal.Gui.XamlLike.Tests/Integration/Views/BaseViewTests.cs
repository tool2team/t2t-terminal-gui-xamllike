using Xunit;
using System.Text;
using Terminal.Gui.ViewBase;

namespace Terminal.Gui.XamlLike.Tests.Integration.Views;

public abstract class BaseViewTests<T> where T : View
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    public string XamlName => $"{typeof(T).Name}Test.tui.xaml";
    public string XamlPath => Path.Combine(AppContext.BaseDirectory, "Integration", "Assets", XamlName);

    public string SnapshotName => $"{XamlName}.g.cs";
    public string SnapshotPath => Path.Combine(AppContext.BaseDirectory, "Integration", "Snapshots", SnapshotName);

    public string GeneratedSourcePath => Path.Combine(typeof(TuiXamlGenerator).Namespace!, typeof(TuiXamlGenerator).FullName!, SnapshotName);

    public BaseViewTests()
    {
    }

    public string GetXamlContent()
    {
        Assert.True(File.Exists(XamlPath), $"Missing test asset: {XamlPath}");
        return File.ReadAllText(XamlPath);
    }

    public string GetSnapshotContent()
    {
        Assert.True(File.Exists(SnapshotPath), $"Missing test snapshot: {SnapshotPath}");
        return File.ReadAllText(SnapshotPath);
    }

    [Fact]
    public async Task Should_Generate_From_Xaml_And_Match_Snapshot()
    {
        var generator = new Terminal.Gui.XamlLike.TuiXamlGenerator();

        // Minimal C# source for compilation to exist
        var sources = new[]
        {       
            ("Dummy.cs", """
            namespace TestNs;
            public partial class Dummy { }
            """)
        };

        var result = GeneratorTestHost.Run(
            incrementalGenerator: generator,
            sources: [],
            additionalFiles: new[] { (XamlName, GetXamlContent()) });

        // 1) Verify that at least one source was generated
        Assert.True(result.Results.Length > 0);

        var generated = result
            .Results
            .SelectMany(r => r.GeneratedSources)
            .ToList();

        Assert.NotEmpty(generated);

        // 2) Verify that the correct file (hintName) exists
        Assert.Contains(generated, g => g.HintName == SnapshotName);

        var file = generated.First(g => g.HintName == SnapshotName);
        var text = file!.SourceText.ToString();

        // 3) Verify snapshot match
        Assert.Equal(GetSnapshotContent(), text);
    }
}