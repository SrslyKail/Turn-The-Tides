using System;
using TurnTheTides;
using UnityEngine;

/// <summary>
/// Object to manage the state of the audio player.
/// Listens for the <see cref="TTTEvents.ChangeBoardState"/> event to change the music.
/// 
/// Should be a singleton, so do not instanciate it directly. Access it though MusicManager.Instance instead.
/// <para>
/// Written by Gurjeet Bhangoo, ported to a singleton by Corey Buchan.
/// </summary>
public class MusicManager : MonoBehaviour
{
    /// <summary>
    /// The instance of the MusicManager. This is a singleton, so it should only be accessed through the Instance property.
    /// </summary>
    private static MusicManager _instance;

    /// <summary>
    /// Gets the instance of the MusicManager. If it doesn't exist, it will create one.
    /// </summary>
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

    // Audio sources for different board states.
    // I'm just gonna hope these names are self-explanatory.
    [Header("Audio Sources")]
    public AudioClip MainMenuMusic;
    public AudioClip NewBoardMusic;
    public AudioClip LowPollutionMusic;
    public AudioClip ModeratePollutionMusic;
    public AudioClip HighPollutionMusic;
    public AudioClip GameOverMusic;

    /// <summary>
    /// The reference to the audio player component.
    /// </summary>
    [Header("References")]
    public AudioSource AudioPlayer;

    /// <summary>
    /// Called when the object is created.
    /// </summary>
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

    /// <summary>
    /// Updates the current state of the dynamic audio player.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Plays the current music clip.
    /// </summary>
    private void PlayMusic()
    {
        if (AudioPlayer.isPlaying)
        {
            AudioPlayer.Stop();
        }

        AudioPlayer.Play();
    }

    /// <summary>
    /// Stops the current music clip.
    /// </summary>
    private void StopMusic()
    {
        AudioPlayer.Stop();
    }

    /// <summary>
    /// Pauses the current music clip.
    /// </summary>
    private void PauseMusic()
    {
        AudioPlayer.Pause();
    }

    /// <summary>
    /// Unpauses the current music clip.
    /// </summary>
    private void UnpauseMusic()
    {
        AudioPlayer.UnPause();
    }
}
