using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FmodAudioBusController : MonoBehaviour
{
    private FMOD.Studio.Bus bus;
    public string busPath;

    private Slider slider;

    [SerializeField]
    private float busVolume;

    // Start is called before the first frame update
    void Start()
    {
        bus = FMODUnity.RuntimeManager.GetBus(busPath);
        bus.getVolume(out busVolume);
        slider = GetComponent<Slider>();
        
    }

    public void SetBusVolume(float volume)
    {
        bus.setVolume(volume);
        bus.getVolume(out busVolume);
    }
}
