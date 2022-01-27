using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetNPCPlayState : DuetBaseState
{
    public override void EnterState(DuetStateManager duet)
    {
        Debug.Log("NPC's Turn");
        duet.turnTime = duet.setTurnTime;
    }

    public override void UpdateState(DuetStateManager duet)
    {
        duet.turnTime -= Time.deltaTime;

        if(Input.GetKeyDown("space") && !duet.buttonPressed)
        {
            //you interrupted
            duet.score--;
            duet.buttonPressed = true;
        }

        if(duet.turnTime <= 0)
        {
            duet.SwitchState(duet.finishedState);
        }
    }

    public override void GeneratePlayState(DuetStateManager duet)
    {

    }
}
