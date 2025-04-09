using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Models
{
    public class Enrollment
    {

		public int EnrollmentId { get; set; }
		public string StudentId { get; set; }
		public string StudentEmail { get; set; }

		public int CourseId { get; set; }
		public bool IsCompleted { get; set; }
		public DateTime EnrollmentDate { get; set; }

		// Navigation Properties
		public ApplicationUser Student { get; set; }
		public Course Course { get; set; }
		
	}
}
