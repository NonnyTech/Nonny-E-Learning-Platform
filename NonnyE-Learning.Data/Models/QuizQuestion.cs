using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Models
{
	public class QuizQuestion
	{
		public int QuizQuestionId { get; set; }
		public string QuestionText { get; set; }
		public string OptionA { get; set; }
		public string OptionB { get; set; }
		public string OptionC { get; set; }
		public string OptionD { get; set; }
		public string CorrectOption { get; set; }

		public int ModuleId { get; set; }
		public Module Module { get; set; }
		public int Order { get; set; }

	}

}
