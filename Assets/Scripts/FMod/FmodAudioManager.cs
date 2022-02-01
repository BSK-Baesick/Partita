using System;
using UniRx.Async;
using Naninovel;
using UnityEngine;

[InitializeAtRuntime] // makes the service auto-initialize with other built-in engine services
public class FmodAudioManager : IEngineService<FmodAudioConfiguration>, IFmodAudioManager
{
    public FmodAudioConfiguration Configuration { get; set; }

    public FmodAudioManager(FmodAudioConfiguration config)
    {
        // Engine service constructors are invoked when the engine is initializing;
        // remember that it's not safe to use other services here, as they are not initialized yet.
        // Instead, store references to the required services and use them in `InitializeServiceAsync()` method.

        Configuration = config;
    }

    public FmodAudioController audioController;

    public UniTask InitializeServiceAsync()
    {
        // Invoked when the engine is initializing, after services required in the constructor are initialized;
        audioController = Engine.CreateObject<FmodAudioController>();
        Engine.CreateObject<FMODUnity.StudioListener>();
        
        return UniTask.CompletedTask;
    }

    public void ResetService()
    {
        // Invoked when resetting engine state (eg, loading a script or starting a new game).

    }

    public FmodAudioController GetFmodAudioController()
    {
        return audioController;
    }

    public void DestroyService()
    {
        // Invoked when destroying the engine.
    }

    #region MUSIC
    //HV: MUSIC METHODS

    //HV: Music Duet (0 = Millia, 1 = Stephan, 2 = Vera, 3 = Pascha, 4 = Zurab)
    public void StartMusicDuet(int parameterNumber)
    {
        audioController.PlayMusicDuet(parameterNumber);
    }

    //HV: Music Finale (0 = false, 1 = true)
    public void StartMusicFinale(int milliaParameterint, int stephanParameterint, int veraParameterint, int paschaParameterint, int zurabParameterint)
    {
        audioController.PlayMusicFinale(milliaParameterint, stephanParameterint, veraParameterint, paschaParameterint, zurabParameterint);
    }

    //HV: Music Menu
    public void StartMenuMusic()
    {
        audioController.PlayMenuMusic();
    }

    public void EndMenuMusic()
    {
        audioController.StopMenuMusic();
    }

    //HV: Music World
    public void StartWorldMusic()
    {
        audioController.PlayWorldMusic();
    }

    //HV: Music World
    public void EndWorldMusic()
    {
        audioController.StopWorldMusic();
    }

    //HV: Getting Ready
    public void StartMiniGameGettingReady()
    {
        audioController.PlayMiniGameGettingReady();
    }

    public void EndMiniGameGettingReady()
    {
        audioController.StopWorldMusic();
    }

    //HV: Bus Stop Soundscape/Heavy Snow Day
    public void StartBusStopSoundscape()
    {
        audioController.PlayBusStopSoundscape();
    }

    public void EndBusStopSoundscape()
    {
        audioController.StopBusStopSoundscape();
    }

    //HV: Bonfire Soundscape/Heavy Snow Night
    public void StartBonfireSoundscape()
    {
        audioController.PlayBonfireSoundscape();
    }

    public void EndBonfireSoundscape()
    {
        audioController.StopBonfireSoundscape();
    }

    #endregion

    #region SFX
    //HV: SFX METHODS

    //HV: Lover Talk
    public void StartLoverTalk()
    {
        audioController.PlayLoverTalk();
    }

    //HV: Millia Minigame
    public void StartMinigameMillia()
    {
        audioController.PlayMinigameMillia();
    }

    //HV: Pascha Minigame
    public void StartMinigamePascha()
    {
        audioController.PlayMinigamePascha();
    }

    //HV: Protag Minigame
    public void StartMinigameProtag()
    {
        audioController.PlayMinigameProtag();
    }

    //HV: Stephan Minigame
    public void StartMinigameStephan()
    {
        audioController.PlayMinigameStephan();
    }

    //HV: Vera Minigame
    public void StartMinigameVera()
    {
        audioController.PlayMinigameVera();
    }

    //HV: Zurab Minigame
    public void StartMinigameZurab()
    {
        audioController.PlayMinigameZurab();
    }

    //HV: Zurab Minigame
    public void StartMinigameBirds()
    {
        audioController.PlayMinigameBirds();
    }

    public void StartClickUI()
    {
        audioController.PlayClickUI();
    }
    #endregion

}