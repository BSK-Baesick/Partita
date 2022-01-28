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
            switch(duet.turnType[duet.turnIndex])
            {
                case 1:
                    duet.SwitchState(duet.playerState);
                    break;
                case 2:
                    duet.SwitchState(duet.npcState);
                    break;
                case 3:
                    duet.SwitchState(duet.pickChoiceState);
                    break;
                default:
                    Debug.LogError("Invalid State Skipping Turn");
                    duet.SwitchState(duet.finishedState);
                    break;
            }
        }
    }

    public override void GeneratePlayState(DuetStateManager duet)
    {

    }
}
