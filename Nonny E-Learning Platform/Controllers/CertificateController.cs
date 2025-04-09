using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.Services.Interfaces;
using System.Security.Claims;

namespace Nonny_E_Learning_Platform.Controllers
{
	[Authorize]
	public class CertificateController : Controller
	{
		private readonly ICertificateService _certificateService;

		public CertificateController(ICertificateService certificateService)
		{
			_certificateService = certificateService;
		}
		[HttpGet]
		public async Task<IActionResult> DownloadCertificate(int courseId)
		{
			var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var certificate = await _certificateService.GenerateCertificateAsync(studentId, courseId);

			if (certificate == null)
				return NotFound("Certificate not available. Ensure all course modules are completed.");

			return File(certificate.FileBytes, "application/pdf", certificate.FileName);
		}
	}
}
