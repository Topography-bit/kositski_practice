using System.Windows;

namespace WpfOrderCalculator;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenOrderButton_Click(object sender, RoutedEventArgs e)
    {
        var clientName = ClientNameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(clientName))
        {
            MessageBox.Show("Введите имя клиента.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
            ClientNameTextBox.Focus();
            return;
        }

        var orderWindow = new OrderWindow(clientName)
        {
            Owner = this
        };

        var result = orderWindow.ShowDialog();
        ResultLabel.Text = result == true
            ? $"Заказ подтвержден. Сумма: {orderWindow.TotalPrice:N0} руб."
            : "Заказ отменен.";
    }
}
