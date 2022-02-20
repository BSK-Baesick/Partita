using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using Naninovel;

public class FmodAudioController : MonoBehaviour
{

    //HV: Duet Parameters 
    private readonly string milliaDuetParameter = "Millia_duet";
    private readonly string stephanDuetParameter = "Stephan_duet";
    private readonly string veraDuetParameter = "Vera_duet";
    private readonly string paschaDuetParameter = "Pascha_duet";
    private readonly string zurabDuetParameter = "Zurab_duet";

    private readonly string duetParameterName = "duet_character";

    private readonly string musicHoldParameterName = "MusicHold";

    [SerializeField, Range(-12f, 12f)] 
    private float pitch;

    [SerializeField] private FMOD.Studio.EventInstance duetMusic;
    [SerializeField] private FMOD.Studio.EventInstance finaleMusic;
    [SerializeField] private FMOD.Studio.EventInstance menuMusic;
    [SerializeField] private FMOD.Studio.EventInstance worldMusic;
    [SerializeField] private FMOD.Studio.EventInstance gettingReadyMusic;
    [SerializeField] private FMOD.Studio.EventInstance busStopSoundscape;
    [SerializeField] private FMOD.Studio.EventInstance bonfireSoundscape;

    FmodAudioConfiguration fmodAudioConfiguration;

    private void Awake()
    {
        if (Engine.Initialized) GetFmodAudioConfiguration();
        else Engine.OnInitializationFinished += GetFmodAudioConfiguration;
    }

    private void GetFmodAudioConfiguration()
    {
        Engine.OnInitializationFinished -= GetFmodAudioConfiguration;
        fmodAudioConfiguration = Engine.GetConfiguration<FmodAudioConfiguration>();
    }

    #region MUSIC
    //HV: MUSIC METHODS

    //HV: Music Duet (0 = Millia, 1 = Stephan, 2 = Vera, 3 = Pascha, 4 = Zurab)
    public void PlayMusicDuet(int parameterNumber, int playMusic = 1)
    {
        var eventReference = fmodAudioConfiguration.duet;

        var musicHold = playMusic == 1 ? 1 : 0;

        duetMusic = FMODUnity.RuntimeManager.CreateInstance(eventReference);
        duetMusic.setParameterByName(duetParameterName, parameterNumber);
        duetMusic.setParameterByName(musicHoldParameterName, musicHold);
        duetMusic.start();
        duetMusic.release();
    }

    //HV: Music Finale (0 = false, 1 = true)
    public void PlayMusicFinale(int milliaParameterint, int stephanParameterint, int veraParameterint, int paschaParameterint, int zurabParameterint)
    {
        var eventReference = fmodAudioConfiguration.finale;
        finaleMusic = FMODUnity.RuntimeManager.CreateInstance(eventReference);
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
        var eventReference = fmodAudioConfiguration.menu;
        menuMusic = FMODUnity.RuntimeManager.CreateInstance(eventReference);
        menuMusic.start();
        menuMusic.release();
    }

    public void StopMenuMusic()
    {
        menuMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    //HV: Music World
    public void PlayWorldMusic()
    {
        var eventReference = fmodAudioConfiguration.world;
        worldMusic = FMODUnity.RuntimeManager.CreateInstance(eventReference);
        worldMusic.start();
        worldMusic.release();
    }

    //HV: Music World
    public void StopWorldMusic()
    {
        worldMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    //HV: Getting Ready
    public void PlayMiniGameGettingReady()
    {
        var eventReference = fmodAudioConfiguration.gettingReady;
        gettingReadyMusic = FMODUnity.RuntimeManager.CreateInstance(eventReference);
        gettingReadyMusic.start();
        gettingReadyMusic.release();
    }

    public void StopMiniGameGettingReady()
    {
        gettingReadyMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    //HV: Bus Stop Soundscape/Heavy Snow Day
    public void PlayBusStopSoundscape()
    {
        var eventReference = fmodAudioConfiguration.busStopSoundscape;
        busStopSoundscape = FMODUnity.RuntimeManager.CreateInstance(eventReference);
        busStopSoundscape.start();
        busStopSoundscape.release();
    }

    public void StopBusStopSoundscape()
    {
        busStopSoundscape.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    //HV: Bonfire Soundscape/Heavy Snow Night
    public void PlayBonfireSoundscape()
    {
        var eventReference = fmodAudioConfiguration.bonfireSoundscape;
        bonfireSoundscape = FMODUnity.RuntimeManager.CreateInstance(eventReference);
        bonfireSoundscape.start();
        bonfireSoundscape.release();
    }

    public void StopBonfireSoundscape()
    {
        bonfireSoundscape.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    #endregion

    #region SFX
    //HV: SFX METHODS

    //HV: Lover Talk
    public void PlayLoverTalk()
    {
        var eventReference = fmodAudioConfiguration.loverTalk;
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);
    }

    //HV: Millia Minigame
    public void PlayMinigameMillia()
    {
        var eventReference = fmodAudioConfiguration.minigameMillia;
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);
    }

    //HV: Pascha Minigame
    public void PlayMinigamePascha()
    {
        var eventReference = fmodAudioConfiguration.minigamePascha;
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);
    }

    //HV: Protag Minigame
    public void PlayMinigameProtag()
    {
        var eventReference = fmodAudioConfiguration.protag;
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);
    }

    //HV: Stephan Minigame
    public void PlayMinigameStephan()
    {
        var eventReference = fmodAudioConfiguration.minigameStephan;
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);
    }

    //HV: Vera Minigame
    public void PlayMinigameVera()
    {
        var eventReference = fmodAudioConfiguration.minigameVera;
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);
    }

    //HV: Zurab Minigame
    public void PlayMinigameZurab()
    {
        var eventReference = fmodAudioConfiguration.minigameZurab;
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);
    }

    //HV: Zurab Minigame
    public void PlayMinigameBirds()
    {
        var eventReference = fmodAudioConfiguration.minigameBirds;
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);
    }

    public void PlayClickUI()
    {
        var eventReference = fmodAudioConfiguration.uiClick;
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);
    }

    public void PlayGameOverSfx()
    {
        var eventReference = fmodAudioConfiguration.uiClick;
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);
    }

    #endregion
}
