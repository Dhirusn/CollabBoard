using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

//var databaseName = "CollabBoardDb";
//var userParam = builder.AddParameter("pgUser", value: "postgres");
//var passParam = builder.AddParameter("pgPass", value: "wWMtNU*Y5QDC58fUNVC7b6");

//var postgres = builder
//    .AddPostgres("postgres", userParam, passParam, 5432)
//    .WithEnvironment("POSTGRES_DB", databaseName)
//    .WithDataVolume();

//var database = postgres.AddDatabase(databaseName);

builder.AddProject<Projects.Web>("web");

builder.Build().Run();
