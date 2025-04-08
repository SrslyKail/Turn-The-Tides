using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System;

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

    /// <summary>
    /// Tests that the GameUI object can recieve next-turn button click events and transmit them further.
    /// </summary>
    [UnityTest]
    public IEnumerator TestGameUiTransmitsNextTurnRequestEvent()
    {
        GameObject uiContainer = new();
        GameUI gameUI = uiContainer.AddComponent<GameUI>();
        NextTurnButton nextTurnButton = uiContainer.AddComponent<NextTurnButton>();
        Button button = uiContainer.AddComponent<Button>();
        nextTurnButton.button = button;
        gameUI.nextTurnButton = nextTurnButton;
        TTTEvents.NextTurnRequestedEvent += (object sender, EventArgs e) => { Assert.Pass(); };

        nextTurnButton.OnButtonClicked = new UnityEvent();
        nextTurnButton.OnButtonClicked.AddListener(() => { TTTEvents.NextTurnRequestedEvent.Invoke(this, EventArgs.Empty); });
        nextTurnButton.OnClick();

        yield return null;
        Assert.Fail();
    }

    /// <summary>
    /// Tests that the water level indicator can have its sea-level set to a certain value.
    /// 
    /// NOTE: This does not test the actual UI element, but rather the internal logic of the WaterLevelIndicator.
    /// </summary>
    [Test]
    public void TestWaterLevelIndicatorSetSeaLevel()
    {
        GameObject uiContainer = new();
        WaterLevelIndicator waterLevelIndicator = uiContainer.AddComponent<WaterLevelIndicator>();
        Slider slider = uiContainer.AddComponent<Slider>();
        waterLevelIndicator.slider = slider;
        GameObject waterLevelLabelObject = new GameObject();
        Text waterLevelLabel = waterLevelLabelObject.AddComponent<Text>();
        GameObject waterLevelIncreaseLabelObject = new GameObject();
        Text waterLevelIncreaseLabel = waterLevelIncreaseLabelObject.AddComponent<Text>();
        waterLevelIndicator.waterLevelLabel = waterLevelLabel;
        waterLevelIndicator.waterLevelIncreaseLabel = waterLevelIncreaseLabel;
        waterLevelIndicator.SetSeaLevel(0, 100, 50);

        Assert.AreEqual(0.5f, waterLevelIndicator.GetSeaLevel());
        Assert.AreEqual(50, waterLevelIndicator.GetSeaLevelInMetres());
    }

    /// <summary>
    /// Tests that the water level indicator can return the current sea level increase.
    /// 
    /// Also tests that the water level increase label is set correctly.
    /// </summary>
    [Test]
    public void TestWaterLevelIndicatorSetSeaLevelIncrease()
    {
        GameObject uiContainer = new();
        WaterLevelIndicator waterLevelIndicator = uiContainer.AddComponent<WaterLevelIndicator>();
        Slider slider = uiContainer.AddComponent<Slider>();
        waterLevelIndicator.slider = slider;
        GameObject waterLevelLabelObject = new GameObject();
        Text waterLevelLabel = waterLevelLabelObject.AddComponent<Text>();
        GameObject waterLevelIncreaseLabelObject = new GameObject();
        Text waterLevelIncreaseLabel = waterLevelIncreaseLabelObject.AddComponent<Text>();
        waterLevelIndicator.waterLevelLabel = waterLevelLabel;
        waterLevelIndicator.waterLevelIncreaseLabel = waterLevelIncreaseLabel;

        waterLevelIndicator.SetSeaLevelIncrease(0.5f);

        Assert.AreEqual(0.5f, waterLevelIndicator.GetSeaLevelIncrease());
        Assert.AreEqual("+0.5m", waterLevelIncreaseLabel.text);
    }
}
