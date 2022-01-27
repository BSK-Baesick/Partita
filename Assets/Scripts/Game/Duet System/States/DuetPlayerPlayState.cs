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
        duet.turnTime -= Time.deltaTime;

        if(duet.turnTime <= 0)
        {
            if(duet.buttonPressed == false)
                duet.score--;
            
            duet.turnTime = 0;
            duet.SwitchState(duet.finishedState);
        }

        if(Input.GetKeyDown("space") && duet.buttonPressed == false)
        {
            //perfect
            duet.buttonPressed = true;
            duet.score++;
        }
    }

    public override void GeneratePlayState(DuetStateManager duet)
    {

    }
}
