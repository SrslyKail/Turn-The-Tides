using System;
using TurnTheTides;
using UnityEngine;

/// <summary>
/// Manager for handling the state of the UI in the game.
/// <para>
/// State of the UI is set through the <see cref="TTTEvents.ChangeBoardState"/> event.
/// </para>
/// </summary>
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                UIManager found = Helper.FindOrCreateSingleton<UIManager>("Prefabs/Managers/UIManager");

                if (found.enabled == false)
                {
                    found.enabled = true;
                }

                _instance = found;
            }

            return _instance;
        }
    }

    [SerializeField]
    GameObject MainMenuGuiPrefab;
    [SerializeField]
    GameObject LoadingGuiPrefab;
    [SerializeField]
    GameObject GameGuiPrefab;

    GameObject MainMenuGui;
    GameObject LoadingGui;
    GameObject GameGui;

    private void Awake()
    {
        TTTEvents.ChangeBoardState += OnChangeBoardState;
        MainMenuGui = Instantiate(MainMenuGuiPrefab);
        LoadingGui = Instantiate(LoadingGuiPrefab);
        GameGui = Instantiate(GameGuiPrefab);
    }

    private void Start()
    {
        LoadingGui.SetActive(false);
        GameGui.SetActive(false);
    }

    private void OnChangeBoardState(object sender, EventArgs e)
    {
        BoardStateEventArgs args = e as BoardStateEventArgs;
        switch (args.NewBoardState)
        {
            case BoardState.MainMenu: {
                LoadingGui.SetActive(false);
                GameGui.SetActive(false);
                MainMenuGui.SetActive(true);
                break;
            }
            case BoardState.Loading: {
                LoadingGui.SetActive(true);
                GameGui.SetActive(false);
                MainMenuGui.SetActive(false);
                break;
            }
            case BoardState.NewBoard: {
                LoadingGui.SetActive(false);
                GameGui.SetActive(true);
                MainMenuGui.SetActive(false);
                break;
            }
            default: break;
        }
    }
}
