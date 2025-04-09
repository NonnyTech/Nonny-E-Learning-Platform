using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services.Interfaces
{
	public interface IEnrollmentServices
	{
		Task<BaseResponse<int>> CreateOrGetEnrollmentAsync(int courseId, string studentId);
		Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(string studentId);

	}
}
