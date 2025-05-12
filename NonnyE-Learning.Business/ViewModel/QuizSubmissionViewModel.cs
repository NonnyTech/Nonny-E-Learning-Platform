using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.ViewModel
{
	public class QuizSubmissionViewModel
	{
		public int ModuleId { get; set; }
		public List<QuestionAnswer> Answers { get; set; }
	}

	public class QuestionAnswer
	{
		public int QuizQuestionId { get; set; }
		public string SelectedOption { get; set; } // A, B, C, or D
	}
}
