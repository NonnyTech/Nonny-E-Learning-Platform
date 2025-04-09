using Microsoft.EntityFrameworkCore;
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

        public async Task<Course> AddCourseAsync(Course course)
        {
            var addCourse = await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return addCourse.Entity;
            
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var checkCourse = await _context.Courses.FindAsync(courseId);
            if (checkCourse != null) 
            {
               _context.Courses.Remove(checkCourse);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
           return await _context.Courses.ToListAsync();
        }

        public async Task<Course> GetCourseById(int courseId)
        {
           var getCourse = await _context.Courses.FirstOrDefaultAsync(m=>m.CourseId==courseId);
            return getCourse;
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            var updateCourse = _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return updateCourse.Entity;
        }

		public async Task<DateTime?> GetCourseCompletionDate(string studentId, int courseId)
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

			return latestCompletionDate == default ? (DateTime?)null : latestCompletionDate;
		}
	}
}
