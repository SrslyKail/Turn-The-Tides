using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to manage the main menu.
/// 
/// Written by Ben Henry.
/// </summary>
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
    /// <summary>
    /// Called once before the application starts.
    /// </summary>
    public void Awake()
    {
        TTTEvents.FinishCreatingMap += OnFinishCreateMap;
    }

    /// <summary>
    /// Called when the object is created.
    /// </summary>
    public void Start()
    {
        ToOptions();
        MapScaleSlider.GetComponentInParent<TMP_Text>().text = $"Map Scale:{(int)MapScaleSlider.value}";
        FloodAmountSlider.GetComponentInParent<TMP_Text>().text = $"Flood Amount: {Math.Round(FloodAmountSlider.value, 2)}";
        ToHome();
    }

    /// <summary>
    /// Called when the map scale slider is changed.
    /// </summary>
    public void OnMapScaleChange()
    {
        int newValue = (int)MapScaleSlider.value;
        MapScaleSlider.GetComponentInParent<TMP_Text>().text = $"Map Scale: {newValue}";
        TTTEvents.MapScaleChangeEvent.Invoke(this, new MapScaleEventArgs
        {
            MapScale = newValue
        });
    }

    /// <summary>
    /// Called when the flood amount slider is changed.
    /// </summary>
    public void OnFloodAmountChange()
    {
        float newValue = FloodAmountSlider.value;
        FloodAmountSlider.GetComponentInParent<TMP_Text>().text = $"Flood Amount: {Math.Round(newValue, 2)}";
        TTTEvents.FloodIncrementChangeEvent.Invoke(this, new FloodEventArgs
        {
            CurrentWaterLevel = newValue
        });
    }

    /// <summary>
    /// Called when the user selects a file to load.
    /// </summary>
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

    /// <summary>
    /// Called when the user clicks the Options button.
    /// </summary>
    public void ToOptions()
    {
        HomeMenu.SetActive(false);
        OptionsMenu.SetActive(true);
        CreateMapButton.enabled = false;
        ResultsText.text = "";
    }

    /// <summary>
    /// Called when the home menu is returned to from the options menu.
    /// </summary>
    public void ToHome()
    {
        HomeMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

    /// <summary>
    /// Called when the user clicks the Start button.
    /// </summary>
    public void OnStartNewGame()
    {
        TTTEvents.ChangeBoardState.Invoke(this, new BoardStateEventArgs
        {
            NewBoardState = TurnTheTides.BoardState.Loading
        });
    }

    /// <summary>
    /// Called when the user clicks the Create Map button.
    /// </summary>
    public void OnCreateMap()
    {
        BackButton.enabled = false;
        ResultsText.text = "Loading...";
        TTTEvents.CreateNewMap.Invoke(gameObject, new NewMapEventArgs()
        {
            DataFile = loadedFile,
            MapScale = (int)MapScaleSlider.value,
            FloodAmount = FloodAmountSlider.value
        });
    }

    /// <summary>
    /// Called when the user clicks the Quit button.
    /// </summary>
    public void OnClickQuit()
    {
        TTTEvents.QuitGame();
    }

    /// <summary>
    /// Called when the map is finished being generated.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnFinishCreateMap(object sender, EventArgs e)
    {
        ResultsText.text = "Complete!";
        BackButton.enabled = true;
    }
}
