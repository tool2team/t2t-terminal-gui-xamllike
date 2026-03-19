using Terminal.Gui.Views;

namespace Terminal.Gui.XamlLike.Tests.Integration.Views;

/// <summary>
/// Partial class for FileDialogTest test view
/// </summary>
public partial class FileDialogTest : FileDialog
{
    public FileDialogTest()
    {
        InitializeComponent();
        AllowedTypes = new List<IAllowedType>() { new AllowedType("Plain text", ".txt", ".md") };
    }
}
