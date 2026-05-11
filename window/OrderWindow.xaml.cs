using System.Windows;
using System.Windows.Controls;

namespace WpfOrderCalculator;

public partial class OrderWindow : Window
{
    private readonly List<MenuItem> _menuItems = [];
    private DetailsWindow? _detailsWindow;

    public decimal TotalPrice { get; private set; }

    public OrderWindow(string clientName)
    {
        InitializeComponent();
        GreetingLabel.Text = $"Здравствуйте, {clientName}! Выберите позиции заказа.";
        _menuItems.Add(new MenuItem("Пицца Маргарита", 420, PizzaCheckBox));
        _menuItems.Add(new MenuItem("Паста Карбонара", 360, PastaCheckBox));
        _menuItems.Add(new MenuItem("Чай с десертом", 240, TeaCheckBox));
        UpdateTotal();
    }

    private void SelectionChanged(object sender, RoutedEventArgs e)
    {
        UpdateTotal();
        _detailsWindow?.SetItems(GetSelectedItems());
    }

    private void DetailsButton_Click(object sender, RoutedEventArgs e)
    {
        if (_detailsWindow is { IsVisible: true })
        {
            _detailsWindow.Activate();
            _detailsWindow.SetItems(GetSelectedItems());
            return;
        }

        _detailsWindow = new DetailsWindow(GetSelectedItems())
        {
            Owner = this,
            Left = Left + Width + 12,
            Top = Top
        };
        _detailsWindow.Closed += (_, _) => _detailsWindow = null;
        _detailsWindow.Show();
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        if (TotalPrice <= 0)
        {
            MessageBox.Show("Выберите хотя бы одну позицию.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void UpdateTotal()
    {
        TotalPrice = _menuItems.Where(item => item.CheckBox.IsChecked == true).Sum(item => item.Price);
        TotalLabel.Text = $"Сумма: {TotalPrice:N0} руб.";
    }

    private IReadOnlyList<string> GetSelectedItems()
    {
        var selected = _menuItems
            .Where(item => item.CheckBox.IsChecked == true)
            .Select(item => $"{item.Name} - {item.Price:N0} руб.")
            .ToList();

        return selected.Count > 0 ? selected : ["Позиции пока не выбраны"];
    }

    private sealed record MenuItem(string Name, decimal Price, CheckBox CheckBox);
}
