using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

namespace UITests;

public class Tests
{
    private AndroidDriver _androidDriver;

    [OneTimeSetUp]
    public void SetUp()
    {
        var serverUri = new Uri(
            Environment.GetEnvironmentVariable("APPIUM_HOST") ?? "http://127.0.0.1:4723/"
        );
        var driverOptions = new AppiumOptions()
        {
            AutomationName = AutomationName.AndroidUIAutomator2,
            PlatformName = "Android",
            DeviceName = "Android Emulator",
        };

        driverOptions.AddAdditionalAppiumOption("appPackage", "com.android.settings");
        driverOptions.AddAdditionalAppiumOption("appActivity", ".Settings");
        driverOptions.AddAdditionalAppiumOption("noReset", true);

        _androidDriver = new AndroidDriver(serverUri, driverOptions, TimeSpan.FromSeconds(180));
        _androidDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _androidDriver.Dispose();
    }

    [Test]
    public void TestBattery()
    {
        _androidDriver.StartActivity("com.android.settings", ".Settings");
        _androidDriver.FindElement(By.XPath("//*[@text='Battery']")).Click();
    }

    [Test]
    [Order(1)]
    public void ApplicationCanStart()
    {
        _androidDriver.StartActivity("io.kimgoetzke.shoppinglist", ".MainActivity");
        _androidDriver.GetScreenshot().SaveAsFile($"ApplicationCanStart.png");
    }
}
