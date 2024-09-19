var builder = WebApplication.CreateBuilder(args);

Startup.ConfigureServices(builder.Services);

var app = builder.Build();

Startup.Configure(app, app.Environment);

app.Run();
