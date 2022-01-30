using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodEventManager : MonoBehaviour
{
    public Music music;
    public SFX sFX;
}

[System.Serializable]
public class Music
{
    public FMODUnity.EventReference musicDuet;
    public FMODUnity.EventReference musicFinale;
    public FMODUnity.EventReference musicMenu;
    public FMODUnity.EventReference musicWorld;
    public FMODUnity.EventReference minigamegGettingReady;   
}

[System.Serializable]
public class SFX
{
    public FMODUnity.EventReference busStopSoundscape;
    public FMODUnity.EventReference bonfireSoundscape;
    public FMODUnity.EventReference loverTalk;
    public FMODUnity.EventReference minigameMillia;
    public FMODUnity.EventReference minigamePascha;
    public FMODUnity.EventReference minigameProtag;
    public FMODUnity.EventReference minigameStephan;
    public FMODUnity.EventReference minigameVera;
    public FMODUnity.EventReference minigameZurab;
    public FMODUnity.EventReference minigameBirds;
    public FMODUnity.EventReference uI_Click;
}