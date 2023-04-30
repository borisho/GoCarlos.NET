using System.Windows.Controls;
using System.Windows.Input;

namespace GoCarlos.NET.Views;

public partial class WallistUserControl : UserControl
{
    public WallistUserControl()
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
