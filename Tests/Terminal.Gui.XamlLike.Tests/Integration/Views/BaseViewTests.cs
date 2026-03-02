using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using Terminal.Gui.ViewBase;

namespace Terminal.Gui.XamlLike.Tests.Integration.Views
{
    public class BaseViewTests<T> where T : View
    {
        // ✨ Même encodage que le generator
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public string XamlName => $"{typeof(T).Name}Test.tui.xaml";
        public string XamlPath => Path.Combine(AppContext.BaseDirectory, "Integration", "Assets", XamlName);

        public string SnapshotName => $"{XamlName}.g.cs";
        public string SnapshotPath => Path.Combine(AppContext.BaseDirectory, "Integration", "Snapshots", SnapshotName);

        public string GeneratedSourcePath => Path.Combine(typeof(TuiXamlGenerator).Namespace!, typeof(TuiXamlGenerator).FullName!, SnapshotName);

        public CSharpSourceGeneratorTest<TuiXamlGenerator, DefaultVerifier> SourceGeneratorTest { get; } = new();

        public BaseViewTests()
        {
            var xamlText = SourceText.From(GetXamlContent(), Utf8NoBom);
            SourceGeneratorTest.TestState.AdditionalFiles.Add((XamlName, xamlText));

            var snapshotText = SourceText.From(GetSnapshotContent(), Utf8NoBom);
            SourceGeneratorTest.TestState.GeneratedSources.Add((GeneratedSourcePath, snapshotText));
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
            await SourceGeneratorTest.RunAsync(TestContext.Current.CancellationToken);
        }

        [Fact]
        public async Task Should_Generate_Valid_Structure()
        {
            await SourceGeneratorTest.RunAsync(TestContext.Current.CancellationToken);

            var generated = SourceGeneratorTest.TestState.GeneratedSources
                .FirstOrDefault(gs => gs.filename.EndsWith(SnapshotName));

            Assert.NotNull(generated);

            var content = generated.content.ToString();
            var viewName = typeof(T).Name;

            Assert.Contains($"namespace Terminal.Gui.XamlLike.Tests.Integration.Xaml.{viewName}Test", content);
            Assert.Contains($"partial class {viewName}Test", content);
            Assert.Contains("private void InitializeComponent()", content);
        }
    }
}