using CollabBoard.Web.Hubs;
using CollabBoard.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   // await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("CORS");

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.UseExceptionHandler(options => { });
app.MapHub<CollabHub>("/hubs/collab");

app.MapGet("/api/logs", async (IConfiguration config) =>
{
    var logs = new List<object>();
    var connStr = builder.Configuration.GetConnectionString("CollabBoardDb")    ;

    await using var conn = new Npgsql.NpgsqlConnection(connStr);
    await conn.OpenAsync();

    var cmd = new Npgsql.NpgsqlCommand("SELECT * FROM Logs ORDER BY Timestamp DESC LIMIT 100", conn);
    await using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        logs.Add(new
        {
            Timestamp = reader["Timestamp"],
            Level = reader["Level"],
            Message = reader["Message"],
            Exception = reader["Exception"]
        });
    }

    return Results.Ok(logs);
});


app.MapDefaultEndpoints();
app.MapEndpoints();

app.Run();

public partial class Program { }
