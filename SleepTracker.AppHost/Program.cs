var builder = DistributedApplication.CreateBuilder(args);

var sleeptrackerApi = builder.AddProject<Projects.SleepTracker>("sleeptracker");

builder.AddNpmApp("frontend", "../SleepTracker.App")
    .WithReference(sleeptrackerApi)
    .WaitFor(sleeptrackerApi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
