using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Models
{
	public class CourseProgress

	{
		public int CourseProgressId { get; set; }
		public int CourseId { get; set; }
		public string StudentEmail { get; set; }
		public int CompletedModules { get; set; }
		public int TotalModules { get; set; }
		public bool IsCompleted { get; set; }

		public Course Course { get; set; }
	}
}
