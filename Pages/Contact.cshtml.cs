using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Mail;

namespace DivCode.Pages
{
    public class ContactModel : PageModel
    {


        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Simple server-side validation
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Message))
            {
                ViewData["Message"] = "Please fill in all fields.";
                return Page();
            }

            try
            {
                // Gmail where messages will be sent
                var toAddress = new MailAddress("divineilunga011@gmail.com", "Divine Ilunga");

                // Gmail account used to send emails (from your website)
                var fromAddress = new MailAddress("divineilunga011@gmail.com", "");

                // ⚠️ Use your 16-character Gmail App Password here
                const string fromPassword = "YOUR_APP_PASSWORD";

                string subject = $"New Contact Form Message from {Name}";
                string body = $"You received a new message from your website contact form:\n\n" +
                              $"Name: {Name}\nEmail: {Email}\nMessage:\n{Message}";

                using (var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,                // Required for Gmail
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 20000                  // 20 seconds timeout
                })
                using (var mailMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    // Only add ReplyTo if user provided email
                    if (!string.IsNullOrWhiteSpace(Email))
                        mailMessage.ReplyToList.Add(new MailAddress(Email));

                    smtp.Send(mailMessage);
                }

                ViewData["Message"] = "Your message has been sent successfully!";
            }
            catch (SmtpException smtpEx)
            {
                ViewData["Message"] = $"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}";
            }
            catch (Exception ex)
            {
                ViewData["Message"] = "Error sending message: " + ex.Message;
            }

            return Page();
        }
    }
}