using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodAudioPlayer : MonoBehaviour
{
    //HV: Event Manager Reference
    public FmodEventManager fmodEventManager;

    //HV: Duet Parameters 
    public string milliaDuetParameter = "Millia_duet";
    public string stephanDuetParameter = "Stephan_duet";
    public string veraDuetParameter = "Vera_duet";
    public string paschaDuetParameter = "Pascha_duet";
    public string zurabDuetParameter = "Zurab_duet";

    public string duetParameterName = "duet_character";

    void Start()
    {
        fmodEventManager = GetComponent<FmodEventManager>();
    }

    #region MUSIC
    //HV: MUSIC METHODS

    //HV: Music Duet (0 = Millia, 1 = Stephan, 2 = Vera, 3 = Pascha, 4 = Zurab)
    public void PlayMusicDuet(int parameterNumber)
    {
        FMOD.Studio.EventInstance duetMusic = FMODUnity.RuntimeManager.CreateInstance(fmodEventManager.music.musicDuet);
        duetMusic.setParameterByName(duetParameterName, parameterNumber);
        duetMusic.start();
        duetMusic.release();
    }

    //HV: Music Finale (0 = false, 1 = true)
    public void PlayMusicFinale(int milliaParameterint, int stephanParameterint, int veraParameterint, int paschaParameterint, int zurabParameterint)
    {
        FMOD.Studio.EventInstance finaleMusic = FMODUnity.RuntimeManager.CreateInstance(fmodEventManager.music.musicFinale);
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
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.music.musicMenu);
    }

    //HV: Music World
    public void PlayWorldMusic()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.music.musicWorld);
    }

    //HV: Getting Ready
    public void PlayMiniGameGettingReady()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.music.minigamegGettingReady);
    }

    #endregion

    #region SFX
    //HV: SFX METHODS

    //HV: Bus Stop Soundscape/Heavy Snow Day
    public void PlayBusStopSoundscape()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.sFX.busStopSoundscape);
    }

    //HV: Bonfire Soundscape/Heavy Snow Night
    public void PlayBonfireSoundscape()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.sFX.bonfireSoundscape);
    }

    //HV: Lover Talk
    public void PlayLoverTalk()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.sFX.loverTalk);
    }

    //HV: Millia Minigame
    public void PlayMinigameMillia()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.sFX.minigameMillia);
    }

    //HV: Pascha Minigame
    public void PlayMinigamePascha()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.sFX.minigamePascha);
    }

    //HV: Protag Minigame
    public void PlayMinigameProtag()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.sFX.minigameProtag);
    }

    //HV: Stephan Minigame
    public void PlayMinigameStephan()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.sFX.minigameStephan);
    }

    //HV: Vera Minigame
    public void PlayMinigameVera()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.sFX.minigameVera);
    }

    //HV: Zurab Minigame
    public void PlayMinigameZurab()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.sFX.minigameZurab);
    }

    //HV: Zurab Minigame
    public void PlayMinigameBirds()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.sFX.minigameBirds);
    }

    //HV: Minigame Birds
    public void PlayClickUI()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.sFX.uI_Click);
    }
    #endregion

}
