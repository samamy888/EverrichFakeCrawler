using EverrichFakeCrawler.Models;
using System.Diagnostics;
namespace EverrichFakeCrawler.Services
{
    public class MainService
    {
        private readonly CrawlerService _service;
        private readonly ExcelService _excelService;

        public MainService(CrawlerService service, ExcelService excelService)
        {
            _service = service;
            _excelService = excelService;
        }
        public async Task<ExcelResponse> Run()
        {
            var list = await _service.DoCrawler();

            if (list.Count <= 0)
                return null;

            return await _excelService.ExportExcelBuffer(list);
        }
    }
}
