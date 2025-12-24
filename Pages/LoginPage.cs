using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace challenges.Pages
{
    internal class LoginPage : BasePage

    {
        public LoginPage(IWebDriver driver) : base(driver) { }

      
        private IWebElement UserNameField => driver.FindElement(By.Id("username"));
        private IWebElement PasswordField => driver.FindElement(By.Id("password"));
        private IWebElement LoginButton => driver.FindElement(By.CssSelector("button[type='submit']"));

        //  הפעולה שמבצעת את הלוגין 
        public void Login(string user, string pass)
        {
            // ניקוי השדה לפני כתיבה 
            UserNameField.Clear();
            UserNameField.SendKeys(user);

            PasswordField.Clear();
            PasswordField.SendKeys(pass);

            // לחיצה על כפתור ההתחברות
            LoginButton.Click();
        }
        private IWebElement ErrorMessage => driver.FindElement(By.CssSelector(".message"));

        public string GetErrorMessageText()
        {
            try
            {
                return ErrorMessage.Text;
            }
            catch (NoSuchElementException)
            {
                return ""; // אם אין הודעת שגיאה
            }
        }
    }
}
