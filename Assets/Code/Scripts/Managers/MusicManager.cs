using System;
using TurnTheTides;
using UnityEngine;

/// <summary>
/// Object to manage the state of the audio player.
/// Listens for the <see cref="TTTEvents.ChangeBoardState"/> event to change the music.
/// 
///Should be a singleton, so do not instanciate it directly. Access it though MusicManager.Instance instead.
/// </summary>
public class MusicManager : MonoBehaviour
{
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

    [Header("Audio Sources")]
    public AudioClip MainMenuMusic;
    public AudioClip NewBoardMusic;
    public AudioClip LowPollutionMusic;
    public AudioClip ModeratePollutionMusic;
    public AudioClip HighPollutionMusic;
    public AudioClip GameOverMusic;

    [Header("References")]
    public AudioSource AudioPlayer;

    private void Start()
    {
        DontDestroyOnLoad(this);
        SingletonCheck();
        AudioPlayer.clip = MainMenuMusic;
        TTTEvents.ChangeBoardState += UpdateAudioPlayer;

        PlayMusic();
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

    private void UpdateAudioPlayer(object sender, EventArgs e)
    {
        BoardStateEventArgs args = (BoardStateEventArgs)e;
        // Loop audio if we're not at game over
        AudioPlayer.loop = args.NewBoardState != BoardState.GameOver;

        switch (args.NewBoardState)
        {
            case BoardState.GameOver:
                AudioPlayer.clip = GameOverMusic;
                break;
            case BoardState.MainMenu:
                AudioPlayer.clip = MainMenuMusic;
                break;
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
            case BoardState.Loading:
                break; // Keeping playing the current music.
            default:
                Debug.LogWarning("No music for this board state.");
                break;
        }

        PlayMusic();
    }

    private void PlayMusic()
    {
        if (AudioPlayer.isPlaying)
        {
            AudioPlayer.Stop();
        }

        AudioPlayer.Play();
    }

    private void StopMusic()
    {
        AudioPlayer.Stop();
    }

    private void PauseMusic()
    {
        AudioPlayer.Pause();
    }

    private void UnpauseMusic()
    {
        AudioPlayer.UnPause();
    }
}
