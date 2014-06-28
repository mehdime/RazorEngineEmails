using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication.Models;
using RazorEngine.Templating;

namespace ConsoleApplication
{
    class Program
    {
        static readonly string TemplateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");

        static void Main(string[] args)
        {
            // Generate an email using a strongly-typed model
            Console.WriteLine("Generating email with strongly-typed model...");
            StronglyTypedModel();

            // Generate an email using a dynamic model
            Console.WriteLine("Generating email with dynamic model...");
            DynamicModel();

            Console.WriteLine("Done.");
            Console.ReadLine();

            // Pro tip: use http://smtp4dev.codeplex.com to test emails while in development
        }

        static void StronglyTypedModel()
        {
            var welcomeEmailTemplatePath = Path.Combine(TemplateFolderPath, "WelcomeEmailStronglyTyped.cshtml");

            // Generate the email body from our email template
            var stronglyTypedModel = new UserModel() { Name = "Sarah", Email = "sarah@mail.example", IsPremiumUser = false };

            var templateService = new TemplateService();
            var emailHtmlBody = templateService.Parse(File.ReadAllText(welcomeEmailTemplatePath), stronglyTypedModel, null, null);

            // Send the email
            var email = new MailMessage()
            {
                Body = emailHtmlBody,
                IsBodyHtml = true,
                Subject = "Welcome (generated from strongly-typed model)"
            };

            email.To.Add(new MailAddress(stronglyTypedModel.Email, stronglyTypedModel.Name));
            // The From field will be populated from the app.config value by default

            var smtpClient = new SmtpClient();
            smtpClient.Send(email);

            // In the real world, you'll probably want to use async version instead:
            // await smtpClient.SendMailAsync(email);
        }

        static void DynamicModel()
        {
            var welcomeEmailTemplatePath = Path.Combine(TemplateFolderPath, "WelcomeEmailDynamic.cshtml");

            // Generate the email body from our email template
            // Note: the RazorEngine library supports using an anonymous type as a model without having to transform it to an Expando object first.
            var anonymousModel = new { Name = "Sarah", Email = "sarah@mail.example", IsPremiumUser = false };

            var templateService = new TemplateService();
            var emailHtmlBody = templateService.Parse(File.ReadAllText(welcomeEmailTemplatePath), anonymousModel, null, null);

            // Send the email
            var email = new MailMessage()
            {
                Body = emailHtmlBody,
                IsBodyHtml = true,
                Subject = "Welcome (generated from dynamic model)"
            };

            email.To.Add(new MailAddress(anonymousModel.Email, anonymousModel.Name));
            // The From field will be populated from the app.config value by default

            var smtpClient = new SmtpClient();
            smtpClient.Send(email);
        }
    }
}
