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
            //取得有昇恆昌字眼的文字 再去取連結
            var hrefEval = "[...document.querySelectorAll('table tr td:nth-child(2) a')].filter(x=>x.innerText.includes('昇恆昌') || x.innerText.includes('昇恒昌')).map(x=>x.href)";
            var nameEval = "[...document.querySelectorAll('table tr td:nth-child(2) a div div')].filter(x=>x.innerText.includes('昇恆昌') || x.innerText.includes('昇恒昌')).map(x=>x.innerText)";
            var page = await _playWrightService.GetPage();
            //先到搜尋頁
            await page.GotoAsync("https://mbasic.facebook.com/search/top/?q=%E6%98%87%E6%81%86%E6%98%8C");
            //第一個一定是粉絲專頁
            await page.Locator("text=查看全部").First.ClickAsync();

            //找齊所有昇恆昌的資料
            while (true)
            {
                var hrefData = await GetJsonElement<List<string>>(page, hrefEval);
                var nameData = await GetJsonElement<List<string>>(page, nameEval);

                if (hrefData.Count == 0)
                    break;

                for (var i = 0; i < nameData.Count; i++)
                {
                    var item = new CrawlerModel()
                    {
                        Name = nameData[i],
                        Hyperlink = hrefData[i].Replace("mbasic", "www"),
                        Time = DateTime.Now.ToString(),
                        LikeCount = "0",
                    };
                    result.Add(item);
                }

                var 查看更多 =  await page.Locator("text=查看更多結果").CountAsync();

                if(查看更多 > 0)
                {
                    await page.Locator("text=查看更多結果").First.ClickAsync();
                }
            }

            //找讚數
            for (var i = 0; i < result.Count; i++)
            {
                var url = result[i].Hyperlink;
                await page.GotoAsync(url);
                var likeCount = string.Empty;
                var 粉絲專業 = (await page.Locator("text=/.+個讚/").CountAsync()) > 0;
                var 地標 = (await page.Locator("text=/.+人說這讚/").CountAsync()) > 0;
                if (粉絲專業)
                {
                    likeCount = await page.InnerTextAsync("text=/.+個讚/", new PageInnerTextOptions { Timeout = 1000 });

                }
                if (地標)
                {
                    likeCount = await page.InnerTextAsync("text=/.+人說這?讚/", new PageInnerTextOptions { Timeout = 1000 });
                }

                result[i].LikeCount = LikeCountParse(likeCount);
                result[i].Hyperlink = page.Url;
            }

            //按讚數排序
            result = result.OrderBy(x => x.LikeCount).ToList();
            return result;
        }
        private string LikeCountParse(string count)
        {
            return count
                .Replace("說這讚", "")
                .Replace("個讚", "")
                .Replace("萬", "0,000")
                .Replace("人", "")
                .Trim();
        }
        private async Task<T> GetJsonElement<T>(IPage page, string eval)
        {
            var jsonElement = await page.EvaluateAsync(eval);
            string json = JsonSerializer.Serialize(jsonElement);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
