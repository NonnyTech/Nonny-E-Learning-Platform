using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Helper
{
	public static class EmailTemplate
	{
		public static string ContactFormTemplate()
		{
			{ return @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Contact Form Submission</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color: #f3f4f6; margin: 0; padding: 0;"">
    <div style=""width: 100%; padding: 32px 16px; box-sizing: border-box;"">
        <div style=""max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 32px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);"">
            
            <div style=""text-align: center; border-bottom: 1px solid #dddddd; padding-bottom: 20px; margin-bottom: 20px;"">
                <img src=""https://i.imghippo.com/files/Ft3436Izg.png"" style=""width: 200px; height: 125px;"" alt=""NonnyPlus Logo"">
                <h1 style=""text-align: center; font-size: 24px; color: #333; margin: 0; padding: 20px 0;"">New Contact Form Submission</h1>
            </div>
            
            <p style=""font-size: 16px; color: #555; margin-bottom: 20px;"">
                You have received a new message from the contact form on your website. Here are the details:
            </p>
            
            <table style=""width: 100%; border-collapse: collapse; margin-bottom: 20px;"">
                <tr>
                    <td style=""padding: 12px; border: 1px solid #eee;""><strong>Sender's Email:</strong></td>
                    <td style=""padding: 12px; border: 1px solid #eee;"">{{Email}}</td>
                </tr>
                <tr>
                    <td style=""padding: 12px; border: 1px solid #eee;""><strong>Phone Number:</strong></td>
                    <td style=""padding: 12px; border: 1px solid #eee;"">{{PhoneNumber}}</td>
                </tr>
                <tr>
                    <td style=""padding: 12px; border: 1px solid #eee;""><strong>Message:</strong></td>
                    <td style=""padding: 12px; border: 1px solid #eee;"">{{Message}}</td>
                </tr>
            </table>

            <p style=""font-size: 14px; color: #999; margin-top: 20px;"">
                This message was sent from the contact form on your website. Please respond directly to the sender's email.
            </p>
            
        </div>
    </div>
</body>
</html>
"; }
		}
		public static string RegistrationConfirmationTemplate()
		{
			return @"
    <!DOCTYPE html>
    <html lang=""en"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Email Confirmation</title>
    </head>
    <body style=""font-family: Arial, sans-serif; background-color: #f3f4f6; margin: 0; padding: 0;"">
        <div style=""width: 100%; padding: 32px 16px; box-sizing: border-box;"">
            <div style=""max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 32px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);"">
                
                <div style=""text-align: center; border-bottom: 1px solid #dddddd; padding-bottom: 20px; margin-bottom: 20px;"">
                    <img src=""https://i.imghippo.com/files/Ft3436Izg.png"" style=""width: 200px; height: 125px;"" alt=""NonnyPlus Logo"">
                    <h1 style=""text-align: center; font-size: 24px; color: #333; margin: 0; padding: 20px 0;"">Confirm Your Email</h1>
                </div>
                
                <p style=""font-size: 16px; color: #333333;"">
                    Hello, <br><br>
                    Thank you for registering with NonnyPlus! Please confirm your email address by clicking the link below:
                </p>
                <div style=""text-align: center; margin: 20px 0;"">
                    <a href=""{{ConfirmationLink}}"" style=""background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;"">
                        Confirm Email
                    </a>
                </div>
                <p style=""font-size: 14px; color: #555;"">
                    If you did not sign up for this account, please ignore this email.
                </p>
                <div style=""text-align: center; border-top: 1px solid #dddddd; padding-top: 20px; margin-top: 20px;"">
                    <h3 style=""font-size: 16px; color: #333;"">Thanks for using NonnyPLUS!</h3>
                    <p style=""color: #555;""><a href=""https://www.nonnyplus.com"">www.nonnyplus.com.ng</a></p>
                </div>
            </div>
        </div>
    </body>
    </html>
    ";
		}

		public static string ForgetPasswordTemplate()
		{
			return @"
    <!DOCTYPE html>
    <html lang=""en"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Reset Your Password</title>
    </head>
    <body style=""font-family: Arial, sans-serif; background-color: #f9f9f9; margin: 0; padding: 0;"">
        <div style=""max-width: 600px; margin: 0 auto; padding: 20px; background-color: #fff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
            <div style=""text-align: center; margin-bottom: 20px;"">
                <img src=""https://i.imghippo.com/files/Ft3436Izg.png"" alt=""NonnyPlus Logo"" style=""width: 150px; height: auto;"" />
            </div>
            <h2 style=""text-align: center; color: #333;"">Reset Your Password</h2>
            <p style=""font-size: 16px; color: #555;"">
                Hello,<br><br>
                You have requested to reset your password. Click the button below to set a new password:
            </p>
            <div style=""text-align: center; margin: 20px 0;"">
                <a href=""{{ResetLink}}"" style=""display: inline-block; background-color: #4CAF50; color: white; text-decoration: none; padding: 10px 20px; border-radius: 5px; font-size: 16px;"">Reset Password</a>
            </div>
            <p style=""font-size: 14px; color: #999;"">
                If you did not request a password reset, please ignore this email. Your password will remain unchanged.
            </p>
            <div style=""text-align: center; margin-top: 20px; color: #333;"">
                <p>Thank you,<br>The NonnyPlus Team</p>
            </div>
        </div>
    </body>
    </html>
    ";
		}

		public static string PaymentConfirmationTemplate()
		{
			return @"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Payment Confirmation</title>
    </head>
    <body style='font-family: Arial, sans-serif; background-color: #f3f4f6; padding: 20px;'>
        <div style='max-width: 600px; margin: auto; background: #fff; padding: 20px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);'>
           <div style=""text-align: center; margin-bottom: 20px;"">
                <img src=""https://i.imghippo.com/files/Ft3436Izg.png"" alt=""NonnyPlus Logo"" style=""width: 150px; height: auto;"" />
            </div>
            <h2 style='color: #333;'>Payment Confirmation</h2>
            <p>Dear {{CustomerName}},</p>
            <p>We have received your payment successfully.</p>
            <p><strong>Transaction ID:</strong> {{TransactionId}}</p>
            <p><strong>Amount:</strong> {{Amount}}</p>
            <p><strong>Payment Date:</strong> {{PaymentDate}}</p>
            <p>Thank you for choosing NonnyPLUS</p>
            <p>Best Regards, <br> The NonnyPLUS Team</p>
        </div>
    </body>
    </html>";
		}

		public static string OtpVerificationTemplate()
		{
			return @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Your OTP Code</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color: #f9f9f9; margin: 0; padding: 0;"">
    <div style=""max-width: 600px; margin: auto; background-color: #fff; padding: 20px; border-radius: 6px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);"">
        <div style=""text-align: center;"">
            <img src=""https://i.imghippo.com/files/Ft3436Izg.png"" alt=""NonnyPlus Logo"" style=""width: 150px; height: auto;"" />
        </div>
        <h2 style=""color: #333;"">Your One-Time Password (OTP)</h2>
        <p style=""font-size: 16px; color: #555;"">
            Please use the OTP below to complete your login. This code will expire in 5 minutes.
        </p>
        <div style=""text-align: center; margin: 30px 0;"">
            <span style=""font-size: 32px; letter-spacing: 5px; font-weight: bold; color: #4CAF50;"">{{OtpCode}}</span>
        </div>
        <p style=""font-size: 14px; color: #999;"">
            If you did not request this OTP, please ignore this email.
        </p>
        <p style=""font-size: 14px; color: #333;"">
            Thanks,<br>The NonnyPLUS Team
        </p>
    </div>
</body>
</html>
";
		}
	}
}
