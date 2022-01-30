using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetPlayFinishedState : DuetBaseState
{
    public override void EnterState(DuetStateManager duet)
    {
        Debug.Log("Finished");
        duet.turnTime = duet.setTurnTime; //reset the timer
        duet.buttonPressed = false;
    }

    public override void UpdateState(DuetStateManager duet)
    {
        //GO BACK TO STORY
    }
}
