using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace UITests;

public abstract class BaseTest
{
    protected AppiumDriver App => AppiumSetup.AppiumDriver;

    protected AppiumElement FindUiElement(string id)
    {
        return App.FindElement(
            App is WindowsDriver ? MobileBy.AccessibilityId(id) : MobileBy.Id(id)
        );
    }
}
