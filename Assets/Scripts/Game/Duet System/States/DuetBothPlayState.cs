using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetBothPlayState : DuetBaseState
{
    public override void EnterState(DuetStateManager duet)
    {
        Debug.Log("Player's Turn");
    }

    public override void UpdateState(DuetStateManager duet)
    {

    }
}