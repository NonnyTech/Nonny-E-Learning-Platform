using NonnyE_Learning.Business.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services.Interfaces
{
	public interface IEmailServices
	{

		void SendConfirmationEmail(string email, string subject, string body);

		void SendForgetPasswordEmail(string email, string subject, string body);

		void SendPaymentConfirmationEmail(PaymentConfirmationEmailModel model);
		void SendContactUsEmail(ContactUsModel model);


	}
}
