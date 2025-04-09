using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Models
{
	public  class ModuleProgress
	{
		public int ModuleProgressId { get; set; }
		public int ModuleId { get; set; }
		public string StudentId { get; set; }
		public bool IsCompleted { get; set; }
		public DateTime? CompletedAt { get; set; }

		// Navigation property
		public Module Module { get; set; }
	}
}
