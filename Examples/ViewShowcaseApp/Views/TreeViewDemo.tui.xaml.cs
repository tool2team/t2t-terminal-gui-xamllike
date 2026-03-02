using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace ViewShowcaseApp.Views;

public partial class TreeViewDemo : View
{
    public TreeViewDemo()
    {
        InitializeComponent();

        // Create sample tree structure
        TreeNode root = new TreeNode("Root Folder");

        TreeNode documentsNode = new TreeNode("Documents");
        documentsNode.Children.Add(new TreeNode("report.pdf"));
        documentsNode.Children.Add(new TreeNode("presentation.pptx"));
        documentsNode.Children.Add(new TreeNode("notes.txt"));

        TreeNode projectsNode = new TreeNode("Projects");
        TreeNode project1 = new TreeNode("Project A");
        project1.Children.Add(new TreeNode("source.cs"));
        project1.Children.Add(new TreeNode("README.md"));
        projectsNode.Children.Add(project1);

        TreeNode project2 = new TreeNode("Project B");
        project2.Children.Add(new TreeNode("main.cpp"));
        project2.Children.Add(new TreeNode("CMakeLists.txt"));
        projectsNode.Children.Add(project2);

        TreeNode downloadsNode = new TreeNode("Downloads");
        downloadsNode.Children.Add(new TreeNode("file1.zip"));
        downloadsNode.Children.Add(new TreeNode("file2.iso"));
        
        root.Children.Add(documentsNode);
        root.Children.Add(projectsNode);
        root.Children.Add(downloadsNode);
        
        // Set the tree root
        FileTree.AddObject(root);
        FileTree.ExpandAll();
    }

    private void OnSelectionChanged(object? sender, EventArgs e)
    {
        ITreeNode selected = FileTree.SelectedObject;
        if (selected is TreeNode node)
        {
            StatusLabel.Text = $"Selected: {node.Text}";
        }
    }

    private void OnObjectActivated(object? sender, ObjectActivatedEventArgs<ITreeNode> e)
    {
        ITreeNode selected = FileTree.SelectedObject;
        if (selected is TreeNode node)
        {
            StatusLabel.Text = $"Activated: {node.Text}";
        }
    }
}
