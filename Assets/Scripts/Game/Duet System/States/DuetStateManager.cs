using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using Naninovel;
using Naninovel.Commands;

public class DuetStateManager : MonoBehaviour
{
    public DuetBaseState currentState;
    public DuetStartState startState = new DuetStartState();
    public DuetPlayerPlayState playerState = new DuetPlayerPlayState();
    public DuetBonusState bonusState = new DuetBonusState();
    public DuetChanceState chanceState = new DuetChanceState();
    public DuetPlayFinishedState finishedState = new DuetPlayFinishedState();
    
    public int turnType; //1: pick // 2: chance // 3: bonus


    public bool buttonPressed;
    public int score;
    public float setTurnTime;
    public float turnTime;


    // Start is called before the first frame update
    void Start()
    {
        currentState = startState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(score >= 100)
            score = 100;

        if(score <= 0)
            score = 0;

        var variableManager = Engine.GetService<ICustomVariableManager>();
        variableManager.TrySetVariableValue("duetScore", score);

        currentState.UpdateState(this);
    }

    public void SwitchState(DuetBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
}
