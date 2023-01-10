using EverrichFakeCrawler.Services;
using Microsoft.AspNetCore.Mvc;

namespace EverrichFakeCrawler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrawlerController : Controller
    {
        private readonly MainService _mainServices;

        public CrawlerController(ILogger<CrawlerController> logger, MainService mainService)
        {
            _mainServices = mainService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var excelResponse = await _mainServices.Run();
            if(excelResponse.Data == null)
                return BadRequest($"輸出Excel失敗 , 原因 : {excelResponse.Msg}");

            return new FileStreamResult(new MemoryStream(excelResponse.Data), "application/ms-excel")
            {
                FileDownloadName = $"{DateTime.Now.ToString("yyyy-MM-dd HH-mm")}_Everrich爬蟲.xlsx"
            };
        }
    }
}