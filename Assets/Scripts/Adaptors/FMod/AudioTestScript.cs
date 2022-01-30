using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTestScript : MonoBehaviour
{
    public FmodAudioPlayer fmodAudioPlayer;

    // Start is called before the first frame update
    void Start()
    {
        fmodAudioPlayer = GetComponent<FmodAudioPlayer>();
        fmodAudioPlayer.PlayMinigameBirds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
