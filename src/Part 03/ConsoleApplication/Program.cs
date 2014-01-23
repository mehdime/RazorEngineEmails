using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication.Models;
using RazorEngine;
using RazorEngine.Configuration.Xml;
using RazorEngine.Templating;

namespace ConsoleApplication
{
    class Program
    {
        static readonly string TemplateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");

        static void Main(string[] args)
        {
            /*
             * Quick benchmark demonstrating the effect of using RazorEngine's caching functionality.
             * 
             * We'll generate the same email 3 times in a row - first without caching and then with caching.
             * We'll measure and print how long each try takes.
             */

            var welcomeEmailTemplate = File.ReadAllText(Path.Combine(TemplateFolderPath, "WelcomeEmail.cshtml"));
            var model = new UserModel() { Name = "Sarah", Email = "sarah@mail.example", IsPremiumUser = false };

            var templateService = new TemplateService();

            // Add the default namespaces that will be automatically imported in all template classes
            templateService.AddNamespace("ConsoleApplication.Models");

            //-- Warm-up
            var emailHtmlBody = templateService.Parse(welcomeEmailTemplate, model, null, null);
            Stopwatch watch = new Stopwatch();

            //-- Without cache
            Console.WriteLine("Without cache: ");
            for (int i = 1; i < 4; i++)
            {
                watch.Start();
                emailHtmlBody = templateService.Parse(welcomeEmailTemplate, model, null, null);
                watch.Stop();

                Console.WriteLine("Try #{0}: {1}ms", i, watch.ElapsedMilliseconds);
                watch.Reset();
            }

            //-- With cache
            Console.WriteLine("With cache: ");
            for (int i = 1; i < 4; i++)
            {
                watch.Start();
                emailHtmlBody = templateService.Parse(welcomeEmailTemplate, model, null, "Welcome");
                watch.Stop();

                Console.WriteLine("Try #{0}: {1}ms", i, watch.ElapsedMilliseconds);
                watch.Reset();
            }

            Console.ReadLine();

            /* 
             * Example output:
             * 
Without cache:
Try #1: 116ms
Try #2: 114ms
Try #3: 118ms
With cache:
Try #1: 114ms
Try #2: 0ms
Try #3: 0ms
             * 
             */
        }
    }
}
