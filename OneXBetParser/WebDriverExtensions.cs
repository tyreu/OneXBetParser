using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;

namespace TestParser
{
    public static class WebDriverExtensions
    {
        public static ReadOnlyCollection<IWebElement> FindElements(this IWebDriver driver, By by, uint timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElements(by));
            }
            return driver.FindElements(by);
        }
        public static IWebElement FindElement(this IWebElement driver, By by, bool handled)
        {
            IWebElement value = null;
            try
            {
                value = driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                value = null;
            }
            return value;
        }
    }
}
