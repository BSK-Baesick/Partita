using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetStartState: DuetBaseState
{
    public override void EnterState(DuetStateManager duet)
    {
        duet.score = 0;
        duet.buttonPressed = false;
    }

    public override void UpdateState(DuetStateManager duet)
    {
        if(Input.GetKeyDown("space"))
        {
            duet.SwitchState(duet.playerState);
        }
    }
}
