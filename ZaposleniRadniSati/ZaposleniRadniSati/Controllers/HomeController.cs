using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ZaposleniRadniSati.Managers;
using ZaposleniRadniSati.Models;

namespace ZaposleniRadniSati.Controllers
{
    public class HomeController : Controller
    {
        private readonly ZaposleniManager zaposleniManager;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            zaposleniManager = new ZaposleniManager();
        }

        public async Task<IActionResult> IndexAsync()
        {
            var zaposleni = await zaposleniManager.GetZaposleniAsync();
            var zaposleniPoRedu = zaposleni.OrderByDescending(e => e.RadniSati).ToList();
            return View(zaposleniPoRedu);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
