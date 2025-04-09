using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.ViewModel
{
	public class CourseLearningViewModel
	{
		public IEnumerable<Module> Modules { get; set; }
		public bool IsCourseCompleted { get; set; }
	}
}
