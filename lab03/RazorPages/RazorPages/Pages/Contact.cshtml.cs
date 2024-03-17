using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace RazorPages.Pages
{
    [Bind]
    public class ContactForm
    {
        [BindProperty(Name = "first_name")]
        public string? FirstName { get; set; }

        [BindProperty(Name = "last_name")]
        public string? LastName { get; set; }

        [BindProperty(Name = "email")]
        public string? Email { get; set; }

        [BindProperty(Name = "phone")]
        public string? Phone { get; set; }

        [BindProperty(Name = "select_service")]
        public string? Service { get; set; }

        [BindProperty(Name = "select_price")]
        public string? Price { get; set; }

        [BindProperty(Name = "comments")]
        public string? Comments { get; set; }
    }

    [IgnoreAntiforgeryToken]
    public class ContactModel : PageModel
    {
        public ContactForm InputForm { get; set; } = default!;
        private EmailAddressAttribute validator = new();

        public IActionResult OnPost()
        {
            if (InputForm.FirstName == null || InputForm.FirstName == string.Empty)
                return Content("<div class=\"error_message\">Attention! You must enter your name.</div>");

            if (InputForm.Email == null || InputForm.Email.Trim() == string.Empty)
                return Content("<div class=\"error_message\">Attention! Please enter a valid email address.</div>");

            if (!validator.IsValid(InputForm.Email))
                return Content("<div class=\"error_message\">Attention! You have enter an invalid e-mail address, try again.</div>");

            if (InputForm.Comments == null || InputForm.Comments.Trim() == string.Empty)
                return Content("<div class=\"error_message\">Attention! Please enter your message.</div>");

            WriteLog();
            return Content($@"<fieldset>
                              <div id='success_page'>
                              <h1>Email Sent Successfully.</h1>
                              <p>Thank you <strong>{InputForm.FirstName}</strong>, your message has been submitted to us.</p>
                              </div>
                              </fieldset>");
        }

        private void WriteLog()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using var stream = System.IO.File.Open("logs.csv", FileMode.Append);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, config);
            if (stream.Length == 0)
            {
                csv.WriteHeader<ContactForm>();
                csv.NextRecord();
            }
            csv.WriteRecord(InputForm);
            csv.NextRecord();
        }
    }
}
