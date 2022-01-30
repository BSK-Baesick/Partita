using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetPlayerPlayState : DuetBaseState
{
    /////////////////////////////////////////////////////PLAYER SOLO MODE
    private int scoreEarned;
    private int timerInstance;
    public int buttonChosen;

    private int turns;

    private string whoseTurn;
    
    //public enChoiceButtons choiceButtons;

    public override void EnterState(DuetStateManager duet)
    {
        duet.turnTime = duet.setTurnTime;
        turns = 6;
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
        
        //reset variables then check if we should loop back or continue
        if(duet.turnTime <= 0)
        {
            duet.turnTime = duet.setTurnTime;
            duet.buttonPressed = false;
            scoreEarned = 0;
            buttonChosen = 0;

            if(turns > 0)
            {
                turns--;
            }

            if(turns <= 0)
            {
                switch(duet.score)
                {
                    case int n when n >= 90:
                        //back to story
                        break;
                    case int n when n >= 30:
                        duet.SwitchState(duet.chanceState);
                        break;
                    case int n when n < 30:
                        //back to story dont get character
                        break;
                }
            }
        }

    }

    private void CheckInput()
    {
        if(buttonChosen == timerInstance)
        {
            //PLAY GOOD AUDIO
            scoreEarned = buttonChosen * 5;
            buttonChosen = 0;
        }
        else
        {
            //PLAY BAD AUDIO
            scoreEarned = buttonChosen * -3;
            buttonChosen = 0;
        }
    }
}
