using Microsoft.AspNetCore.Mvc;

namespace Nonny_E_Learning_Platform.Controllers
{
	public abstract class BaseController : Controller
	{
		protected void SetSuccessMessage(string message)
		{
			TempData["success"] = message;
		}

		protected void SetErrorMessage(string message)
		{
			TempData["error"] = message;
		}
	}
}
