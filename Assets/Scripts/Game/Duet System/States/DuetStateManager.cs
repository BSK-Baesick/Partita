using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuetStateManager : MonoBehaviour
{
    DuetBaseState currentState;
    public DuetPlayerPlayState playerState = new DuetPlayerPlayState();
    public DuetPickChoiceState pickChoiceState = new DuetPickChoiceState();
    public DuetBonusState bonusState = new DuetBonusState();
    public DuetChanceState chanceState = new DuetChanceState();
    public DuetNPCPlayState npcState = new DuetNPCPlayState();
    public DuetBothPlayState overlapState = new DuetBothPlayState();
    public DuetPlayFinishedState finishedState = new DuetPlayFinishedState();
    
    public int turnType; //1: player // 2: NPC (wait) // 3: Both // 4:chance // 5: Bonus //6: Game Done

    public bool buttonPressed;
    public int score;
    public float setTurnTime;
    public float turnTime;


    // Start is called before the first frame update
    void Start()
    {
        currentState = playerState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(score >= 100)
            score = 100;

        if(score <= 0)
            score = 0;

        currentState.UpdateState(this);
    }

    public void SwitchState(DuetBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
}
