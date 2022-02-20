using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Naninovel;

public class DuetStartState: DuetBaseState
{

    public override void EnterState(DuetStateManager duet)
    {

        duet.score = 50;
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
