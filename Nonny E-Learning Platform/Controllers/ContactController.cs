using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;

namespace Nonny_E_Learning_Platform.Controllers
{
    public class ContactController : Controller
    {
		private readonly IEmailServices _emailServices;

		public ContactController(IEmailServices emailServices)
		{
			_emailServices = emailServices;
		}
		public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactUsModel model)
		{
			if (!ModelState.IsValid)
			{
				TempData["error"] = "Please fill in all required fields.";
				return View(model);
			}

			try
			{

				_emailServices.SendContactUsEmail(model);
				TempData["success"] = "Your message has been sent successfully!";
			}
			catch
			{
				TempData["error"] = "There was an issue sending your message. Please try again later.";
			}

			return RedirectToAction("Index");
		}

	}
}
