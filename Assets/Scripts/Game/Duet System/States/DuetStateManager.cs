using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetStateManager : MonoBehaviour
{
    DuetBaseState currentState;
    public DuetPlayerPlayState playerState = new DuetPlayerPlayState();
    public DuetNPCPlayState npcState = new DuetNPCPlayState();
    public DuetBothPlayState overlapState = new DuetBothPlayState();
    public DuetPlayFinishedState finishedState = new DuetPlayFinishedState();
    
    public bool isPlayersTurn;
    public int turnsAmount;
    public int turnIndex;

    public bool buttonPressed;
    public int score;
    public float setTurnTime;
    public float turnTime;


    // Start is called before the first frame update
    void Start()
    {
        //maybe change to switch???
        if(isPlayersTurn)
        {
            currentState = playerState;
            currentState.EnterState(this);
        }
        else
        {
            currentState = npcState;
            currentState.EnterState(this);
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
