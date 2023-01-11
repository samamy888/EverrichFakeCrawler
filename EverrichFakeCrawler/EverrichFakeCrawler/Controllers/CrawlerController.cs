using EverrichFakeCrawler.Services;
using JC_PTTLogin.Models;
using Microsoft.AspNetCore.Mvc;

namespace EverrichFakeCrawler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrawlerController : Controller
    {
        private readonly ILogger<CrawlerController> _logger;
        private readonly MainService _mainServices;

        public CrawlerController(ILogger<CrawlerController> logger, MainService mainService)
        {
            _logger = logger;
            _mainServices = mainService;
        }
        [HttpPost]
        public async Task<IActionResult> Index([FromBody]CrawlerRequest request)
        {
            _logger.LogInformation("開始爬蟲");
            var excelResponse = await _mainServices.Run(request);
            if(excelResponse.Data == null)
                return BadRequest($"輸出Excel失敗 , 原因 : {excelResponse.Msg}");

            return new FileStreamResult(new MemoryStream(excelResponse.Data), "application/ms-excel")
            {
                FileDownloadName = $"{DateTime.Now.ToString("yyyy-MM-dd HH-mm")}_Everrich爬蟲.xlsx"
            };
        }
    }
}