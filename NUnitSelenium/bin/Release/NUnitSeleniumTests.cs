using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using NUnit.Framework;
using System.Threading;
using System.Collections.Generic;

namespace NUnitSelenium
{
    [TestFixture("chrome", "96", "Windows 10")]
    //[TestFixture("internet explorer", "11", "Windows 7")]
    //[TestFixture("firefox", "60", "Windows 7")]
    //[TestFixture("chrome", "71", "Windows 7")]
    //[TestFixture("internet explorer", "11", "Windows 10")]
    //[TestFixture("firefox", "58", "Windows 7")]
    //[TestFixture("chrome", "67", "Windows 7")]
    //[TestFixture("internet explorer", "10", "Windows 7")]
    //[TestFixture("firefox", "55", "Windows 7")]
    [Parallelizable(ParallelScope.Children)]
    public class NUnitSeleniumSample
    {
        public static string LT_USERNAME = Environment.GetEnvironmentVariable("LT_USERNAME") ==null ? "your username" : Environment.GetEnvironmentVariable("LT_USERNAME");
        public static string LT_ACCESS_KEY = Environment.GetEnvironmentVariable("LT_ACCESS_KEY") == null ? "your accessKey" : Environment.GetEnvironmentVariable("LT_ACCESS_KEY");
        public static bool tunnel = Boolean.Parse(Environment.GetEnvironmentVariable("LT_TUNNEL")== null ? "false" : Environment.GetEnvironmentVariable("LT_TUNNEL"));       
        public static string build = Environment.GetEnvironmentVariable("LT_BUILD") == null ? "your build name" : Environment.GetEnvironmentVariable("LT_BUILD");
        public static string seleniumUri = "https://beta-smartui-hub.lambdatest.com/wd/hub";


        ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();
        private String browser;
        private String version;
        private String os;

        public NUnitSeleniumSample(String browser, String version, String os)
        {
            this.browser = browser;
            this.version = version;
            this.os = os;
        }

        [SetUp]
        public void Init()
        {

            Dictionary<String, object> errorC = new Dictionary<string, object>();
            errorC.Add("red", 200);
            errorC.Add("green", 0);
            errorC.Add("blue", 255);


            Dictionary<String, object> output = new Dictionary<string, object>();
            output.Add("errorColor", errorC);
            output.Add("errorType", "movement");
            output.Add("transparency", 3.0);
            output.Add("largeImageThreshold", 300);
            output.Add("useCrossOrigin", false);
            output.Add("outputDiff", false);

            
            Dictionary<String, object> smartUI = new Dictionary<string, object>();
            smartUI.Add("scaleToSameSize",true);
            smartUI.Add("ignore", "antialiasing");
            smartUI.Add("output", output);


            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability(CapabilityType.BrowserName, browser);
            capabilities.SetCapability(CapabilityType.Version, version);
            capabilities.SetCapability(CapabilityType.Platform, os);
            capabilities.SetCapability("smartUI.options", smartUI);
            capabilities.SetCapability("smartUI.project", "NUnit Sample");

            //Requires a named tunnel.
            if (tunnel)
            {
                capabilities.SetCapability("tunnel", tunnel);
            }
            if (build != null)
            {
                capabilities.SetCapability("build", build);
            }
          
            capabilities.SetCapability("user", LT_USERNAME);
            capabilities.SetCapability("accessKey", LT_ACCESS_KEY);

            capabilities.SetCapability("name",
            String.Format("{0}:{1}",
            TestContext.CurrentContext.Test.ClassName,
            TestContext.CurrentContext.Test.MethodName));
            driver.Value = new RemoteWebDriver(new Uri(seleniumUri), capabilities, TimeSpan.FromSeconds(600));
            Console.Out.WriteLine(driver);
        }

        [Test]
       public void Todotest()
        {
            {
                Console.WriteLine("Navigating to todos app.");
                driver.Value.Navigate().GoToUrl("https://lambdatest.github.io/sample-todo-app/");

                driver.Value.FindElement(By.Name("li4")).Click();
                Console.WriteLine("Clicking Checkbox");
                driver.Value.FindElement(By.Name("li5")).Click();


                // If both clicks worked, then te following List should have length 2
                IList<IWebElement> elems = driver.Value.FindElements(By.ClassName("done-true"));
                // so we'll assert that this is correct.
                Assert.AreEqual(2, elems.Count);

                Console.WriteLine("Entering Text");
                driver.Value.FindElement(By.Id("sampletodotext")).SendKeys("Yey, The text is changed");
                driver.Value.FindElement(By.Id("addbutton")).Click();

                

                // For Smartui TakeScreenshot
                ((IJavaScriptExecutor)driver.Value).ExecuteScript("smartui.takeScreenshot");

                // lets also assert that the new todo we added is in the list
                string spanText = driver.Value.FindElement(By.XPath("/html/body/div/div/div/ul/li[6]/span")).Text;
                Assert.AreEqual("Yey, The text is changed", spanText);

                

            }
        }

        [TearDown]
        public void Cleanup()
        {
            bool passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
            try
            {
                // Logs the result to LambdaTest
                ((IJavaScriptExecutor)driver.Value).ExecuteScript("lambda-status=" + (passed ? "passed" : "failed"));
            }
            finally
            {
                
                // Terminates the remote webdriver session
                driver.Value.Quit();
            }
        }
    }
}
