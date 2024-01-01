using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
            .Enrich.WithProperty("Application", "QmkOverlay.TestConsole").Enrich.FromLogContext();
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<TestConsoleRunner>();
        services.AddTransient<ITestService, TestService>();
    })
    .Build();

await host.RunAsync();