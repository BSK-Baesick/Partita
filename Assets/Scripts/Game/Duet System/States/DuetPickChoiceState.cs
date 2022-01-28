using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetPickChoiceState : DuetBaseState
{
    private int scoreEarned;
    private int timerInstance;
    public int buttonChosen;
    //public enChoiceButtons choiceButtons;

    public override void EnterState(DuetStateManager duet)
    {
        duet.turnTime = duet.setTurnTime;
    }

    public override void UpdateState(DuetStateManager duet)
    {
        duet.turnTime -= Time.deltaTime;
        timerInstance = Mathf.RoundToInt(duet.turnTime);

        //choose button to play
        if(Input.GetKeyDown("a") && !duet.buttonPressed) //red
        {
            buttonChosen = 1;
            duet.buttonPressed = true;
            CheckInput();
            duet.score += scoreEarned;
        }
        if(Input.GetKeyDown("s") && !duet.buttonPressed) //yellow
        {
            buttonChosen = 2;
            duet.buttonPressed = true;
            CheckInput();
            duet.score += scoreEarned;
        }
        if(Input.GetKeyDown("d") && !duet.buttonPressed) //green
        {
            buttonChosen = 3;
            duet.buttonPressed = true;
            CheckInput();
            duet.score += scoreEarned;
        }
        
        if(duet.turnTime <= 0)
        {
            scoreEarned = 0;
            buttonChosen = 0;
            duet.SwitchState(duet.finishedState);
        }

    }

    private void CheckInput()
    {
        if(buttonChosen == timerInstance)
        {
            scoreEarned = buttonChosen * 2;
            buttonChosen = 0;
        }
    }

    public override void GeneratePlayState(DuetStateManager duet)
    {

    }
}
