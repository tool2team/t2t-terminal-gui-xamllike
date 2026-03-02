using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Terminal.Gui.XamlLike.Tests;

internal sealed class InMemoryAdditionalText : AdditionalText
{
    private readonly SourceText _text;

    public InMemoryAdditionalText(string path, string content)
    {
        Path = path;
        _text = SourceText.From(content);
    }

    public override string Path { get; }

    public override SourceText GetText(System.Threading.CancellationToken cancellationToken = default) => _text;
}
