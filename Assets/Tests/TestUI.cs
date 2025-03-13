using NUnit.Framework;

public class TestUI
{
    /// <summary>
    /// Test that the pollution meter can be set to a specific value and that the value is stored correctly
    /// Important because this is what the user sees to know how much pollution they have
    /// </summary>
    [TestCase]
    public void TestPollutionMeterSetProgress()
    {
        //The progress slider should be set to the expected value
    }

    /// <summary>
    /// Test that the pollution meter can return the current value
    /// Important because the user sees this value to know how much pollution they have
    /// </summary>
    [TestCase]
    public void TestPollutionMeterGetProgress()
    {
        //The progress slider should return the expected value
    }

}
