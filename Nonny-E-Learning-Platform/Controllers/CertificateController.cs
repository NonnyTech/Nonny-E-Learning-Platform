using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.Services.Interfaces;
using System.Security.Claims;

namespace Nonny_E_Learning_Platform.Controllers
{
    /// <summary>
    /// Handles certificate-related actions for students.
    /// </summary>
    [Authorize]
    public class CertificateController : BaseController
    {
        private readonly ICertificateService _certificateService;

        public CertificateController(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        /// <summary>
        /// Downloads the certificate PDF for a completed course.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadCertificate(int courseId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var certificate = await _certificateService.GenerateCertificateAsync(studentId, courseId);

            if (!certificate.Success || certificate.Data == null)
            {
                SetErrorMessage(certificate.Message);
                return RedirectToAction("MyCourses");
            }

            return File(certificate.Data.FileBytes, "application/pdf", certificate.Data.FileName);
        }
    }
}
