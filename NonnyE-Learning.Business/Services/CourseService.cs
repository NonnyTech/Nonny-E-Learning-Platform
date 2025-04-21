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
    public class CourseService : ICourseServices
    {
       
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
           
            _context = context;
        }

		public async Task<BaseResponse<Course>> AddCourseAsync(Course course)
		{
			try
			{
				var addCourse = await _context.Courses.AddAsync(course);
				await _context.SaveChangesAsync();
				return new BaseResponse<Course>
				{
					Success = true,
					Data = addCourse.Entity
				};
			}
			catch (Exception ex)
			{
				return new BaseResponse<Course>
				{
					Success = false,
					Message = $"An error occurred while adding the course: {ex.Message}"
				};
			}
		}

		public async Task<BaseResponse<bool>> DeleteCourseAsync(int courseId)
		{
			try
			{
				var checkCourse = await _context.Courses.FindAsync(courseId);
				if (checkCourse != null)
				{
					_context.Courses.Remove(checkCourse);
					await _context.SaveChangesAsync();
					return new BaseResponse<bool>
					{
						Success = true,
						Data = true
					};
				}
				return new BaseResponse<bool>
				{
					Success = false,
					Message = "Course not found"
				};
			}
			catch (Exception ex)
			{
				return new BaseResponse<bool>
				{
					Success = false,
					Message = $"An error occurred while deleting the course: {ex.Message}"
				};
			}
		}

		public async Task<BaseResponse<IEnumerable<Course>>> GetAllCoursesAsync()
		{
			try
			{
				var courses = await _context.Courses.ToListAsync();
				return new BaseResponse<IEnumerable<Course>>
				{
					Success = true,
					Data = courses
				};
			}
			catch (Exception ex)
			{
				return new BaseResponse<IEnumerable<Course>>
				{
					Success = false,
					Message = $"An error occurred while retrieving courses: {ex.Message}"
				};
			}
		}

		public async Task<BaseResponse<Course>> GetCourseById(int courseId)
		{
			try
			{
				var getCourse = await _context.Courses.FirstOrDefaultAsync(m => m.CourseId == courseId);
				if (getCourse == null)
				{
					return new BaseResponse<Course>
					{
						Success = false,
						Message = "Course not found"
					};
				}
				return new BaseResponse<Course>
				{
					Success = true,
					Data = getCourse
				};
			}
			catch (Exception ex)
			{
				return new BaseResponse<Course>
				{
					Success = false,
					Message = $"An error occurred while retrieving the course: {ex.Message}"
				};
			}
		}

		public async Task<BaseResponse<Course>> UpdateCourseAsync(Course course)
		{
			try
			{
				var updateCourse = _context.Courses.Update(course);
				await _context.SaveChangesAsync();
				return new BaseResponse<Course>
				{
					Success = true,
					Data = updateCourse.Entity
				};
			}
			catch (Exception ex)
			{
				return new BaseResponse<Course>
				{
					Success = false,
					Message = $"An error occurred while updating the course: {ex.Message}"
				};
			}
		}

		public async Task<BaseResponse<DateTime?>> GetCourseCompletionDate(string studentId, int courseId)
		{
			try
			{
				// Retrieve all module IDs for the given course
				var moduleIds = await _context.Modules
					.Where(m => m.CourseId == courseId)
					.Select(m => m.ModuleId)
					.ToListAsync();

				// Get the most recent completion date from the student's progress
				var latestCompletionDate = await _context.ModuleProgress
					.Where(mp => moduleIds.Contains(mp.ModuleId) && mp.StudentId == studentId && mp.IsCompleted)
					.OrderByDescending(mp => mp.CompletedAt)
					.Select(mp => mp.CompletedAt)
					.FirstOrDefaultAsync();

				if (latestCompletionDate == default)
					return new BaseResponse<DateTime?>
					{
						Success = false,
						Message = "Student has not completed the course modules."
					};

				return new BaseResponse<DateTime?>
				{
					Success = true,
					Data = latestCompletionDate
				};
			}
			catch (Exception ex)
			{
				return new BaseResponse<DateTime?>
				{
					Success = false,
					Message = $"An error occurred while retrieving course completion date: {ex.Message}"
				};
			}
		}
	}

}

