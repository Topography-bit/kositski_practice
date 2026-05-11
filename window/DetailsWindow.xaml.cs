using System.Windows;

namespace WpfOrderCalculator;

public partial class DetailsWindow : Window
{
    public DetailsWindow(IReadOnlyList<string> items)
    {
        InitializeComponent();
        SetItems(items);
    }

    public void SetItems(IReadOnlyList<string> items)
    {
        ItemsListBox.ItemsSource = null;
        ItemsListBox.ItemsSource = items;
    }
}
