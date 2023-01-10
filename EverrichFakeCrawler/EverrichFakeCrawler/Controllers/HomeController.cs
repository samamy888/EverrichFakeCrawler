using EverrichFakeCrawler.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EverrichFakeCrawler.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MainService _mainServices;

        public HomeController(ILogger<HomeController> logger, MainService mainServices)
        {
            _logger = logger;
            _mainServices = mainServices;
        }

        public IActionResult Index()
        {
            return View();
        }     
    }
}