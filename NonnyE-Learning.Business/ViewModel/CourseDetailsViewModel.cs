using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.ViewModel
{
    public class CourseDetailsViewModel
    {
		public int CourseId { get; set; }
		public string Title { get; set; }
		public string Instructor { get; set; }
		public string Duration { get; set; }
		public int Lectures { get; set; }
		public decimal Price { get; set; }
		public string ImageUrl { get; set; }
		public string Category { get; set; }
		public int EnrollmentId { get; set; }
	}
}
