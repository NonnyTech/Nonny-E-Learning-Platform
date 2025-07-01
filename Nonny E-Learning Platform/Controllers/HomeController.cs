using Microsoft.AspNetCore.Mvc;
using Nonny_E_Learning_Platform.Models;
using NonnyE_Learning.Business.Services.Interfaces;
using System.Diagnostics;

namespace Nonny_E_Learning_Platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IPricingPlanServices _pricingPlanServices;

		public HomeController(ILogger<HomeController> logger, IPricingPlanServices pricingPlanServices)
        {
            _logger = logger;
			_pricingPlanServices = pricingPlanServices;
		}

        public IActionResult Index()
        {
            return View();
        }

		public IActionResult AccessDenied()
		{
			return View();
		}

		public IActionResult Privacy()
        {
            return View();
        }

        public  async Task<IActionResult> Pricing()
        {
			var response = await _pricingPlanServices.GetAllPricingPlan();

			if (!response.Success)
			{

				TempData["ErrorMessage"] = response.Message;
				return View("Error");
			}

			return View(response.Data);
		}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
