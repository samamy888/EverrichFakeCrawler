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
        public async Task<ExcelResponse> Run(JC_PTTLogin.Models.CrawlerRequest request)
        {
            var list = await _service.DoCrawler(request);

            if (list.Count <= 0)
                return new ExcelResponse();

            return await _excelService.ExportExcelBuffer(list);
        }
    }
}
