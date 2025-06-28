using HealthCheckWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ApiHealthCheckerWorker>();

var host = builder.Build();
host.Run();
