using NonnyE_Learning.Business.DTOs;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services.Interfaces
{
	public interface ICertificateService
	{
		Task<CertificateFile> GenerateCertificateAsync(string studentId,int courseId);
	}
}
