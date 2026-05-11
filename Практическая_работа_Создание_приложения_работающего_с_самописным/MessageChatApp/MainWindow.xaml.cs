using System.IO;
using System.Windows;
using System.Windows.Threading;
using MessageClient;

namespace MessageChatApp;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _refreshTimer = new() { Interval = TimeSpan.FromSeconds(3) };
    private MessageApiClient? _client;

    private static readonly string UserNameFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MessageChatApp",
        "username.txt");

    public MainWindow()
    {
        InitializeComponent();
        LoadSavedUserName();
        _refreshTimer.Tick += async (_, _) => await RefreshMessagesAsync();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        await AuthenticateAsync(register: false);
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        await AuthenticateAsync(register: true);
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await RefreshMessagesAsync();
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        if (!SessionStore.IsAuthenticated || _client is null)
        {
            MessageBox.Show("Сначала выполните вход.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var text = MessageTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        try
        {
            await _client.SendMessageAsync(SessionStore.Token, text);
            MessageTextBox.Clear();
            await RefreshMessagesAsync();
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Ошибка отправки: {ex.Message}";
        }
    }

    private async Task AuthenticateAsync(bool register)
    {
        var userName = UserNameTextBox.Text.Trim();
        var password = PasswordBox.Password;

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Введите имя пользователя и пароль.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            CreateClient();
            var response = register
                ? await _client!.RegisterAsync(userName, password)
                : await _client!.LoginAsync(userName, password);

            SessionStore.Set(response);
            SaveUserName(response.UserName);
            SessionLabel.Text = $"Вход выполнен: {response.UserName}";
            MessageTextBox.IsEnabled = true;
            StatusLabel.Text = register ? "Пользователь зарегистрирован, токен получен." : "Токен получен.";
            _refreshTimer.Start();
            await RefreshMessagesAsync();
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Ошибка авторизации: {ex.Message}";
        }
    }

    private async Task RefreshMessagesAsync()
    {
        if (_client is null)
        {
            return;
        }

        try
        {
            var messages = await _client.GetMessagesAsync();
            MessagesListBox.ItemsSource = messages
                .OrderBy(message => message.Id)
                .Select(message => new MessageViewModel(message))
                .ToList();

            if (messages.Count > 0)
            {
                MessagesListBox.ScrollIntoView(MessagesListBox.Items[MessagesListBox.Items.Count - 1]);
            }
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Ошибка обновления: {ex.Message}";
        }
    }

    private void CreateClient()
    {
        _client?.Dispose();
        _client = new MessageApiClient(ApiUrlTextBox.Text);
    }

    private void LoadSavedUserName()
    {
        if (!File.Exists(UserNameFile))
        {
            return;
        }

        var userName = File.ReadAllText(UserNameFile).Trim();
        if (string.IsNullOrWhiteSpace(userName))
        {
            return;
        }

        UserNameTextBox.Text = userName;
        UserNameTextBox.IsEnabled = false;
        UserNameLockLabel.Text = "Имя сохранено. Чтобы сменить его, удалите файл username.txt в AppData\\MessageChatApp.";
    }

    private static void SaveUserName(string userName)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(UserNameFile)!);
        File.WriteAllText(UserNameFile, userName);
    }

    protected override void OnClosed(EventArgs e)
    {
        _refreshTimer.Stop();
        _client?.Dispose();
        base.OnClosed(e);
    }

    private sealed class MessageViewModel(ChatMessage message)
    {
        public int Id { get; } = message.Id;

        public string UserName { get; } = message.UserName;

        public string Text { get; } = message.Text;

        public string CreatedAtText { get; } = message.CreatedAt.ToString("dd.MM.yyyy HH:mm:ss");
    }
}
