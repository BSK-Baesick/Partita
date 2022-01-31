using System;
using UniRx.Async;
using Naninovel;

[InitializeAtRuntime] // makes the service auto-initialize with other built-in engine services
public class FmodAudioManager : IEngineService<FmodAudioConfiguration>, IStatefulService<SettingsStateMap>, IFmodAudioManager
{
    public FmodAudioConfiguration Configuration { get; }

    public FmodAudioManager(FmodAudioConfiguration config)
    {
        // Engine service constructors are invoked when the engine is initializing;
        // remember that it's not safe to use other services here, as they are not initialized yet.
        // Instead, store references to the required services and use them in `InitializeServiceAsync()` method.

        Configuration = config;
    }


    //HV: Duet Parameters 
    public string milliaDuetParameter = "Millia_duet";
    public string stephanDuetParameter = "Stephan_duet";
    public string veraDuetParameter = "Vera_duet";
    public string paschaDuetParameter = "Pascha_duet";
    public string zurabDuetParameter = "Zurab_duet";

    public string duetParameterName = "duet_character";

    public UniTask InitializeServiceAsync()
    {
        // Invoked when the engine is initializing, after services required in the constructor are initialized;

        return UniTask.CompletedTask;
    }

    public void ResetService()
    {
        // Invoked when resetting engine state (eg, loading a script or starting a new game).

    }

    public void DestroyService()
    {
        // Invoked when destroying the engine.
    }

    public void SaveServiceState(SettingsStateMap stateMap)
    {
        /* var settings = new Settings
        {
            MasterVolume = Configuration.masterVolume,
            MusicVolume = Configuration.musicVolume,
            SfxVolume = Configuration.sfxVolume,
        };

        stateMap.SetState(settings); */
    }

    public UniTask LoadServiceStateAsync(SettingsStateMap stateMap)
    {
        /* var settings = stateMap.GetState<Settings>();

        if (settings is null) // Apply default settings.
        {
            Configuration.masterVolume = GetMasterVolume();
            Configuration.musicVolume = GetMusicBusVolume();
            Configuration.sfxVolume = GetSfxBusVolume();
            return UniTask.CompletedTask;
        }

        Configuration.masterVolume = settings.MasterVolume;
        Configuration.musicVolume = settings.MusicVolume;
        Configuration.sfxVolume = settings.SfxVolume; */

        return UniTask.CompletedTask;
    }

    #region MUSIC
    //HV: MUSIC METHODS

    //HV: Music Duet (0 = Millia, 1 = Stephan, 2 = Vera, 3 = Pascha, 4 = Zurab)
    public virtual void PlayMusicDuet(int parameterNumber)
    {
        FMOD.Studio.EventInstance duetMusic = FMODUnity.RuntimeManager.CreateInstance(Configuration.duet);
        duetMusic.setParameterByName(duetParameterName, parameterNumber);
        duetMusic.start();
        duetMusic.release();
    }

    //HV: Music Finale (0 = false, 1 = true)
    public virtual void PlayMusicFinale(int milliaParameterint, int stephanParameterint, int veraParameterint, int paschaParameterint, int zurabParameterint)
    {
        FMOD.Studio.EventInstance finaleMusic = FMODUnity.RuntimeManager.CreateInstance(Configuration.finale);
        finaleMusic.setParameterByName(milliaDuetParameter, milliaParameterint);
        finaleMusic.setParameterByName(stephanDuetParameter, stephanParameterint);
        finaleMusic.setParameterByName(veraDuetParameter, veraParameterint);
        finaleMusic.setParameterByName(paschaDuetParameter, paschaParameterint);
        finaleMusic.setParameterByName(zurabDuetParameter, zurabParameterint);
        finaleMusic.start();
        finaleMusic.release();
    }

    //HV: Music Menu
    public void PlayMenuMusic()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.menu);
    }

    //HV: Music World
    public void PlayWorldMusic()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.world);
    }

    //HV: Getting Ready
    public void PlayMiniGameGettingReady()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.gettingReady);
    }

    #endregion

    #region SFX
    //HV: SFX METHODS

    //HV: Bus Stop Soundscape/Heavy Snow Day
    public void PlayBusStopSoundscape()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.busStopSoundscape);
    }

    //HV: Bonfire Soundscape/Heavy Snow Night
    public void PlayBonfireSoundscape()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.bonfireSoundscape);
    }

    //HV: Lover Talk
    public void PlayLoverTalk()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.loverTalk);
    }

    //HV: Millia Minigame
    public void PlayMinigameMillia()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.minigameMillia);
    }

    //HV: Pascha Minigame
    public void PlayMinigamePascha()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.minigamePascha);
    }

    //HV: Protag Minigame
    public void PlayMinigameProtag()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.protag);
    }

    //HV: Stephan Minigame
    public void PlayMinigameStephan()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.minigameStephan);
    }

    //HV: Vera Minigame
    public void PlayMinigameVera()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.minigameVera);
    }

    //HV: Zurab Minigame
    public void PlayMinigameZurab()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.minigameZurab);
    }

    //HV: Zurab Minigame
    public void PlayMinigameBirds()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.minigameBirds);
    }

    public void PlayClickUI()
    {
        FMODUnity.RuntimeManager.PlayOneShot(Configuration.uiClick);
    }
    #endregion
}
