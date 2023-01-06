using EverrichFakeCrawler.Services;
using System.Diagnostics;
namespace EverrichFakeCrawler
{
    public class App
    {
        private readonly CrawlerService _service;

        public App(CrawlerService service)
        {
            _service = service;
        }
        public async Task Run()
        {
            await _service.DoCrawler();
        }
    }
}
