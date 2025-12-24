using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using challenges.Pages;
using ExcelDataReader;
using System.Data;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium.Support.UI;

namespace challenges.Tests
{
    [TestClass]
    public class ChallengeTests
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
        public void ExecuteUploadChallengeTests()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // מציאת נתיב הקובץ בתיקיית ה-Data בצורה דינמית
            string projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string filePath = Path.Combine(projectDir, "Data", "TestData.xlsx");

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                    });

                    // 1. קודם כל מתחברים למערכת (שימוש בגיליון הראשון - אינדקס 0)
                    DataTable loginDt = result.Tables[0];
                    string adminUser = loginDt.Rows[0]["Username"].ToString();
                    string adminPass = loginDt.Rows[0]["Password"].ToString();

                    driver.Navigate().GoToUrl(url);
                    HomePage homePage = new HomePage(driver);
                    homePage.ClickSignIn();

                    LoginPage loginPage = new LoginPage(driver);
                    loginPage.Login(adminUser, adminPass);

                   
                    // מחכים עד שה-URL ישתנה לדף הבית (Home) אחרי שההודעה "התחברת בהצלחה" נעלמת
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(d => d.Url.Equals(url));

                    Console.WriteLine("Login successful and redirected to Home page.");
                    ;

                    Thread.Sleep(2000);

                    // 2. מעבר לגיליון האתגרים (Challenges)
                    DataTable challengeDt = result.Tables["Challenges"];
                    UploadChallengePage uploadPage = new UploadChallengePage(driver);

                    foreach (DataRow row in challengeDt.Rows)
                    {
                        // הגנה מפני שורות ריקות באקסל
                        if (string.IsNullOrEmpty(row["ChallengeName"].ToString())) break;

                        string cName = row["ChallengeName"].ToString();
                        string cDesc = row["Description"].ToString();
                        string cDays = row["Days"].ToString();



                       
                        string imgFileName = row["ImagePath"]?.ToString(); // קריאת השם מהאקסל
                        string fullImgPath = "";

                        // בדיקה: אם התא באקסל ריק, לא נחבר את המילה "Data"
                        if (!string.IsNullOrEmpty(imgFileName) && imgFileName.Trim().ToLower() != "none" && imgFileName.Trim() != "")
                        {
                            fullImgPath = Path.Combine(projectDir, "Data", imgFileName);
                        }

                        Console.WriteLine($"--- Uploading Challenge: {cName} ---");
                        // ניווט לדף העלאת אתגר 
                        driver.FindElement(By.LinkText("Upload challenge")).Click();
                        Thread.Sleep(1000);

                        
                        // מילוי הטופס ושליחה
                        uploadPage.CreateChallenge(cName, cDesc, cDays, fullImgPath);

                        //  המתנה קצרה שהודעת ההצלחה תופיע על המסך
                        WebDriverWait wait2 = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                        var successMessageElement = wait2.Until(d => d.FindElement(By.CssSelector("app-create-challenge p.success")));

                        string actualMessage = successMessageElement.Text;
                        Console.WriteLine($"Actual message found: {actualMessage}");

                        // בדיקה שההודעה מכילה את שם האתגר ואת מילת ההצלחה
                        string expectedPart = $"האתגר \"{cName}\" נוצר בהצלחה!";
                        Assert.IsTrue(actualMessage.Contains(expectedPart),
                            $"שגיאה: ההודעה שנמצאה היא '{actualMessage}' ולא הכילה '{expectedPart}'");

                        Console.WriteLine($"Challenge {cName} verified successfully!");

                        // המתנה קלה כדי לראות את ההודעה לפני שהיא נעלמת או עוברת דף
                        Thread.Sleep(2000);

                        Console.WriteLine($"Challenge {cName} uploaded successfully!");
                    }
                }
            }
        }
        [TestMethod]
        public void VerifyFullFlowBlockedForGuest()
        {
            // 1. ניווט לדף הבית כאורח (בלי לוגין!)
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(2000);

            //  לחיצה על כפתור "Upload challenge" בנאב-בר
            driver.FindElement(By.LinkText("Upload challenge")).Click();
            Console.WriteLine("Clicked Upload Challenge link as guest.");
            Thread.Sleep(2000);

            
            UploadChallengePage uploadPage = new UploadChallengePage(driver);

            // מזינים נתונים לדוגמה כדי לבדוק את השרת
            uploadPage.CreateChallenge("אתגר בדיקת אבטחה", "ניסיון העלאה ללא התחברות", "10", "");
            Console.WriteLine("Filled form details and clicked Submit.");
            Thread.Sleep(2000);

            //  אימות הודעת השגיאה הספציפית 
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // זיהוי האלמנט לפי ה-class שמופיע בתמונה: p.error
            var errorMsgElement = wait.Until(d => d.FindElement(By.CssSelector("app-create-challenge p.error")));

            string actualError = errorMsgElement.Text;
            string expectedError = "שגיאת אימות: עליך להיות מחובר כדי להעלות אתגר.";

            Console.WriteLine($"Found message: {actualError}");

            Assert.IsTrue(actualError.Contains("עליך להיות מחובר"),
                $"הופיעה שגיאה אחרת מהמצופה: {actualError}");

            Thread.Sleep(3000); 
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