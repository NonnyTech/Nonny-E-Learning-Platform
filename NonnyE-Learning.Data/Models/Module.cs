using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Models
{
	public class Module
	{
		public int ModuleId { get; set; }
		public string Title { get; set; }
		public string CourseContent { get; set; }
		public int Order { get; set; }
		public int CourseId { get; set; }

		// Navigation Property
		public virtual Course Course { get; set; }
		public ModuleProgress ModuleProgress { get; set; }

	}
}
