namespace MessagesApi.Models;

public sealed class CreateMessageRequest
{
    public string UserName { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
}
