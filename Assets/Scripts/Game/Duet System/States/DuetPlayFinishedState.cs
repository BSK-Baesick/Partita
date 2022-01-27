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
        duet.turnIndex++;
    }

    public override void UpdateState(DuetStateManager duet)
    {
        if(duet.turnIndex < duet.turnsAmount)
        {
            if(duet.isPlayersTurn)
                duet.SwitchState(duet.playerState);
            else
                duet.SwitchState(duet.npcState);
        }
    }

    public override void GeneratePlayState(DuetStateManager duet)
    {

    }
}
