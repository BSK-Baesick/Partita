using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [HideInInspector]public GameObject choiceUI;
    public Image c1Image;
    public Image c2Image;
    public Image c3Image;

    [HideInInspector]public GameObject chanceUI;
    [HideInInspector]public GameObject bonusUI;
    [HideInInspector]public GameObject startUI;

    public GameObject pickNotes;

    public Text scoreText;
    public Text turnText;
    public Slider scoreBar;
    public Image colorImage;
    public Slider timerBar;
    public Image timerFill;
    
    // Start is called before the first frame update
    void Start()
    {
        startUI = gameObject.transform.Find("Canvas/Panel/StartUI").gameObject;
        choiceUI = gameObject.transform.Find("Canvas/Panel/ChoiceUI").gameObject;
        chanceUI = gameObject.transform.Find("Canvas/Panel/ChanceUI").gameObject;
        bonusUI = gameObject.transform.Find("Canvas/Panel/BonusUI").gameObject;
    }
}
