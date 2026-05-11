using MessagesApi.Models;

namespace MessagesApi.Services;

public sealed class InMemoryMessageStore
{
    private readonly object _sync = new();
    private readonly Dictionary<string, UserRecord> _users = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _tokens = new(StringComparer.Ordinal);
    private readonly List<MessageDto> _messages = [];
    private int _nextMessageId = 1;

    public AuthResponse Register(string userName, string password)
    {
        userName = Normalize(userName);
        password = password.Trim();

        if (password.Length < 3)
        {
            throw new InvalidOperationException("Пароль должен содержать минимум 3 символа.");
        }

        lock (_sync)
        {
            if (_users.ContainsKey(userName))
            {
                throw new InvalidOperationException("Пользователь уже существует.");
            }

            _users[userName] = new UserRecord(userName, password);
            return CreateToken(userName);
        }
    }

    public AuthResponse Login(string userName, string password)
    {
        userName = Normalize(userName);
        password = password.Trim();

        lock (_sync)
        {
            if (!_users.TryGetValue(userName, out var user) || user.Password != password)
            {
                throw new InvalidOperationException("Неверное имя пользователя или пароль.");
            }

            return CreateToken(userName);
        }
    }

    public IReadOnlyList<MessageDto> GetMessages()
    {
        lock (_sync)
        {
            return _messages.Select(Clone).ToList();
        }
    }

    public MessageDto AddLegacyMessage(string userName, string text)
    {
        userName = Normalize(userName);
        return AddMessage(userName, text);
    }

    public MessageDto AddSecureMessage(string token, string text)
    {
        lock (_sync)
        {
            if (!_tokens.TryGetValue(token, out var userName))
            {
                throw new UnauthorizedAccessException("Токен не найден или устарел.");
            }

            return AddMessageUnsafe(userName, text);
        }
    }

    private MessageDto AddMessage(string userName, string text)
    {
        lock (_sync)
        {
            return AddMessageUnsafe(userName, text);
        }
    }

    private MessageDto AddMessageUnsafe(string userName, string text)
    {
        text = text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException("Сообщение не может быть пустым.");
        }

        var message = new MessageDto
        {
            Id = _nextMessageId++,
            UserName = userName,
            Text = text,
            CreatedAt = DateTime.Now
        };
        _messages.Add(message);
        return Clone(message);
    }

    private AuthResponse CreateToken(string userName)
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", string.Empty)
            .Replace("/", string.Empty)
            .Replace("=", string.Empty);
        _tokens[token] = userName;
        return new AuthResponse { UserName = userName, Token = token };
    }

    private static string Normalize(string value)
    {
        value = value.Trim();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("Имя пользователя обязательно.");
        }

        return value;
    }

    private static MessageDto Clone(MessageDto message)
    {
        return new MessageDto
        {
            Id = message.Id,
            UserName = message.UserName,
            Text = message.Text,
            CreatedAt = message.CreatedAt
        };
    }

    private sealed record UserRecord(string UserName, string Password);
}
