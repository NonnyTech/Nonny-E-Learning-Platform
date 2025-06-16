using NonnyE_Learning.Business.DTOs.Base;
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
		Task<BaseResponse<IEnumerable<Course>>> GetAllCoursesAsync();
		Task<BaseResponse<Course>> GetCourseById(int courseId);
		Task<BaseResponse<Course>> AddCourseAsync(Course course);
		Task<BaseResponse<Course>> UpdateCourseAsync(Course course);
		Task<BaseResponse<bool>> DeleteCourseAsync(int courseId);
		Task<BaseResponse<DateTime?>> GetCourseCompletionDate(string studentId, int courseId);
		Task<List<Course>> GetCoursesByInstructorAsync(string firstName, string lastName);


	}
}
