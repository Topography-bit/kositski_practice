namespace MessageClient;

public sealed class AuthRequest
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public sealed class AuthResponse
{
    public string UserName { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}

public sealed class ChatMessage
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

public sealed class SecureMessageRequest
{
    public string Message { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}
