using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using challenges.Pages;
using System;
using System.Threading;
using OpenQA.Selenium.Support.UI;

namespace challenges.Tests
{
    [TestClass]
    public class HomePageTests
    {
        private IWebDriver driver;
        private string url = "http://localhost:4200/";

        [TestInitialize]
        public void Setup()
        {
            new WebDriverManager.DriverManager().SetUpDriver(new WebDriverManager.DriverConfigs.Impl.ChromeConfig());
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [TestMethod]
        public void VerifyPopularChallengeNavigation()
        {
            driver.Navigate().GoToUrl(url);
            HomePage homePage = new HomePage(driver);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            //  מחכים שהכרטיסיות יטענו 
            wait.Until(d => !string.IsNullOrEmpty(homePage.GetFirstPopularChallengeName()));

            //  שמירת השם המדויק 
            string expectedName = homePage.GetFirstPopularChallengeName();
            Console.WriteLine($"שם האתגר שזיהינו בבית: {expectedName}");

            //  לחיצה עם JS
            homePage.ClickFirstPopularChallenge();

            // אימות בדף הפירוט  h1.challenge-name 
            IWebElement actualTitleElement = wait.Until(d => d.FindElement(By.CssSelector("h1.challenge-name")));
            string actualTitle = actualTitleElement.Text.Trim();
            Console.WriteLine($"הכותרת שנמצאה בדף הפירוט: {actualTitle}");

            //  השוואה סופית
            Assert.AreEqual(expectedName, actualTitle, "שם האתגר בדף הפירוט לא תואם לשם בדף הבית!");
           
            Thread.Sleep(2000); 
        }
        [TestCleanup]
        public void Teardown()
        {
            if (driver != null)
            {
                driver.Quit();
            }
        }
    }
}