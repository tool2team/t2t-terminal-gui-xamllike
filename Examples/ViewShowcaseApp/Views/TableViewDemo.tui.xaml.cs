using System.Data;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace ViewShowcaseApp.Views;

public partial class TableViewDemo : View
{
    public TableViewDemo()
    {
        InitializeComponent();
        
        // Create a sample data table
        var dataTable = new DataTable();
        dataTable.Columns.Add("ID", typeof(int));
        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("Department", typeof(string));
        dataTable.Columns.Add("Salary", typeof(decimal));
        
        // Add sample data
        dataTable.Rows.Add(1, "John Doe", "Engineering", 75000);
        dataTable.Rows.Add(2, "Jane Smith", "Marketing", 65000);
        dataTable.Rows.Add(3, "Bob Johnson", "Sales", 60000);
        dataTable.Rows.Add(4, "Alice Williams", "Engineering", 80000);
        dataTable.Rows.Add(5, "Charlie Brown", "HR", 55000);
        dataTable.Rows.Add(6, "Diana Prince", "Engineering", 90000);
        dataTable.Rows.Add(7, "Eve Adams", "Marketing", 62000);
        dataTable.Rows.Add(8, "Frank Castle", "Security", 70000);
        
        // Set the table source
        EmployeeTable.Table = new DataTableSource(dataTable);
    }

    private void OnCellActivated(object? sender, CellActivatedEventArgs e)
    {
        var table = (DataTableSource)EmployeeTable.Table;
        var row = e.Row;
        var col = e.Col;
        
        if (row >= 0 && row < table.Rows && col >= 0 && col < table.Columns)
        {
            var value = table[row, col];
            var columnName = table.ColumnNames[col];
            StatusLabel.Text = $"Selected: {columnName} = {value} (Row {row + 1}, Col {col + 1})";
        }
    }
}
