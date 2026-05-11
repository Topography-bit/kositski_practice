namespace MessageClient;

public static class SessionStore
{
    public static string UserName { get; set; } = string.Empty;

    public static string Token { get; set; } = string.Empty;

    public static bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    public static void Set(AuthResponse response)
    {
        UserName = response.UserName;
        Token = response.Token;
    }

    public static void Clear()
    {
        UserName = string.Empty;
        Token = string.Empty;
    }
}
