using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services
{
	public class EnrollmentServices : IEnrollmentServices
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public EnrollmentServices(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public async Task<BaseResponse<int>> CreateOrGetEnrollmentAsync(int courseId, string studentId)
		{
			try

			{
	      		var enrollment = await _context.Enrollements
				.FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

				if (enrollment == null)
				{
					var student = await _userManager.FindByIdAsync(studentId);
					if (student == null)
					{
						throw new Exception("Student not found.");
					}
					enrollment = new Enrollment
					{
						CourseId = courseId,
						StudentId = studentId,
						StudentEmail = student.Email,
						IsCompleted = false,
						EnrollmentDate = DateTime.UtcNow
					};

					_context.Enrollements.Add(enrollment);
					await _context.SaveChangesAsync();
				}
				return new BaseResponse<int>
				{
					Success = true,
					Message = "Course enrollment created successfully.",
					Data = enrollment.EnrollmentId
				};
			}
			catch (Exception ex) 
			{
				return new BaseResponse<int>
				{
					Success = false,
					Message = "An error occurred while creating the enrollment.",
					Errors = new List<string> { ex.Message }
				};

			}
		}

		public async Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(string studentId)
		{
			return await _context.Enrollements
		   .Where(e => e.StudentId == studentId)
		   .Include(e => e.Course)
		   .ToListAsync();
		}
	}
}
