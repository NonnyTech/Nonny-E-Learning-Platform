using Microsoft.AspNetCore.Mvc;

namespace Nonny_E_Learning_Platform.Controllers
{
    public class AboutController : Controller
{
    private void SetSuccessMessage(string message)
    {
        TempData["success"] = message;
    }

    private void SetErrorMessage(string message)
    {
        TempData["error"] = message;
    }
    
        public IActionResult Index()
        {
            return View();
        }
    }
}
