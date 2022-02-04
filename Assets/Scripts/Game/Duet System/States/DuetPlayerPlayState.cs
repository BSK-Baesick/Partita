using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using Naninovel;
using Naninovel.Commands;

public class DuetPlayerPlayState : DuetBaseState
{
    public List<int> choices = new List<int>();

    private int scoreEarned;
    private int timerInstance;
    public int buttonChosen;

    public int turns;

    private string whoseTurn;
    
    //public enChoiceButtons choiceButtons;

    public override void EnterState(DuetStateManager duet)
    {
        duet.turnTime = duet.setTurnTime;
        turns = Random.Range(4,7);

        while(choices.Count < 3)
        {
            NewNumber(4);
        }
    }

    public override void UpdateState(DuetStateManager duet)
    {
        duet.turnTime -= Time.deltaTime;
        timerInstance = Mathf.RoundToInt(duet.turnTime);

        //choose button to play
        if(Input.GetKeyDown("a") && !duet.buttonPressed) //red
        {
            buttonChosen = choices[0];
            Debug.Log(buttonChosen + " " + choices[0] + " " + timerInstance);
            duet.buttonPressed = true;
            CheckInput();
            duet.score += scoreEarned;
        }
        if(Input.GetKeyDown("s") && !duet.buttonPressed) //yellow
        {
            buttonChosen = choices[1];
            Debug.Log(buttonChosen + " " + choices[1] + " " + timerInstance);
            duet.buttonPressed = true;
            CheckInput();
            duet.score += scoreEarned;
        }
        if(Input.GetKeyDown("d") && !duet.buttonPressed) //green
        {
            buttonChosen = choices[2];
            Debug.Log(buttonChosen + " " + choices[2] + " " + timerInstance);
            duet.buttonPressed = true;
            CheckInput();
            duet.score += scoreEarned;
        }
        
        //reset variables then check if we should loop back or continue
        if(duet.turnTime <= 0)
        {
            choices.Clear();
            duet.turnTime = duet.setTurnTime;
            duet.buttonPressed = false;
            scoreEarned = 0;
            buttonChosen = 0;

            if(turns > 0)
            {
                turns--;

                while(choices.Count < 3)
                {
                    NewNumber(4);
                }
            }

            if(turns <= 0)
            {
                var switchCommand = new SwitchToNovel();
                switch(duet.score)
                {
                    case int n when n >= 90:
                        switchCommand.ExecuteAsync().Forget();
                        duet.SwitchState(duet.startState);
                        break;
                    case int n when n >= 20:
                        duet.SwitchState(duet.chanceState);
                        break;
                    case int n when n < 20:
                        switchCommand.ExecuteAsync().Forget();
                        duet.SwitchState(duet.startState);
                        break;
                }
            }
        }

    }

    //shuffle choices
    public int NewNumber(int r) 
    {
        int a = 0;

        while(a==0)
        {    
            a = Random.Range (1, r);

            if(!choices.Contains(a))
            {
                choices.Add(a);
            }
            else
            {
                a=0;
            }
        }
        return a;
    }

    private void CheckInput()
    {
        if(buttonChosen == timerInstance)
        {
            //PLAY GOOD AUDIO
            scoreEarned = buttonChosen * 3;
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
