using TurnTheTides;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public enum MusicCategory
    {
        MainMenu,
        MainGame,
        GameOver
    }

    public enum BoardState
    {
        None,
        NewBoard,
        LowPollution,
        ModeratePollution,
        HighPollution
    }

    private static MusicManager _instance;
    public static MusicManager Instance
    {
        get
        {
            if (_instance == null)
            {
                MusicManager found = Helper.FindOrCreateSingleton<MusicManager>("Prefabs/Managers/MusicManager");
                if (found.enabled == false)
                {
                    found.enabled = true;
                }
                _instance = found;
            }
            return _instance;
        }
    }

    public MusicCategory CurrentMusicCategory = MusicCategory.MainMenu;

    public BoardState CurrentBoardState = BoardState.None;

    public bool PlayOnAwake = false;

    [Header("Audio Sources")]
    public AudioClip MainMenuMusic;
    public AudioClip NewBoardMusic;
    public AudioClip LowPollutionMusic;
    public AudioClip ModeratePollutionMusic;
    public AudioClip HighPollutionMusic;
    public AudioClip GameOverMusic;

    [Header("References")]
    public AudioSource AudioPlayer;

    public void Start()
    {
        SingletonCheck();
        if (PlayOnAwake)
        {
            PlayMusic();
        }
    }
    private void SingletonCheck()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        if (_instance != null && _instance != this)
        {
            Helper.SmartDestroy(gameObject);
        }
    }

    private void UpdateAudioPlayer()
    {
        AudioPlayer.loop = (CurrentMusicCategory != MusicCategory.GameOver);

        if (CurrentMusicCategory != MusicCategory.MainGame)
        {
            CurrentBoardState = BoardState.None;
        }

        switch (CurrentMusicCategory)
        {
            case MusicCategory.GameOver:
                AudioPlayer.clip = GameOverMusic;
                break;
            case MusicCategory.MainMenu:
                AudioPlayer.clip = MainMenuMusic;
                break;
            case MusicCategory.MainGame:
                switch (CurrentBoardState)
                {
                    case BoardState.NewBoard:
                        AudioPlayer.clip = NewBoardMusic;
                        break;
                    case BoardState.LowPollution:
                        AudioPlayer.clip = LowPollutionMusic;
                        break;
                    case BoardState.ModeratePollution:
                        AudioPlayer.clip = ModeratePollutionMusic;
                        break;
                    case BoardState.HighPollution:
                        AudioPlayer.clip = HighPollutionMusic;
                        break;
                    default:
                        Debug.LogWarning("No music for this board state.");
                        break;
                }
                break;
            default:
                Debug.LogWarning("No music for this category.");
                break;
        }
    }

    public void PlayMusic()
    {
        if (AudioPlayer.isPlaying)
        {
            AudioPlayer.Stop();
        }
        UpdateAudioPlayer();
        AudioPlayer.Play();
    }

    public void StopMusic()
    {
        AudioPlayer.Stop();
    }

    public void PauseMusic()
    {
        AudioPlayer.Pause();
    }

    public void UnpauseMusic()
    {
        AudioPlayer.UnPause();
    }
}
