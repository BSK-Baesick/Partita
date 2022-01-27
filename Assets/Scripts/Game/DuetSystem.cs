using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuetSystem : MonoBehaviour
{
    [SerializeField]
    private float buttonChangeTime;
    public int score = 50;
    [SerializeField]
    private Slider scoreBar;
    public int lives;

    public Image buttonImage;
    public Image barImage;
    public Slider timerBar;
    [SerializeField]
    private RectTransform barMask;

    public int buttonState;
    public int barState;
    private int noteState;
    float rectSize = 100f;

    public GameObject[] pressFX;
    [SerializeField]
    private Transform fxSpawn;
    private int fxState;

    void Start()
    {
        GenerateButton();
        InvokeRepeating("Subtract", 1f, 1f);
    }
    void FixedUpdate()
    {
        float valueDecreaser = 1f * Time.deltaTime;
        rectSize -= 100f/buttonChangeTime * Time.deltaTime;
        timerBar.value -= valueDecreaser;
        scoreBar.value = score;
        barMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectSize);
    }

    void Update()
    {
        noteState = buttonState + barState;
        if(score <= 0)
            score = 0;
        
        switch(noteState)
        {
            case 2:
                fxState = 0;
                break;
            case 3:
                fxState = 0;
                break;
            case 4:
                fxState = 1;
                break;
            case 5:
                fxState = 2;
                break;
        }

        switch(buttonState)
        {
            case 1:
                buttonImage.color = Color.magenta;
                break;
            case 2:
                buttonImage.color = Color.cyan;
                break;
        }

        if(Input.GetKeyDown("space") && buttonState == 2 && noteState >= 4)
        {
            Debug.Log(noteState);
            score += noteState;
            noteState = 0;
            buttonState = 0;
            GameObject clone = Instantiate(pressFX[fxState], fxSpawn.position, fxSpawn.rotation);
            Destroy (clone, 1.0f);
            barImage.enabled = false;
            buttonImage.enabled = false;
        }

        if(buttonState == 2 && barState <= 1)
            buttonImage.color = Color.magenta;

        if(Input.GetKeyDown("space") && buttonState == 2 && barState == 1)
        {
            Debug.Log(noteState);
            score -= noteState;
            GameObject clone = Instantiate(pressFX[0], fxSpawn.position, fxSpawn.rotation);
            Destroy (clone, 5.0f);
            barImage.enabled = false;
            buttonImage.enabled = false;
        }
        
        if(Input.GetKeyDown("space") && buttonState == 1)
        {
            Debug.Log(noteState);
            score -= noteState;
            GameObject clone = Instantiate(pressFX[0], fxSpawn.position, fxSpawn.rotation);
            Destroy (clone, 5.0f);
            barImage.enabled = false;
            buttonImage.enabled = false;
        }
    }

    void Subtract()
    {
        barState -= 1;
    }

    void GenerateButton()
    {
        barState = Mathf.RoundToInt(buttonChangeTime);
        timerBar.maxValue = buttonChangeTime;
        timerBar.value = buttonChangeTime;
        rectSize = 100f;
        buttonState = Random.Range(1, 3);
        StartCoroutine(Timer());
        buttonImage.enabled = true;
        barImage.enabled = true;
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(buttonChangeTime);
        if(noteState > 3)
            score -= noteState;
        GenerateButton();
    }
}
