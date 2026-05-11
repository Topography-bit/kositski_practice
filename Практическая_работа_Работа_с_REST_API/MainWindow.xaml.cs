using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace CurrencyConverterApp;

public partial class MainWindow : Window
{
    private static readonly string[] Currencies = ["USD", "EUR", "RUB", "GBP", "CNY", "JPY"];
    private readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(8)
    };

    public MainWindow()
    {
        InitializeComponent();
        BaseCurrencyComboBox.ItemsSource = Currencies;
        TargetCurrencyComboBox.ItemsSource = Currencies;
        BaseCurrencyComboBox.SelectedItem = "USD";
        TargetCurrencyComboBox.SelectedItem = "EUR";
    }

    private async void ConvertButton_Click(object sender, RoutedEventArgs e)
    {
        if (!decimal.TryParse(AmountTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount) &&
            !decimal.TryParse(AmountTextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out amount))
        {
            MessageBox.Show("Введите корректную сумму.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var baseCurrency = BaseCurrencyComboBox.SelectedItem?.ToString() ?? "USD";
        var targetCurrency = TargetCurrencyComboBox.SelectedItem?.ToString() ?? "EUR";

        try
        {
            StatusLabel.Text = "Загрузка курса...";
            var rates = await LoadRatesAsync(baseCurrency);
            if (!rates.TryGetValue(targetCurrency, out var rate))
            {
                throw new InvalidOperationException($"Курс {targetCurrency} не найден.");
            }

            var converted = amount * rate;
            ResultLabel.Text = $"{amount:N2} {baseCurrency} = {converted:N2} {targetCurrency}";
            StatusLabel.Text = "Курс получен через REST API.";
        }
        catch (Exception ex)
        {
            var fallback = GetFallbackRate(baseCurrency, targetCurrency);
            var converted = amount * fallback;
            ResultLabel.Text = $"{amount:N2} {baseCurrency} = {converted:N2} {targetCurrency}";
            StatusLabel.Text = $"API недоступен, использован локальный пример курса. Причина: {ex.Message}";
        }
    }

    private async Task<Dictionary<string, decimal>> LoadRatesAsync(string baseCurrency)
    {
        var url = $"https://api.exchangerate-api.com/v4/latest/{baseCurrency}";
        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<ExchangeRates>(stream);
        return data?.Rates ?? throw new InvalidOperationException("Пустой ответ API.");
    }

    private static decimal GetFallbackRate(string baseCurrency, string targetCurrency)
    {
        var usdRates = new Dictionary<string, decimal>
        {
            ["USD"] = 1m,
            ["EUR"] = 0.92m,
            ["RUB"] = 92m,
            ["GBP"] = 0.79m,
            ["CNY"] = 7.23m,
            ["JPY"] = 155m
        };

        return usdRates[targetCurrency] / usdRates[baseCurrency];
    }

    private sealed class ExchangeRates
    {
        [JsonPropertyName("base")]
        public string Base { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; } = [];
    }
}
