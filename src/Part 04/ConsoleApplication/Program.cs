using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Mvc.Razor;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;
using ConsoleApplication.Models;
using Microsoft.CSharp;
using RazorEngine.Templating;
using RazorEngineHost = RazorEngine.Compilation.RazorEngineHost;

namespace ConsoleApplication
{
    class Program
    {
        static readonly string TemplateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RazorTemplates");

        static void Main(string[] args)
        {
            /* 
             * This program lets you take a peek behind the scene by displaying the source code of the class generated 
             * by the Razor parser when it parses a Razor template. 
             * 
             * We'll use a very simple Razor template you'll find in RazorTemplates/SimpleHtmlDocument.cshtml. 
             * This template takes a strongly-typed model of type ConsoleApplication.Models.WelcomeModel.
             * 
             * We'll first parse it with the ASP.NET Razor View Engine and then with the RazorEngine library.
             * 
             * The generated code will be placed in GeneratedByAspNetRazorViewEngine.cs and GeneratedByRazorEngine.cs
             * respectively in the executable's folder.
             * 
             */

            var templatePath = Path.Combine(TemplateFolderPath, "SimpleHtmlDocument.cshtml");

            Console.WriteLine("Generating Razor view source code using the ASP.NET Razor View Engine...");
            File.WriteAllText("GeneratedByAspNetRazorViewEngine.cs", GenerateCodeWithAspNetRazorViewEngine(templatePath));
            Console.WriteLine("Done. Stored in " + Path.GetFullPath("GeneratedByAspNetRazorViewEngine.cs"));

	        Console.WriteLine();

            Console.WriteLine("Generating Razor view source code using the RazorEngine library...");
            File.WriteAllText("GeneratedByRazorEngine.cs", GenerateCodeWithRazorEngine(templatePath, typeof(WelcomeModel)));
            Console.WriteLine("Done. Stored in " + Path.GetFullPath("GeneratedByRazorEngine.cs"));

            Console.WriteLine();
            Console.WriteLine("All done.");

            Console.ReadLine();
        }

        public static string GenerateCodeWithAspNetRazorViewEngine(string razorTemplatePath)
        {
            //-- Configure the code-generator
            var host = new MvcWebPageRazorHost("/", razorTemplatePath);

            //-- Parse the template into a CodeDOM graph (see http://msdn.microsoft.com/en-us/library/y2k85ax6(v=vs.110).aspx for an overview of what CodeDOM is)
            var engine = new RazorTemplateEngine(host);
            GeneratorResults results;

            using (var reader = new StringReader(File.ReadAllText(razorTemplatePath)))
            {
                results = engine.GenerateCode(reader);
            }

            //-- Generate C# code from the CodeDOM graph
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                new CSharpCodeProvider().GenerateCodeFromCompileUnit(results.GeneratedCode, writer, new CodeGeneratorOptions());
                return builder.ToString();
            }
        }

        public static string GenerateCodeWithRazorEngine(string razorTemplatePath, Type modelType)
        {
            //-- Configure the code-generator
            var host = new RazorEngineHost(new RazorEngine.Compilation.CSharp.CSharpRazorCodeLanguage(true), () => new HtmlMarkupParser())
            {
                DefaultBaseTemplateType = typeof(TemplateBase<>),
                DefaultModelType = modelType,
                DefaultBaseClass = typeof(TemplateBase<>).FullName,
                DefaultClassName = "RazorViewGeneratedByRazorEngine",
                DefaultNamespace = "CompiledRazorTemplates.Dynamic",
                GeneratedClassContext = new GeneratedClassContext("Execute", "Write", "WriteLiteral",
                                                                  "WriteTo", "WriteLiteralTo",
                                                                  "RazorEngine.Templating.TemplateWriter",
                                                                  "DefineSection")
                {
                    ResolveUrlMethodName = "ResolveUrl"
                }
            };

            //-- Parse the template into a CodeDOM graph (see http://msdn.microsoft.com/en-us/library/y2k85ax6(v=vs.110).aspx for an overview of what CodeDOM is)
            var engine = new RazorTemplateEngine(host);
            GeneratorResults result;

            using (var reader = new StringReader(File.ReadAllText(razorTemplatePath)))
            {
                result = engine.GenerateCode(reader);
            }

            //-- Generate C# code from the CodeDOM graph
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                new CSharpCodeProvider().GenerateCodeFromCompileUnit(result.GeneratedCode, writer, new CodeGeneratorOptions());
                return builder.ToString();
            }
        }
    }
}
