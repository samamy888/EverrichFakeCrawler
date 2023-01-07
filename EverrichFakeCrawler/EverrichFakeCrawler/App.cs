using EverrichFakeCrawler.Services;
using System.Diagnostics;
namespace EverrichFakeCrawler
{
    public class App
    {
        private readonly CrawlerService _service;
        private readonly ExcelService _excelService;

        public App(CrawlerService service,ExcelService excelService)
        {
            _service = service;
            _excelService = excelService;
        }
        public async Task Run()
        {
            var list = await _service.DoCrawler();

            if(list.Count > 0)
                await _excelService.ExportExcelBuffer(list);
        }
    }
}
