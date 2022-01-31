using Naninovel;
using UnityEngine;

[EditInProjectSettings]
public class FmodAudioConfiguration : Configuration
{

    [Header("Music & BGM's")]

    public FMODUnity.EventReference duet;

    public FMODUnity.EventReference finale;

    public FMODUnity.EventReference menu;

    public FMODUnity.EventReference world;

    public FMODUnity.EventReference gettingReady;


    [Header("Sound Effects (SFX)")]

    public FMODUnity.EventReference busStopSoundscape;

    public FMODUnity.EventReference bonfireSoundscape;

    public FMODUnity.EventReference loverTalk;

    public FMODUnity.EventReference uiClick;

    [Header("Minigame")]

    public FMODUnity.EventReference minigameMillia;

    public FMODUnity.EventReference minigamePascha;

    public FMODUnity.EventReference protag;

    public FMODUnity.EventReference minigameStephan;

    public FMODUnity.EventReference minigameVera;

    public FMODUnity.EventReference minigameZurab;

    public FMODUnity.EventReference minigameBirds;

}