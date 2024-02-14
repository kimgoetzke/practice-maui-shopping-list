namespace UITests;

public class UnitTest2 : BaseTest
{
    [Test]
    public void SampleTest()
    {
        App.GetScreenshot().SaveAsFile($"{nameof(SampleTest)}.png");
    }
}
