using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 80 portu dinle
builder.WebHost.UseUrls("http://0.0.0.0:80");

// Ocelot config dosyasýný environment’a göre seç
var ocelotConfig = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Docker"
    ? "ocelot.docker.json"
    : "ocelot.local.json";

builder.Configuration.AddJsonFile(ocelotConfig, optional: false, reloadOnChange: true);

builder.Services.AddOcelot();

var app = builder.Build();
await app.UseOcelot();

app.Run();
