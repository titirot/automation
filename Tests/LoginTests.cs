using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using challenges.Pages;
using ExcelDataReader;
using System.Data;
using System;
using System.IO;
using System.Threading;

namespace challenges.Tests
{
    [TestClass]
    public class LoginTests
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
        public void ExecuteLoginTestsFromExcel()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TestData.xlsx");

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                    });

                    DataTable dt = result.Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        if (string.IsNullOrEmpty(row["Username"].ToString()))
                        {
                            break;
                        }
                        // 1. קריאת נתונים מהאקסל 
                        string username = row["Username"].ToString();
                        string password = row["Password"].ToString();

                        // המרת ערך ה-TRUE/FALSE מהאקסל למשתנה בוליאני בקוד
                        bool isOK = bool.Parse(row["isOK"].ToString());

                        // 2. הכנת דפדפן נקייה לכל משתמש (מניעת ערבוב סשנים)
                        driver.Manage().Cookies.DeleteAllCookies();
                        driver.Navigate().GoToUrl(url);
                        ((IJavaScriptExecutor)driver).ExecuteScript("window.localStorage.clear(); window.sessionStorage.clear();");
                        driver.Navigate().Refresh();
                        Thread.Sleep(2000); // המתנה לטעינת אנגולר

                        Console.WriteLine($"--- Starting test for: {username} (Expected Success: {isOK}) ---");

                        HomePage homePage = new HomePage(driver);

                        
                        if (driver.FindElements(By.LinkText("Signin")).Count == 0)
                        {
                            homePage.FullSignOut();
                            driver.Navigate().GoToUrl(url);
                            Thread.Sleep(1000);
                        }

                        // 3. תהליך ההתחברות
                        homePage.ClickSignIn();
                        LoginPage loginPage = new LoginPage(driver);
                        loginPage.Login(username, password);
                        Thread.Sleep(2000);

                        // בדיקת התוצאה לפי הציפייה באקסל
                        if (isOK)
                        {
                            // בדיקת הצלחה למשתמשים תקינים (isOK = TRUE)
                            bool isLoginSuccessful = driver.PageSource.Contains("התחברת בהצלחה !") ||
                                                     driver.PageSource.Contains("שלום " + username);

                            Assert.IsTrue(isLoginSuccessful, $"המשתמש {username} היה אמור להצליח, אבל ההתחברות נכשלה!");
                            Console.WriteLine($"{username} logged in successfully.");

                            // התנתקות מסודרת לקראת המשתמש הבא
                            homePage.FullSignOut();
                        }
                        else
                        {
                            // בדיקת חסימה למשתמשים שגויים (isOK = FALSE)
                           
                            var errorMessages = driver.FindElements(By.CssSelector(".message"));
                            bool hasError = errorMessages.Count > 0 && errorMessages[0].Displayed;

                            Assert.IsTrue(hasError, $"המשתמש {username} היה אמור להיכשל, אבל לא הופיעה הודעת שגיאה!");

                            if (hasError)
                            {
                                string errorText = errorMessages[0].Text;
                                // אימות שטקסט השגיאה מכיל את המילה "שגויים" כפי שמופיע באתר
                                Assert.IsTrue(errorText.Contains("שגויים"), $"טקסט השגיאה לא תואם למצופה: {errorText}");
                                Console.WriteLine($"Blocked invalid user {username} with message: {errorText}");
                            }
                        }

                        Console.WriteLine($"Finished user: {username}");
                    }
                }
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}