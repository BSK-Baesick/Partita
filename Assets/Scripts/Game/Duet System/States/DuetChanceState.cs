using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetChanceState : DuetBaseState
{
    /////////////////////////////////////////////////////////////////////////////CHANCE MODE
    public override void EnterState(DuetStateManager duet)
    {
        duet.turnTime = duet.setTurnTime;
    }

    public override void UpdateState(DuetStateManager duet)
    {   
        //Timer Countdown
        duet.turnTime -= Time.deltaTime;

        if(duet.turnTime <= 0)
        {
            duet.SwitchState(duet.finishedState);
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
                    duet.SwitchState(duet.npcState); //BACK TO STORY
                    break;
            }
        }
    }
}
