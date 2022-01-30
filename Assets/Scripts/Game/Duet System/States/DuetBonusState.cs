using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetBonusState : DuetBaseState
{
    //////////////////////////////////////////////////////////////////// BONUS/BUTTON MASH MODE
    public int currentColor;
    private float colorTime;

    public override void EnterState(DuetStateManager duet)
    {
        duet.turnTime = duet.setTurnTime;
        GenerateColor();
    }

    public override void UpdateState(DuetStateManager duet)
    {
        duet.turnTime -= Time.deltaTime;
        colorTime -= Time.deltaTime;

        if(Input.GetKeyDown("space"))
        {
            switch(currentColor)
            {
                case 1:
                    //mash for points (g)
                    duet.score++;
                    break;
                case 2:
                    //mash for time (y)
                    duet.turnTime+= 0.5f;
                    break;
                case 3:
                    //mash will end (r)
                    duet.turnTime = 0;
                    break;
            }
        }

        if(duet.turnTime >= duet.setTurnTime)
            duet.turnTime = duet.setTurnTime;

        if(colorTime <= 0)
            GenerateColor();

        if(duet.turnTime <= 0)
        {
            duet.SwitchState(duet.playerState); //////////////////////////////////////BACK TO STORY
        }
    }

    public void GenerateColor()
    {
        currentColor = Random.Range(1,4);
        Debug.Log(currentColor);
        colorTime = Random.Range(1,2);
    }
}
