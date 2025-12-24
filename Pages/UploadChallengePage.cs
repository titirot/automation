using OpenQA.Selenium;

namespace challenges.Pages
{
    public class UploadChallengePage
    {
        private IWebDriver driver;

        public UploadChallengePage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // איתור האלמנטים לפי ה-ID שמופיע בצילומי המסך
        private IWebElement NameInput => driver.FindElement(By.Id("name"));
        private IWebElement DescriptionTextArea => driver.FindElement(By.Id("description"));
        private IWebElement NumOfDaysInput => driver.FindElement(By.Id("numOfDays"));
        private IWebElement FileUploadInput => driver.FindElement(By.Id("file-upload"));
        private IWebElement SubmitButton => driver.FindElement(By.CssSelector("button[type='submit']"));

        // פונקציה למילוי טופס האתגר
        public void CreateChallenge(string name, string description, string days, string imagePath)
        {
            NameInput.Clear();
            NameInput.SendKeys(name);

            DescriptionTextArea.Clear();
            DescriptionTextArea.SendKeys(description);

            NumOfDaysInput.Clear();
            NumOfDaysInput.SendKeys(days);

            // העלאת קובץ בסלניום מתבצעת על ידי שליחת הנתיב לשדה ה-input
           
            if (!string.IsNullOrEmpty(imagePath) && imagePath != "none")
            {
                FileUploadInput.SendKeys(imagePath);
            }

            SubmitButton.Click();
        }
    }
}