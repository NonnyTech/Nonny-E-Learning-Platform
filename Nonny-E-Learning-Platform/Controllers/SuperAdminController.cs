using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.Services.Interfaces;

namespace Nonny_E_Learning_Platform.Controllers
{
	public class SuperAdminController : BaseController
{
   
		private readonly ITransactionServices _transactionServices;

		public SuperAdminController(ITransactionServices transactionServices)
		{
			_transactionServices = transactionServices;
		}

		public IActionResult Index()
{
    return View();
}

[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> AllTransactions()
{
    var response = await _transactionServices.GetAllTransactionAsync();
    if (!response.Success)
    {
        SetErrorMessage(response.Message ?? "Failed to retrieve transactions.");
        return View("Error");
    }
    return View(response.Data);
}
}
}
