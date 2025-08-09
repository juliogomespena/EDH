using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace EDH.Presentation.Common.Resources.Behaviors;

public static class DataGridRefreshBehavior
{
    public static readonly DependencyProperty EnableRefreshProperty =
        DependencyProperty.RegisterAttached(
            "EnableRefresh",
            typeof(bool),
            typeof(DataGridRefreshBehavior),
            new PropertyMetadata(false, OnEnableRefreshChanged)
        );

    public static bool GetEnableRefresh(DependencyObject obj) =>
        (bool)obj.GetValue(EnableRefreshProperty);

    public static void SetEnableRefresh(DependencyObject obj, bool value) =>
        obj.SetValue(EnableRefreshProperty, value);

    private static void OnEnableRefreshChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DataGrid grid)
        {
            if ((bool)e.NewValue)
                grid.RowEditEnding += Grid_RowEditEnding;
            else
                grid.RowEditEnding -= Grid_RowEditEnding;
        }
    }

    private static void Grid_RowEditEnding(object? sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction != DataGridEditAction.Commit || sender is null) return;

        var dg = (DataGrid)sender;
        // Defer so WPF finishes its own commit cycle first
        dg.Dispatcher.BeginInvoke(new Action(() =>
            {
                dg.CommitEdit(DataGridEditingUnit.Row, true);
                dg.Items.Refresh();
            }),
            DispatcherPriority.Background);
    }
}