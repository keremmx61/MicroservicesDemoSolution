using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductService.Data;
using ProductService.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using ProductService.Validators;
using ProductService.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// === 1. Veritabaný ve Servisleri Sisteme Tanýtma ===
// === 1. Veritabaný ve Servisleri Sisteme Tanýtma ===
builder.Services.AddDbContext<ProductDbContext>(options =>
{
    var dbProvider = builder.Configuration.GetValue<string>("DB_PROVIDER");

    if (dbProvider == "PostgreSQL")
    {
        // DÜZELTÝLMÝÞ KISIM: PostgreSQL için daha basit kullaným
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsqlOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(); // Parametreler kaldýrýldý
        });
    }
    else
    {
        // SQL Server için olan kýsým ayný kalýyor
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: new[] { 4060 });
        });
    }
}, ServiceLifetime.Transient);

builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();

// === REDIS CACHING ===
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ProductService_";
});

// === JSON TABANLI LOCALIZATION ===
// Kendi implementasyonunla, OrchardCore olmadan
builder.Services.AddSingleton<IStringLocalizer<SharedResources>, JsonStringLocalizer>();

// === API ve Validation Ayarlarý ===
builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ProductValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Product API", Version = "v1" });
});

var app = builder.Build();

// === Localization Middleware === ?? En erken çaðrýlmalý
var supportedCultures = new[] { "en-US", "tr-TR" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Otomatik Migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    dbContext.Database.Migrate();
}

// Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();