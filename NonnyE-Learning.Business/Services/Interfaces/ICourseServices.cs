using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services.Interfaces
{
    public interface ICourseServices
    {
           Task<IEnumerable<Course>> GetAllCoursesAsync();
           Task  <Course> GetCourseById(int courseId);
           Task<Course> AddCourseAsync(Course course);
           Task<Course> UpdateCourseAsync(Course course);
           Task<bool> DeleteCourseAsync(int courseId);
		Task<DateTime?> GetCourseCompletionDate(string studentId, int courseId);


	}
}
