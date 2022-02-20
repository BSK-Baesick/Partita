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

    public FMODUnity.EventReference busStopSoundscape;

    public FMODUnity.EventReference bonfireSoundscape;


    [Header("Sound Effects (SFX)")]

    public FMODUnity.EventReference loverTalk;

    public FMODUnity.EventReference uiClick;

    public FMODUnity.EventReference gameOver;

    [Header("Minigame")]

    public FMODUnity.EventReference gettingReady;

    public FMODUnity.EventReference minigameMillia;

    public FMODUnity.EventReference minigamePascha;

    public FMODUnity.EventReference minigameStephan;

    public FMODUnity.EventReference minigameVera;

    public FMODUnity.EventReference minigameZurab;

    public FMODUnity.EventReference protag;

    public FMODUnity.EventReference minigameBirds;

}