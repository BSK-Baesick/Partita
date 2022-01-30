using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetNPCPlayState : DuetBaseState
{
    ///////////////////////////////////////////////////////////////////////NPC SOLO MODE
    private int scoreEarned;
    private int timerInstance;
    
    //public enChoiceButtons choiceButtons;

    public override void EnterState(DuetStateManager duet)
    {
        duet.turnTime = duet.setTurnTime;
    }

    public override void UpdateState(DuetStateManager duet)
    {
        duet.turnTime -= Time.deltaTime;
        timerInstance = Mathf.RoundToInt(duet.turnTime);

        //choose button to play
        if(Input.GetKeyDown("a") && !duet.buttonPressed) //red
        {
            //END DUET
        }
        if(Input.GetKeyDown("s") && !duet.buttonPressed) //yellow
        {
            //END DUET
        }
        if(Input.GetKeyDown("d") && !duet.buttonPressed) //green
        {
            //END DUET
        }
        if(Input.GetKeyDown("space") && !duet.buttonPressed)
        {
            //END DUET
        }
        
        if(duet.turnTime <= 0)
        {
            scoreEarned = 0;
            duet.SwitchState(duet.finishedState);
        }

    }
}
