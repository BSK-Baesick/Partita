using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [HideInInspector]public GameObject soloUI;
    [HideInInspector]public GameObject duetUI;

    public GameObject duetNotes;

    public Slider scoreBar;
    public Image buttonImage;
    public Slider timerBar;
    public Image timerFill;
    
    // Start is called before the first frame update
    void Start()
    {
        soloUI = gameObject.transform.Find("Canvas/SoloUI").gameObject;
        duetUI = gameObject.transform.Find("Canvas/DuetUI").gameObject;
    }
}
