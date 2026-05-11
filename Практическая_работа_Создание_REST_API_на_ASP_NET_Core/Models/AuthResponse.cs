namespace MessagesApi.Models;

public sealed class AuthResponse
{
    public string UserName { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}
