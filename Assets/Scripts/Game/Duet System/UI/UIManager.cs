using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [HideInInspector]public GameObject npcUI;
    [HideInInspector]public GameObject duetUI;
    [HideInInspector]public GameObject chanceUI;
    [HideInInspector]public GameObject bonusUI;

    public GameObject duetNotes;

    public Slider scoreBar;
    public Image buttonImage;
    public Image colorImage;
    public Slider timerBar;
    public Image timerFill;
    
    // Start is called before the first frame update
    void Start()
    {
        bonusUI = gameObject.transform.Find("Canvas/ChanceUI").gameObject;
        npcUI = gameObject.transform.Find("Canvas/SoloUI").gameObject;
        duetUI = gameObject.transform.Find("Canvas/DuetUI").gameObject;
        bonusUI = gameObject.transform.Find("Canvas/BonusUI").gameObject;
    }
}
