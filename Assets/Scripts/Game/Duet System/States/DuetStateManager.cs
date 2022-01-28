using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetStateManager : MonoBehaviour
{
    DuetBaseState currentState;
    public DuetPlayerPlayState playerState = new DuetPlayerPlayState();
    public DuetPickChoiceState pickChoiceState = new DuetPickChoiceState();
    public DuetNPCPlayState npcState = new DuetNPCPlayState();
    public DuetBothPlayState overlapState = new DuetBothPlayState();
    public DuetPlayFinishedState finishedState = new DuetPlayFinishedState();
    
    public int[] turnType; //1: player (button) // 2: NPC (wait) // 3: Both (choice)
    public int turnsAmount;
    public int turnIndex;

    public bool buttonPressed;
    public int score;
    public float setTurnTime;
    public float turnTime;


    // Start is called before the first frame update
    void Start()
    {
        turnsAmount = turnType.Length;
        switch(turnType[turnIndex])
        {
            case 1:
                currentState = playerState;     //PRESS TO GO
                currentState.EnterState(this);
                break;
            case 2:
                currentState = npcState;        //WAIT TO FINISH
                currentState.EnterState(this);
                break;
            case 3:
                currentState = pickChoiceState;     //BOTH PLAY
                currentState.EnterState(this);
                break;
            default:
                Debug.LogError("Invalid State Skipping Turn");
                currentState = finishedState;
                currentState.EnterState(this);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(DuetBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
}
