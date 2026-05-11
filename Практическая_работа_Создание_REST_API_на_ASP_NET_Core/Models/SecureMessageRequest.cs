namespace MessagesApi.Models;

public sealed class SecureMessageRequest
{
    public string Message { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}
