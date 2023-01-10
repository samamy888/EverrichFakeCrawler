using EverrichFakeCrawler.Configs;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace EverrichFakeCrawler.Services
{
    public class PlayWrightService
    {
        private readonly Config _config;
        private IBrowserContext _browser;
        public PlayWrightService(IOptions<Config> config)
        {
            _config = config.Value;
        }
        public async Task<IBrowserContext> GetBrowserContext()
        {
            // 取得一個 Playwright 實例 (instance)
            var playwright = await Playwright.CreateAsync();
            string playWrightPath = _config.PlayWrightPath;
            try
            {
                return await playwright.Chromium.LaunchPersistentContextAsync(playWrightPath, new BrowserTypeLaunchPersistentContextOptions
                {
                    Headless = false,
                    SlowMo = 50,
                    ExecutablePath = Path.Combine(_config.ChromePathX86, "chrome.exe"),
                    Channel = "chrome",
                });
            }
            catch(Exception)
            {
                return await playwright.Chromium.LaunchPersistentContextAsync(playWrightPath, new BrowserTypeLaunchPersistentContextOptions
                {
                    Headless = false,
                    SlowMo = 50,
                    ExecutablePath = Path.Combine(_config.ChromePath, "chrome.exe"),
                    Channel = "chrome",
                });
            }
            
        }
        public async Task<IPage> GetPage()
        {
            _browser = await GetBrowserContext();
            var page = _browser.Pages.First();
            return page;
        }
        public async Task ClosePage()
        {
            await _browser.CloseAsync();
        }
    }
}
