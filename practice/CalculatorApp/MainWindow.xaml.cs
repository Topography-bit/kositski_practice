using System;
using System.Windows;
using System.Windows.Controls;
namespace CalculatorApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // Метод для получения чисел из текстовых полей
        private bool TryGetNumbers(out double first, out double second)
        {
            first = 0;
            second = 0;
            // Пытаемся преобразовать текст в числа
            if (!double.TryParse(FirstNumberTextBox.Text, out first))
            {
                MessageBox.Show("Введите корректное первое число!",
                "Ошибка ввода",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
                return false;
            }
            if (!double.TryParse(SecondNumberTextBox.Text, out second))
            {
                MessageBox.Show("Введите корректное второе число!",
                "Ошибка ввода",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        private bool TryGetFirst(out double first)
        {
            first = 0;
            // Пытаемся преобразовать текст в числа
            if (!double.TryParse(FirstNumberTextBox.Text, out first))
            {
                MessageBox.Show("Введите корректное первое число!",
                "Ошибка ввода",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        // Обработчик кнопки сложения
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (TryGetNumbers(out double first, out double second))
            {
                double result = first + second;
                ResultTextBox.Text = result.ToString();
                History.Items.Add($"{first} + {second} = {result}");
            }
        }
        // Обработчик кнопки вычитания
        private void SubtractButton_Click(object sender, RoutedEventArgs
       e)
        {
            if (TryGetNumbers(out double first, out double second))
            {
                double result = first - second;
                ResultTextBox.Text = result.ToString();
                History.Items.Add($"{first} - {second} = {result}");
            }
        }
        // Обработчик кнопки умножения
        private void MultiplyButton_Click(object sender, RoutedEventArgs
       e)
        {
            if (TryGetNumbers(out double first, out double second))
            {
                double result = first * second;
                ResultTextBox.Text = result.ToString();
                History.Items.Add($"{first} * {second} = {result}");
            }
        }
        // Обработчик кнопки деления
        private void DivideButton_Click(object sender, RoutedEventArgs e)
        {
            if (TryGetNumbers(out double first, out double second))
            {
                // Проверка деления на ноль
                if (second == 0)
                {
                    MessageBox.Show("Деление на ноль невозможно!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                    return;
                }
                double result = first / second;
                ResultTextBox.Text = result.ToString();
                History.Items.Add($"{first} / {second} = {result}");
            }
        }
        // Обработчик кнопки очистки
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            FirstNumberTextBox.Text = string.Empty;
            SecondNumberTextBox.Text = string.Empty;
            ResultTextBox.Text = string.Empty;
            FirstNumberTextBox.Focus();
        }

        private void Degree_Click(object sender, RoutedEventArgs e)
        {
            if (TryGetNumbers(out double first, out double second))
            {
                double result = Math.Pow(first, second);
                ResultTextBox.Text = result.ToString();
                History.Items.Add($"{first}^{second} = {result}");
            }
        }

        private void sqrt_Click(object sender, RoutedEventArgs e)
        {
            if (TryGetFirst(out double first)){
                double result = Math.Sqrt(first);
                ResultTextBox.Text = result.ToString();
                History.Items.Add($"√{first} = {result}");
            }
        }
    }
}