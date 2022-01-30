using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [HideInInspector]public GameObject choiceUI;
    [HideInInspector]public GameObject chanceUI;
    [HideInInspector]public GameObject bonusUI;
    [HideInInspector]public GameObject startUI;

    public GameObject pickNotes;

    public Slider scoreBar;
    public Image colorImage;
    public Slider timerBar;
    public Image timerFill;
    
    // Start is called before the first frame update
    void Start()
    {
        startUI = gameObject.transform.Find("Canvas/StartUI").gameObject;
        choiceUI = gameObject.transform.Find("Canvas/ChoiceUI").gameObject;
        chanceUI = gameObject.transform.Find("Canvas/ChanceUI").gameObject;
        bonusUI = gameObject.transform.Find("Canvas/BonusUI").gameObject;
    }
}
