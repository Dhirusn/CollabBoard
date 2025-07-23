var builder = DistributedApplication.CreateBuilder(args);

var databaseName = "CollabBoardDb";

var postgres = builder
    .AddPostgres("postgres", port: 5432) // add this
    .WithEnvironment("POSTGRES_DB", databaseName)
    .WithDataVolume();


var database = postgres.AddDatabase(databaseName);

builder.AddProject<Projects.Web>("web")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
