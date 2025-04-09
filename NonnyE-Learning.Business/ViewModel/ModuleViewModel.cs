using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.ViewModel
{
	public class ModuleViewModel
	{
		
			public int ModuleId { get; set; }
			public string Title { get; set; }
			public string CourseContent { get; set; }
			public bool IsCompleted { get; set; }  // Holds student progress status
			public int CourseId { get; set; } 
		
	}
}
