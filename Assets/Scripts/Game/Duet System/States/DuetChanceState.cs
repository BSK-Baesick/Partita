using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using Naninovel;
using Naninovel.Commands;

public class DuetChanceState : DuetBaseState
{
    /////////////////////////////////////////////////////////////////////////////CHANCE MODE
    public override void EnterState(DuetStateManager duet)
    {
        duet.turnTime = duet.setTurnTime;
    }

    public override void UpdateState(DuetStateManager duet)
    {   
        var switchCommand = new SwitchToNovel();
        //Timer Countdown
        duet.turnTime -= Time.deltaTime;

        if(duet.turnTime <= 0)
        {
            switchCommand.ExecuteAsync().Forget();
        }
        else
        {
            //END
        }

        if(Input.GetKeyDown("space") && duet.buttonPressed == false)
        {
            switch(Mathf.RoundToInt(duet.turnTime))
            {
                case 3:
                    duet.SwitchState(duet.bonusState); //BUTTON MASH
                    break;
                case 2:
                    duet.SwitchState(duet.playerState); //PICK MODE
                    break;
                case 1:
                    duet.SwitchState(duet.startState);
                    switchCommand.ExecuteAsync().Forget(); //BACK TO STORY
                    break;
            }
        }
    }
}
