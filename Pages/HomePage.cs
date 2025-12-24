using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace challenges.Pages
{
    internal class HomePage : BasePage
    {
        public HomePage(IWebDriver driver) : base(driver) { }

        // כפתור כניסה
        private IWebElement SignInLink => driver.FindElement(By.LinkText("Signin"));

        // הקישור בתפריט העליון (Navbar)
        private IWebElement SignOutNavLink => driver.FindElement(By.CssSelector("a[href='/signout']"));


        private By FinalLogoutButtonLocator => By.CssSelector(".btn-danger");
        private IList<IWebElement> PopularChallengeCards => driver.FindElements(By.CssSelector(".modern-card"));

       
        private IWebElement NextCarouselButton => driver.FindElement(By.CssSelector(".carousel-control-next"));

        public void ClickSignIn()
        {
            SignInLink.Click();
        }
        // פונקציה שמחזירה את שם האתגר הראשון ברשימה
        public string GetFirstPopularChallengeName()
        {
            if (PopularChallengeCards.Count > 0)
            {

                var titleElement = PopularChallengeCards[0].FindElement(By.CssSelector("h3.card-title"));
                return titleElement.Text.Trim();
            }
            throw new NoSuchElementException("לא נמצאו כרטיסיות אתגר בדף הבית");
        }

        // פונקציה ללחיצה על האתגר הראשון
        public void ClickFirstPopularChallenge()
        {
            if (PopularChallengeCards.Count > 0)
            {
                // שימוש ב-JS כדי ללחוץ ישירות על האלמנט גם אם משהו מסתיר אותו
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].click();", PopularChallengeCards[0]);
            }
            else
            {
                throw new NoSuchElementException("לא נמצאו כרטיסיות אתגר עם Class בשם 'modern-card'");
            }
        }
        public void FullSignOut()
        {
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                // 1. לחיצה בנאבבר
                js.ExecuteScript("arguments[0].click();", SignOutNavLink);
                Console.WriteLine("Clicked Signout in Navbar");

                // 2. המתנה קצרה מאוד - לבדוק אם אנחנו בדף ההתנתקות
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

                // 3. ניסיון לחיצה על הכפתור האדום רק אם הוא קיים
                var buttons = driver.FindElements(By.XPath("//button[contains(@class, 'btn-danger')]"));
                if (buttons.Count > 0 && buttons[0].Displayed)
                {
                    js.ExecuteScript("arguments[0].click();", buttons[0]);
                    Console.WriteLine("Clicked red button");
                }
            }
            catch {

                // 4. ניקוי אגרסיבי של כל שאריות המשתמש
                ((IJavaScriptExecutor)driver).ExecuteScript("window.localStorage.clear(); window.sessionStorage.clear();");
                driver.Manage().Cookies.DeleteAllCookies();
            }
        }
    } }
    
 
