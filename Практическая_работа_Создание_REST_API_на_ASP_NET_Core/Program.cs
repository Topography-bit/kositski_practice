var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<MessagesApi.Services.InMemoryMessageStore>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();
