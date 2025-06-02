using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace LLMClippy
{
    internal static class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //using (IWebDriver driver = new EdgeDriver())
            //{
            //    driver.Navigate().GoToUrl("https://www.microsoft.com/");
            //    Console.WriteLine("Page title is: " + driver.Title);
            //    Console.ReadKey();
            //}


            // Build configuration from appsettings.json
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new ClippyForm());
        }
    }
}