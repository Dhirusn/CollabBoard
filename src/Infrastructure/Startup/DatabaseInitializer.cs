// src/CleanAspire.Infrastructure/Startup/DatabaseInitializer.cs
using CollabBoard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanAspire.Infrastructure.Startup;

public sealed class DatabaseInitializer(IServiceProvider sp, ILogger<DatabaseInitializer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = sp.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await ctx.Database.MigrateAsync(stoppingToken);
            logger.LogInformation("All migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Migration failed.");
            throw;   // fail fast – container will restart
        }
    }
}
