using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        
        UImg.turnText.text = stateMg.playerState.turns + "x";

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

        if(stateMg.playerState.choices.Count != 0)
        {
            switch(stateMg.playerState.choices[0])
            {
                case 1:
                    UImg.c1Image.color = Color.magenta;
                    break;
                case 2:
                    UImg.c1Image.color = Color.yellow;
                    break;
                case 3:
                    UImg.c1Image.color = Color.cyan;
                    break;
            }

            switch(stateMg.playerState.choices[1])
            {
                case 1:
                    UImg.c2Image.color = Color.magenta;
                    break;
                case 2:
                    UImg.c2Image.color = Color.yellow;
                    break;
                case 3:
                    UImg.c2Image.color = Color.cyan;
                    break;
            }

            switch(stateMg.playerState.choices[2])
            {
                case 1:
                    UImg.c3Image.color = Color.magenta;
                    break;
                case 2:
                    UImg.c3Image.color = Color.yellow;
                    break;
                case 3:
                    UImg.c3Image.color = Color.cyan;
                    break;
            }
        }

        //set time
        if(stateMg.currentState == stateMg.bonusState)
            UImg.timerBar.maxValue = stateMg.setTurnTime * 2;
        else
            UImg.timerBar.maxValue = stateMg.setTurnTime;

        //enable/disable choice ui on input
        if(stateMg.buttonPressed)
        {
            UImg.pickNotes.SetActive(false);
        }
        else
        {
            UImg.pickNotes.SetActive(true);
        }

        //timer color
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
        if(stateMg.currentState == stateMg.startState)
        {
            //pickchoice
            UImg.startUI.SetActive(true);
            UImg.choiceUI.SetActive(false);
            UImg.chanceUI.SetActive(false);
            UImg.bonusUI.SetActive(false);
        }

        if(stateMg.currentState == stateMg.playerState)
        {
            //pickchoice
            UImg.startUI.SetActive(false);
            UImg.choiceUI.SetActive(true);
            UImg.chanceUI.SetActive(false);
            UImg.bonusUI.SetActive(false);
        }

        if(stateMg.currentState == stateMg.chanceState)
        {
            //chance
            UImg.startUI.SetActive(false);
            UImg.choiceUI.SetActive(false);
            UImg.chanceUI.SetActive(true);
            UImg.bonusUI.SetActive(false);
        }

        if(stateMg.currentState == stateMg.bonusState)
        {
            //bonus
            UImg.startUI.SetActive(false);
            UImg.choiceUI.SetActive(false);
            UImg.chanceUI.SetActive(false);
            UImg.bonusUI.SetActive(true);
        }
    }
}
