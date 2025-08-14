using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var ocelotConfig = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Docker"
    ? "ocelot.docker.json"
    : "ocelot.local.json";
builder.Configuration.AddJsonFile(ocelotConfig, optional: false, reloadOnChange: true);

builder.Services.AddOcelot();

var app = builder.Build();

await app.UseOcelot();

app.Run();