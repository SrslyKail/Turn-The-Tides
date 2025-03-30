using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class TestUI
{


    /// <summary>
    /// Tests that the pollution meter can set its internal slider to a given value.
    /// </summary>
    [Test]
    public void TestPollutionMeterSetProgress()
    {
        GameObject uiContainer = new();
        PollutionMeter pollutionMeter = uiContainer.AddComponent<PollutionMeter>();
        Slider pollutionMeterSlider = uiContainer.AddComponent<Slider>();
        pollutionMeter.ProgressSlider = pollutionMeterSlider;
        pollutionMeter.SetProgress(0.5f);
        Assert.AreEqual(0.5f, pollutionMeterSlider.value);
        //The progress slider should be set to the expected value
    }

    /// <summary>
    /// Tests that the pollution meter can return the current value of its internal slider.
    /// </summary>
    [Test]
    public void TestPollutionMeterGetProgress()
    {
        GameObject uiContainer = new();
        PollutionMeter pollutionMeter = uiContainer.AddComponent<PollutionMeter>();
        Slider pollutionMeterSlider = uiContainer.AddComponent<Slider>();
        pollutionMeter.ProgressSlider = pollutionMeterSlider;

        pollutionMeterSlider.value = 0.69f;
        Assert.AreEqual(0.69f, pollutionMeter.GetProgress());
    }


    /// <summary>
    /// Tests that the next-turn button can be disabled.
    /// </summary>
    [Test]
    public void TestNextTurnButtonDisabled()
    {
        GameObject uiContainer = new();
        NextTurnButton nextTurnButton = uiContainer.AddComponent<NextTurnButton>();
        Button button = uiContainer.AddComponent<Button>();
        nextTurnButton.button = button;
        nextTurnButton.SetButtonEnabled(false);
        Assert.AreEqual(false, button.interactable);
    }
}
