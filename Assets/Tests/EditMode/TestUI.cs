using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;
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

    /// <summary>
    /// Tests that the next-turn button click event is invoked when called externally.
    /// </summary>
    [UnityTest]
    public IEnumerator TestNextTurnButtonOnButtonClicked()
    {
        GameObject uiContainer = new();
        NextTurnButton nextTurnButton = uiContainer.AddComponent<NextTurnButton>();
        Button button = uiContainer.AddComponent<Button>();
        nextTurnButton.button = button;
        nextTurnButton.OnButtonClicked = new UnityEvent();
        nextTurnButton.OnButtonClicked.AddListener(() => { Assert.Pass(); });
        nextTurnButton.OnClick();
        yield return null;
        Assert.Fail();
    }


    /// <summary>
    /// Tests that the next-turn button click event is invoked when the button it's connected to gets pressed.
    /// </summary>
    [UnityTest]
    public IEnumerator TestNextTurnButtonOnClickEventGetsPassedThrough()
    {
        GameObject uiContainer = new();
        NextTurnButton nextTurnButton = uiContainer.AddComponent<NextTurnButton>();
        Button button = uiContainer.AddComponent<Button>();
        nextTurnButton.button = button;
        nextTurnButton.OnButtonClicked = new UnityEvent();
        nextTurnButton.OnButtonClicked.AddListener(() => { Assert.Pass(); });
        button.onClick.AddListener(() => { nextTurnButton.OnClick(); });
        button.onClick.Invoke();
        yield return null;
        Assert.Fail();
    }

    /// <summary>
    /// Tests that no event is invoked when the next-turn button is clicked while disabled.
    /// </summary>
    [UnityTest]
    public IEnumerator TestNextTurnButtonNoEventWhenDisabled()
    {
        GameObject uiContainer = new();
        NextTurnButton nextTurnButton = uiContainer.AddComponent<NextTurnButton>();
        Button button = uiContainer.AddComponent<Button>();
        nextTurnButton.button = button;
        nextTurnButton.OnButtonClicked = new UnityEvent();
        nextTurnButton.OnButtonClicked.AddListener(() => { Assert.Fail(); });
        nextTurnButton.SetButtonEnabled(false);
        nextTurnButton.OnClick();
        yield return null;
        Assert.Pass();
    }
}
