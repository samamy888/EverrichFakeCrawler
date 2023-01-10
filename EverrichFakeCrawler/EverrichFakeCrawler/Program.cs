using EverrichFakeCrawler.Configs;
using EverrichFakeCrawler.Services;
using NLog.Extensions.Logging;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.WebHost.UseElectron(args);
builder.Services.AddScoped<MainService>();
builder.Services.AddScoped<CrawlerService>();
builder.Services.AddScoped<PlayWrightService>();
builder.Services.AddScoped<ExcelService>();
builder.Services.Configure<Config>(builder.Configuration.GetSection("Config"));
builder.Logging.AddNLog("Config/NLog.config");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

Electron.Menu.SetApplicationMenu(new MenuItem[] { });

Task.Run(async () =>
{
    var mainWindow = await Electron.WindowManager.CreateWindowAsync(
    new BrowserWindowOptions
    {
        Title = "ª@«í©÷¤Ï¶BÄF",
        Width = 500,
        Height = 500
    });
});

app.Run();


