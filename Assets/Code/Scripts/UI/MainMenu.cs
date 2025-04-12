using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject HomeMenu;
    [SerializeField]
    GameObject OptionsMenu;
    [SerializeField]
    Slider MapScaleSlider;
    [SerializeField]
    Slider FloodAmountSlider;
    [SerializeField]
    Button CreateMapButton;
    [SerializeField]
    TMP_Text ResultsText;
    [SerializeField]
    Button BackButton;

    TextAsset loadedFile;

    public void Start()
    {
        TTTEvents.FinishCreatingMap += OnFinishCreateMap;
        ToOptions();
        MapScaleSlider.GetComponentInParent<TMP_Text>().text = $"Map Scale:{(int)MapScaleSlider.value}";
        FloodAmountSlider.GetComponentInParent<TMP_Text>().text = $"Flood Amount: {Math.Round(FloodAmountSlider.value, 2)}";
        ToHome();
    }

    public void OnMapScaleChange()
    {
        int newValue = (int)MapScaleSlider.value;
        MapScaleSlider.GetComponentInParent<TMP_Text>().text = $"Map Scale: {newValue}";
        TTTEvents.MapScaleChangeEvent.Invoke(this, new MapScaleEventArgs
        {
            MapScale = newValue
        });
    }

    public void OnFloodAmountChange()
    {
        float newValue = FloodAmountSlider.value;
        FloodAmountSlider.GetComponentInParent<TMP_Text>().text = $"Flood Amount: {Math.Round(newValue, 2)}";
        TTTEvents.FloodIncrementChangeEvent.Invoke(this, new FloodEventArgs
        {
            CurrentWaterLevel = newValue
        });
    }

    public void OnSelectFile()
    {
        ResultsText.text = "";
        LoadExternalJson externalLoader = new();
        if (externalLoader.TryGetDataJson(out TextAsset textAsset))
        {
            loadedFile = textAsset;
            CreateMapButton.enabled = true;
        }
    }

    public void ToOptions()
    {
        HomeMenu.SetActive(false);
        OptionsMenu.SetActive(true);
        CreateMapButton.enabled = false;
        ResultsText.text = "";
    }

    public void ToHome()
    {
        HomeMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

    public void OnStartNewGame()
    {
        TTTEvents.ChangeBoardState.Invoke(this, new BoardStateEventArgs
        {
            NewBoardState = TurnTheTides.BoardState.Loading
        });
    }

    public void OnCreateMap()
    {
        BackButton.enabled = false;
        ResultsText.text = "Loading...";
        TTTEvents.CreateNewMap.Invoke(this.gameObject, new NewMapEventArgs()
        {
            DataFile = loadedFile,
            MapScale = (int)MapScaleSlider.value,
            FloodAmount = FloodAmountSlider.value
        });
    }

    public void OnClickQuit()
    {
        TTTEvents.QuitGame();
    }

    void OnFinishCreateMap(object sender, EventArgs e)
    {
        ResultsText.text = "Complete!";
        BackButton.enabled = true;
    }
}
