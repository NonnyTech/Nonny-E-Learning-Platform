using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.ViewModel
{
	public class PaymentConfirmationEmailModel
	{
		public string Email { get; set; }
		public string StudentName { get; set; }
		public string TransactionId { get; set; }
		public string Amount { get; set; }
		public DateTime PaymentDate { get; set; }
	}
}
