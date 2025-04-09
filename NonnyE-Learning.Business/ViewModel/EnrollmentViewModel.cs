using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.ViewModel
{
    public class EnrollmentViewModel
    {
        public int CourseId { get; set; }
        public decimal Amount { get; set; }
        public string? StudentEmail { get; set; }
        public string? StudentName { get; set; }
    }
}
