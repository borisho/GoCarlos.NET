using System.Windows.Controls;
using System.Windows.Input;

namespace GoCarlos.NET.Views;

public partial class PairingsUserControl : UserControl
{
    public PairingsUserControl()
    {
        InitializeComponent();
    }

    private void DataGrid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is ScrollViewer)
        {
            ((DataGrid)sender).UnselectAll();
        }
    }
}
