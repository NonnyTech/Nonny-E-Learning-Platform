using Microsoft.Extensions.Options;
using NonnyE_Learning.Business.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NonnyE_Learning.Business.AppSetting;
using NonnyE_Learning.Business.ViewModel;
using NonnyE_Learning.Data.Helper;

namespace NonnyE_Learning.Business.Services
{
	public class EmailServices : IEmailServices
	{
		private readonly SmtpClient _smtpClient;
		private readonly string _fromEmail;
		private readonly string _fromName;
		private readonly SmtpSettings _smtpSettings;

		public EmailServices(IOptions<SmtpSettings> smtpSettings)
		{
			_smtpSettings = smtpSettings.Value;

			_smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
			{
				UseDefaultCredentials = false,
				EnableSsl = true,

			};


			_smtpClient.Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Pass);
			_fromEmail = _smtpSettings.FromEmail;
			_fromName = _smtpSettings.FromName;


		}


		public void SendConfirmationEmail(string email, string subject, string body)
		{
			
			var mailMessage = new MailMessage
			{
				From = new MailAddress(_fromEmail, _fromName),
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			};

			
			mailMessage.To.Add(email);

			try
			{

				_smtpClient.Send(mailMessage);
			}
			catch (Exception ex)
			{
				// Log the error or handle as needed
				Console.WriteLine($"Error sending confirmation email: {ex.Message}");
			}
		}

		public void SendContactUsEmail(ContactUsModel model)
		{
			var emailBody = EmailTemplate.ContactFormTemplate();
			var subject = model.Subject;

			var body = GetEmailBody(model.Email, model.Phone, model.Message, emailBody);

			using (var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
			{
				smtpClient.Credentials = new System.Net.NetworkCredential(_smtpSettings.User, _smtpSettings.Pass);
				smtpClient.UseDefaultCredentials = false;
				smtpClient.EnableSsl = false;


				var mailMessage = new MailMessage
				{
					From = new MailAddress(_fromEmail, _fromName),
					Subject = subject,
					Body = body,
					IsBodyHtml = true
				};

				mailMessage.To.Add("stanley.ikemefuna91@gmail.com");

				smtpClient.Send(mailMessage); // Let the exception bubble up
			}
		}
		public void SendForgetPasswordEmail(string email, string subject, string body)
		{

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_fromEmail, _fromName),
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			};

			mailMessage.To.Add(email);

			try
			{

				_smtpClient.Send(mailMessage);
			}
			catch (Exception ex)
			{
				// Log the error or handle as needed
				Console.WriteLine($"Error sending confirmation email: {ex.Message}");
				throw;
			}

		}

		public void SendPaymentConfirmationEmail(PaymentConfirmationEmailModel model)
		{
			var emailBody = EmailTemplate.PaymentConfirmationTemplate()
						  .Replace("{{CustomerName}}", model.StudentName)
						  .Replace("{{TransactionId}}", model.TransactionId)
						  .Replace("{{Amount}}", model.Amount)
						  .Replace("{{PaymentDate}}", model.PaymentDate.ToString("MMMM dd, yyyy HH:mm"));

			var subject = "Payment Confirmation - NonnyPLUS";

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_fromEmail, _fromName),
				Subject = subject,
				Body = emailBody,
				IsBodyHtml = true
			};

			mailMessage.To.Add(model.Email);

			try
			{
				_smtpClient.Send(mailMessage);
			}
			catch (Exception ex)
			{
				// Log the error or handle as needed
				Console.WriteLine($"Error sending payment confirmation email: {ex.Message}");
			}
		}

		private string GetEmailBody(string email, string phone, string message, string emailBody)
		{
			var template = emailBody;

			return template.Replace("{{Email}}", email)
					  .Replace("{{PhoneNumber}}", phone)
					  .Replace("{{Message}}", message);
		}

	}

}
