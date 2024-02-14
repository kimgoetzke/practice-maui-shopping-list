using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace UITests;

[SetUpFixture]
public class AppiumSetup
{
    private static AppiumDriver? _driver;

    public static AppiumDriver AppiumDriver =>
        _driver ?? throw new NullReferenceException("AppiumDriver is null");

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        AppiumServerHelper.StartAppiumLocalServer();
        var androidOptions = new AppiumOptions
        {
            AutomationName = "UIAutomator2",
            PlatformName = "Android",
            PlatformVersion = "14",
            // App = Environment.GetEnvironmentVariable("SHOPPING_LIST_APK") + "/io.kimgoetzke.shoppinglist-Signed.apk",
        };
        androidOptions.AddAdditionalAppiumOption("appPackage", "io.kimgoetzke.shoppinglist");
        androidOptions.AddAdditionalAppiumOption("appActivity", ".MainActivity");
        androidOptions.AddAdditionalAppiumOption("avd", "Pixel_3a_API_34_extension_level_7_x86_64");

        _driver = new AndroidDriver(androidOptions);
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        _driver?.Quit();
        AppiumServerHelper.DisposeAppiumLocalServer();
    }
}
