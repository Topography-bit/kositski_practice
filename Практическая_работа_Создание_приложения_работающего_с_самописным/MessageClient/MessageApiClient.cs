using System.Net.Http.Json;
using System.Text.Json;

namespace MessageClient;

public sealed class MessageApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _ownsClient;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public MessageApiClient(string baseAddress)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(NormalizeBaseAddress(baseAddress)),
            Timeout = TimeSpan.FromSeconds(8)
        };
        _ownsClient = true;
    }

    public MessageApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponse> RegisterAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        return await SendAuthAsync("api/auth/register", userName, password, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        return await SendAuthAsync("api/auth/login", userName, password, cancellationToken);
    }

    public async Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _httpClient.GetFromJsonAsync<List<ChatMessage>>("api/messages", JsonOptions, cancellationToken);
        return messages ?? [];
    }

    public async Task<ChatMessage> SendMessageAsync(string token, string message, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/messages/secure",
            new SecureMessageRequest { Token = token, Message = message },
            JsonOptions,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ChatMessage>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("API вернул пустое сообщение.");
    }

    private async Task<AuthResponse> SendAuthAsync(string url, string userName, string password, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(
            url,
            new AuthRequest { UserName = userName, Password = password },
            JsonOptions,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("API вернул пустой ответ авторизации.");
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new InvalidOperationException(string.IsNullOrWhiteSpace(payload)
            ? $"Ошибка API: {(int)response.StatusCode}"
            : payload);
    }

    private static string NormalizeBaseAddress(string value)
    {
        value = string.IsNullOrWhiteSpace(value) ? "http://localhost:5247" : value.Trim();
        return value.EndsWith('/') ? value : value + "/";
    }

    public void Dispose()
    {
        if (_ownsClient)
        {
            _httpClient.Dispose();
        }
    }
}
