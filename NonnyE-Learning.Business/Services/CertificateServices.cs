using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Business.DTOs;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NonnyE_Learning.Business.Services
{
	public class CertificateServices : ICertificateService
	{
		private readonly ApplicationDbContext _context;
		private readonly IModuleServices _moduleServices;
		private readonly ICourseServices _courseServices;

		public CertificateServices(ApplicationDbContext context, IModuleServices moduleServices,ICourseServices courseServices)
		{
			_context = context;
			_moduleServices = moduleServices;
			_courseServices = courseServices;
		}
		public async Task<CertificateFile> GenerateCertificateAsync(string studentId, int courseId)
		{
			var student = await _moduleServices.GetStudentById(studentId);
			var course = await _courseServices.GetCourseById(courseId);

			// Ensure the student has completed all modules before generating the certificate
			if (!await _moduleServices.HasUserCompletedAllModules(courseId, studentId))
				return null;

			var completionDate = await _courseServices.GetCourseCompletionDate(studentId, courseId);
			string completionDateText = completionDate.HasValue ? completionDate.Value.ToString("MMMM dd, yyyy") : "N/A";

			using (var ms = new MemoryStream())
			{
				var doc = new PdfDocument();
				var page = doc.AddPage();
				var gfx = XGraphics.FromPdfPage(page);

				// Define fonts
				var titleFont = new XFont("Times New Roman", 30, XFontStyleEx.Bold);
				var nameFont = new XFont("Times New Roman", 24, XFontStyleEx.Bold);
				var courseFont = new XFont("Times New Roman", 18, XFontStyleEx.Regular);
				var dateFont = new XFont("Times New Roman", 16, XFontStyleEx.Italic);
				var footerFont = new XFont("Arial", 14, XFontStyleEx.Bold);
				var signatureFont = new XFont("Arial", 14, XFontStyleEx.Italic);

				// Set up certificate background
				gfx.DrawRectangle(XBrushes.WhiteSmoke, new XRect(0, 0, page.Width, page.Height));

				// Load and draw the logo (Centered at the top)
				string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/NonnyIndexLogo.png");
				if (File.Exists(logoPath))
				{
					XImage logo = XImage.FromFile(logoPath);

					// Adjust the size to fit well on the certificate
					double logoWidth = 150;  // Set an appropriate width
					double logoHeight = 30; // Maintain aspect ratio
					double logoX = (page.Width - logoWidth) / 2; // Center horizontally
					double logoY = 120;  // Position slightly below the top edge

					gfx.DrawImage(logo, logoX, logoY, logoWidth, logoHeight);
				}

				// Certificate Title
				gfx.DrawString("Certificate of Completion", titleFont, XBrushes.Black,
					new XRect(0, 180, page.Width, 50), XStringFormats.Center);

				// Subtitle
				gfx.DrawString("This is proudly awarded to", courseFont, XBrushes.Gray,
					new XRect(0, 230, page.Width, 30), XStringFormats.Center);

				// Student Name (Highlighted)
				gfx.DrawString($"{student.FirstName} {student.LastName}", nameFont, XBrushes.DarkBlue,
					new XRect(0, 270, page.Width, 40), XStringFormats.Center);

				// Course Completed
				gfx.DrawString($"for successfully completing the course:", courseFont, XBrushes.Black,
					new XRect(0, 320, page.Width, 30), XStringFormats.Center);

				gfx.DrawString($"{course.Title}", nameFont, XBrushes.DarkGreen,
				   new XRect(0, 360, page.Width, 40), XStringFormats.Center);

				// Completion Date
				gfx.DrawString($"Completed on {completionDateText}", dateFont, XBrushes.Gray,
					new XRect(0, 420, page.Width, 30), XStringFormats.Center);

				// Company Name (Footer)
				gfx.DrawString("NonnyPlus E-Learning System", footerFont, XBrushes.Black,
					new XRect(0, page.Height - 80, page.Width, 30), XStringFormats.Center);

				// Director's Signature (Far Left)
				gfx.DrawString("Director's Signature: ______________________", signatureFont, XBrushes.Black,
					new XRect(20, page.Height - 140, page.Width / 2, 30), XStringFormats.Center);

				// Instructor's Signature (Far Right)
				gfx.DrawString("Instructor's Signature: ______________________", signatureFont, XBrushes.Black,
					new XRect(page.Width - 250, page.Height - 140, page.Width / 2, 30), XStringFormats.Center);

				// If you have images of signatures, you can replace the text with image drawing:
				// string directorSignaturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/DirectorSignature.png");
				// string instructorSignaturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/InstructorSignature.png");

				// If the signature images exist, draw them:
				// if (File.Exists(directorSignaturePath))
				// {
				//     XImage directorSignature = XImage.FromFile(directorSignaturePath);
				//     gfx.DrawImage(directorSignature, 20, page.Height - 150, 100, 30); // Adjust positioning and size
				// }
				// if (File.Exists(instructorSignaturePath))
				// {
				//     XImage instructorSignature = XImage.FromFile(instructorSignaturePath);
				//     gfx.DrawImage(instructorSignature, page.Width - 250, page.Height - 150, 100, 30); // Adjust positioning and size
				// }

				// Save PDF to MemoryStream
				doc.Save(ms);

				return new CertificateFile
				{
					FileBytes = ms.ToArray(),
					FileName = $"{student.FirstName}_{student.LastName}_{course.Title}_Certificate.pdf"
				};
			}
		}
	}
}
