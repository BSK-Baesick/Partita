using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetPlayerPlayState : DuetBaseState
{
    public override void EnterState(DuetStateManager duet)
    {
        Debug.Log("Player's Turn");
        duet.turnTime = duet.setTurnTime;
    }

    public override void UpdateState(DuetStateManager duet)
    {   
        //Timer Countdown
        duet.turnTime -= Time.deltaTime;

        if(duet.turnTime <= 0)
        {
            //buttonMissed
            /*if(duet.buttonPressed == false)
                duet.score--;*/ //commented out because we're not implementing score subtraction anymore
            duet.SwitchState(duet.finishedState);
        }

        if(Input.GetKeyDown("space") && duet.buttonPressed == false)
        {
            //perfect
            duet.buttonPressed = true;
            duet.score += 5;
        }
    }

    public override void GeneratePlayState(DuetStateManager duet)
    {

    }
}
