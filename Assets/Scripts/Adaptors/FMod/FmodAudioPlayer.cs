using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodAudioPlayer : MonoBehaviour
{
   public FmodEventManager fmodEventManager;

    // Start is called before the first frame update
    void Start()
    {
        fmodEventManager = GetComponent<FmodEventManager>();
    }


    public void PlayMenuMusic()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventManager.music.musicMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
