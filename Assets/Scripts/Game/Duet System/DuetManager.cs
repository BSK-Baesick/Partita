using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetManager : MonoBehaviour
{
    DuetStateManager stateMg;
    UIManager UImg;

    void Start()
    {
        stateMg = GetComponentInChildren<DuetStateManager>();
        UImg = GetComponentInChildren<UIManager>();
    }

    void FixedUpdate()
    {
        UImg.scoreBar.value = stateMg.score;
        UImg.timerBar.value = stateMg.turnTime;
        
        switch(stateMg.bonusState.currentColor)
        {
            case 1:
                UImg.colorImage.color = Color.cyan;
                break;
            case 2:
                UImg.colorImage.color = Color.yellow;
                break;
            case 3:
                UImg.colorImage.color = Color.magenta;
                break;
        }

        UImg.timerBar.maxValue = stateMg.setTurnTime;

        if(stateMg.buttonPressed)
        {
            UImg.buttonImage.enabled = false;
            UImg.duetNotes.SetActive(false);
        }
        else
        {
            UImg.buttonImage.enabled = true;
            UImg.duetNotes.SetActive(true);
        }

        switch (Mathf.RoundToInt(stateMg.turnTime))
        {
            case 1:
                UImg.timerFill.color = Color.magenta;
                break;
            case 2:
                UImg.timerFill.color = Color.yellow;
                break;
            case 3:
                UImg.timerFill.color = Color.cyan;
                break;
            default:
                UImg.timerFill.color = Color.magenta;
                break;
        }

        switch(stateMg.turnType)
        {
            case 1:
                //player solo
                UImg.npcUI.SetActive(true);
                UImg.duetUI.SetActive(false);
                UImg.buttonImage.color = Color.cyan;
                break;
            case 2:
                //npc solo
                UImg.npcUI.SetActive(true);
                UImg.duetUI.SetActive(false);
                UImg.buttonImage.color = Color.magenta;
                break;
            case 3:
                //pickChoice
                UImg.npcUI.SetActive(false);
                UImg.duetUI.SetActive(true);
                break;
        }
    }
}
