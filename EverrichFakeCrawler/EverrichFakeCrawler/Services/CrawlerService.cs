using EverrichFakeCrawler.Configs;
using EverrichFakeCrawler.Models;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using System.Text.Json;

namespace EverrichFakeCrawler.Services
{
    public class CrawlerService
    {
        private readonly ILogger<CrawlerService> _logger;
        private readonly PlayWrightService _playWrightService;
        private readonly IPage _page;
        public CrawlerService(ILogger<CrawlerService> logger, PlayWrightService playWrightService)
        {
            _logger = logger;
            _playWrightService = playWrightService;
        }
        public async Task<List<CrawlerModel>> DoCrawler()
        {
            var result = new List<CrawlerModel>();
            var page = await _playWrightService.GetPage();
            //先到搜尋頁
            await page.GotoAsync("https://mbasic.facebook.com/search/top/?q=%E6%98%87%E6%81%86%E6%98%8C");
            //第一個一定是粉絲專頁
            await page.Locator("text=查看全部").First.ClickAsync();

            //取得有昇恆昌字眼的文字 再去取連結
            var hrefEval = "[...document.querySelectorAll('table tr td:nth-child(2) a')].filter(x=>x.innerText.includes('昇恆昌')).map(x=>x.href)";
            var nameEval = "[...document.querySelectorAll('table tr td:nth-child(2) a div div')].filter(x=>x.innerText.includes('昇恆昌')).map(x=>x.innerText)";

            var hrefData = await GetJsonElement<List<string>>(page, hrefEval);
            var nameData = await GetJsonElement<List<string>>(page, nameEval);

            for (var i = 0; i < nameData.Count; i++)
            {
                var item = new CrawlerModel()
                {
                    Name = nameData[i],
                    Hyperlink = hrefData[i],
                    Time = DateTime.Now.ToString(),
                };
                result.Add(item);
            }
            await page.Locator("text=/.+個讚/").First.ClickAsync();


            return result;
        }
        private async Task<T> GetJsonElement<T>(IPage page, string eval)
        {
            var jsonElement = await page.EvaluateAsync(eval);
            string json = JsonSerializer.Serialize(jsonElement);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
