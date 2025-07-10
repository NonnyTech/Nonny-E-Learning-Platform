using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Business.DTOs;
using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PdfDocument = PdfSharp.Pdf.PdfDocument;

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

		public async Task<BaseResponse<CertificateFile>> GenerateCertificateAsync(string studentId, int courseId)
		{
			var student = await _moduleServices.GetStudentById(studentId); // Already optimized if GetStudentById uses AsNoTracking
			if (student == null)
			{
				return new BaseResponse<CertificateFile>
				{
					Success = false,
					Message = "Student not found"
				};
			}

			var courseResponse = await _courseServices.GetCourseById(courseId); // Already optimized if GetCourseById uses AsNoTracking
			if (!courseResponse.Success || courseResponse.Data == null)
			{
				return new BaseResponse<CertificateFile>
				{
					Success = false,
					Message = "Course not found"
				};
			}

			var course = courseResponse.Data;

			if (!await _moduleServices.HasUserCompletedAllModules(courseId, studentId))
			{
				return new BaseResponse<CertificateFile>
				{
					Success = false,
					Message = "Certificate cannot be generated. The student has not completed all modules."
				};
			}

			var completionDateResponse = await _courseServices.GetCourseCompletionDate(studentId, courseId);

			string completionDateText = completionDateResponse.Success && completionDateResponse.Data.HasValue
				? completionDateResponse.Data.Value.ToString("MMMM dd, yyyy")
				: "N/A";

			var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/NonnyIndexLogo.png");

			var document = new CertificateDocument(
				$"{student.FirstName} {student.LastName}",
				course.Title,
				completionDateText,
				logoPath);

			using var ms = new MemoryStream();
			document.GeneratePdf(ms);

			return new BaseResponse<CertificateFile>
			{
				Success = true,
				Message = "Certificate generated successfully",
				Data = new CertificateFile
				{
					FileBytes = ms.ToArray(),
					FileName = $"{student.FirstName}_{student.LastName}_{course.Title}_Certificate.pdf"
				}
			};
		}
	}
}
