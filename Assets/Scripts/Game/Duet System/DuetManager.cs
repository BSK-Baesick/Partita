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
        UImg.timerBar.maxValue = stateMg.setTurnTime;

        if(stateMg.buttonPressed)
        {
            UImg.buttonImage.enabled = false;
        }
        else
        {
            UImg.buttonImage.enabled = true;
        }

        if(stateMg.isPlayersTurn)
        {
            UImg.buttonImage.color = Color.cyan;
        }
        else
        {
            UImg.buttonImage.color = Color.magenta;
        }
    }
}
