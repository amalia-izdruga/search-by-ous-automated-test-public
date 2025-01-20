using System;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace TestAlma
{
    public class AlmaTest
    {
        private IWebDriver driver;
        private readonly string ousId;
        private readonly string browser;
        private readonly string platform;
        private readonly string baseUrl = "https://almascience.eso.org/aq/";



        public AlmaTest()
        {
            // Read the OUS ID from environment variable or set the default value to uid://A002/X639a2a/X2a
            ousId = Environment.GetEnvironmentVariable("OUS_ID") ?? "uid://A002/X639a2a/X2a";

            // Read browser and platform from environment variables or use defaults
            browser = Environment.GetEnvironmentVariable("BROWSER") ?? "Edge"; // Default to Edge
            platform = Environment.GetEnvironmentVariable("PLATFORM") ?? "Windows"; // Default to Windows
        }

        [Fact]
        [Trait("Category", "Applications")]
        public void SearchObservationUnitSetTest()
        {
            // Initialize WebDriver based on the selected browser and platform
            driver = InitializeDriver(browser, platform);

            try
            {
                // Navigate to the base URL
                driver.Navigate().GoToUrl(baseUrl);

                // Wait maximum 10 seconds for the page to be loaded
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElement(By.Id("search")));

                // Find the header search element
                IWebElement search = driver.FindElement(By.Id("search"));
                search.Click();
                // Find the search box for the mous filter
                wait.Until(d => d.FindElement(By.Id("header-filter-mous")));
                IWebElement searchMember = driver.FindElement(By.Id("header-filter-mous"));
                // Enter the OUS ID into "Member ous id" search box
                searchMember.SendKeys(ousId);

                // Wait 5 seconds for the filter to be applied
                Thread.Sleep(5000);

                // Verify that at least one observation is displayed into the results table
                var results = driver.FindElements(By.ClassName("datatable-body-cell-label"));
                Assert.True(results.Count > 0, " No observing targets found for the given OUS ID.");
                Console.WriteLine($"Test Passed: Found one or multiple observing targets for OUS ID '{ousId}'.");
            }
            finally
            {
                // Clean up
                driver.Quit();
            }
        }

        private IWebDriver InitializeDriver(string browser, string platform)
        {
            IWebDriver driver = null;

            switch (browser.ToLower())
            {
                case "edge":
                    var edgeOptions = new EdgeOptions();
                    driver = new EdgeDriver(edgeOptions);
                    break;

                case "chrome":
                    var chromeOptions = new ChromeOptions();
                    driver = new ChromeDriver(chromeOptions);
                    break;

                case "firefox":
                    var firefoxOptions = new FirefoxOptions();
                    driver = new FirefoxDriver(firefoxOptions);
                    break;

                default:
                    throw new NotSupportedException($"Browser {browser} is not supported.");
            }

            driver.Manage().Window.Maximize();
            return driver;
        }
    }
}
