using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Models
{
    public class Course
    {

        public int CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
		public string? Instructor { get; set; }
		public int Lectures { get; set; }
		public string? Duration { get; set; }
		public string? ImageUrl { get; set; }
		public string? Category { get; set; }

		
		//Navigation Property

		public ICollection<Enrollment> Enrollments { get; set; }
		public ICollection<Transaction> Transactions { get; set; }


	}
}

