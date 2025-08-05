using Azure.Identity;
using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Infrastructure.Data;
using CollabBoard.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;


namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("CollabBoardDb");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)   // optional: appsettings.json
            .Enrich.FromLogContext()
            .WriteTo.PostgreSQL(
                connectionString: connectionString,
                tableName: "logs",
                columnOptions: SerilogColumnWriters.Map,
                needAutoCreateTable: false)                   // we already created it
            .CreateLogger();

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

        builder.Services.AddRazorPages();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "CollabBoard API";

        });

        builder.Services.AddSignalR(opt =>
        {
            opt.EnableDetailedErrors = true;
        });

        builder.Services.AddCors(o => o.AddPolicy("CORS", policy => policy
        .WithOrigins(["https://localhost:4200", "http://localhost:4200"])
        .AllowAnyMethod().AllowAnyHeader().AllowCredentials()));
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }
}
