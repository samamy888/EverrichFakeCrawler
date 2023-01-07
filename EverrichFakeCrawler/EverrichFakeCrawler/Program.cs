using EverrichFakeCrawler;
using EverrichFakeCrawler.Configs;
using EverrichFakeCrawler.Services;
using NLog.Extensions.Logging;

static void ConfigureServices(IServiceCollection services)
{
    // build config
    IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

    // configure logging
    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddConsole();
        builder.AddDebug();
        builder.AddNLog(config.GetSection("Config/NLog.config"));
    });

    services.Configure<Config>(config.GetSection("Config"));


    // add app
    services.AddScoped<App>();
    services.AddScoped<CrawlerService>();
    services.AddScoped<Config>();
    services.AddScoped<ExcelService>();
    services.AddScoped<PlayWrightService>();
}

var services = new ServiceCollection();
ConfigureServices(services);

using var serviceProvider = services.BuildServiceProvider();

// entry to run app
await serviceProvider.GetService<App>()!.Run();