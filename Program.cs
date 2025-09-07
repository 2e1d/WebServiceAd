using WebServiceAd.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPlatformSearcher, PlatformSearcher>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

app.Run();
