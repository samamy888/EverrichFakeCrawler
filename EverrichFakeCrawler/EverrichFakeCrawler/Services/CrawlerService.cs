using EverrichFakeCrawler.Configs;
using EverrichFakeCrawler.Models;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using System.Text.Json;
using System.Text.RegularExpressions;

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
        public async Task<List<CrawlerModel>> DoCrawler(JC_PTTLogin.Models.CrawlerRequest request)
        {
            var result = new List<CrawlerModel>();
            //取得有昇恆昌字眼的文字 再去取連結
            var hrefEval = GetHrefEvalString(request.Keyword);
            var nameEval = GetNameEvalString(request.Keyword);
            var page = await _playWrightService.GetPage();
            var searchUrl = $"https://mbasic.facebook.com/search/pages/?q={request.Keyword}";
            //先到搜尋頁
            await page.GotoAsync(searchUrl);

            #region login
            var isLogin = await (page.Locator("text=加入 Facebook 或登入以繼續。").CountAsync());

            if (isLogin > 0)
            {
                try
                {
                    await page.GotoAsync("https://www.facebook.com/login/device-based/regular/login/?login_attempt=1");
                    await page.Locator("#email").FillAsync(request.AccountId);
                    await page.Locator("#pass").FillAsync(request.Password);
                    await page.Locator("#loginbutton").ClickAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"登入失敗 : {ex.Message}");
                    return new List<CrawlerModel>();
                }
            }
            //如果目前的URL還是要登入代表失敗
            var nowUrl = page.Url;
            if (nowUrl.ToLower().Contains("login"))
            {
                _logger.LogError($"登入失敗");
                return new List<CrawlerModel>();
            }
            // 再回到一開始
            await page.GotoAsync(searchUrl);
            #endregion

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
                        Hyperlink = UrlParse(hrefData[i].Replace("mbasic", "www")),
                        Time = DateTime.Now.ToString(),
                        LikeCount = "0",
                    };
                    result.Add(item);
                }

                var 查看更多 = await page.Locator("text=查看更多結果").CountAsync();

                if (查看更多 > 0)
                {
                    await page.Locator("text=查看更多結果").First.ClickAsync();
                }
            }

            //找讚數
            for (var i = 0; i < result.Count; i++)
            {
                var url = result[i].Hyperlink;
                await page.GotoAsync(url);
                var likeCount = "0";
                var 粉絲專頁 = (await page.Locator("text=/.+個讚/").CountAsync()) > 0;
                var 地標 = (await page.Locator("text=/.+人說這?讚/").CountAsync()) > 0;
                if (粉絲專頁)
                {
                    likeCount = await page.InnerTextAsync("text=/.+個讚/", new PageInnerTextOptions { Timeout = 1000 });
                }
                if (地標)
                {
                    likeCount = await page.InnerTextAsync("text=/.+人說這?讚/", new PageInnerTextOptions { Timeout = 1000 });
                }

                result[i].LikeCount = LikeCountParse(likeCount);
            }

            await _playWrightService.ClosePage();

            //按讚數排序
            result = result.OrderBy(x => Decimal.Parse(x.LikeCount.Replace(",", ""))).ToList();
            return result;
        }
        private string LikeCountParse(string count)
        {
            return count
                .Replace("說這讚", "")
                .Replace("說讚", "")
                .Replace("個讚", "")
                .Replace("萬", "0,000")
                .Replace("人", "")
                .Replace(" ", "")
                .Trim();
        }
        private string UrlParse(string url)
        {
            var regex = new Regex("&eav=.+");

            return regex.Replace(url, "").Trim();
        }

        private string GetHrefEvalString(string keyword)
        {
            return $"[...document.querySelectorAll('table tr td:nth-child(2) a')].filter(x=>x.innerText.replaceAll(' ','').toLowerCase().includes('{keyword.ToLower()}')).map(x=>x.href)";
        }
        private string GetNameEvalString(string keyword)
        {
            return $"[...document.querySelectorAll('table tr td:nth-child(2) a div div')].filter(x => x.innerText.replaceAll(' ','').toLowerCase().includes('{keyword.ToLower()}')).map(x => x.innerText)";
        }
        private async Task<T> GetJsonElement<T>(IPage page, string eval)
        {
            var jsonElement = await page.EvaluateAsync(eval);
            string json = JsonSerializer.Serialize(jsonElement);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
