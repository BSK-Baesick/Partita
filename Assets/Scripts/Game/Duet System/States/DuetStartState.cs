using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Naninovel;

public class DuetStartState: DuetBaseState
{

    private int characterId;

    public override void EnterState(DuetStateManager duet)
    {
        var audioManager = Engine.GetService<FmodAudioManager>();
        var variableManager = Engine.GetService<ICustomVariableManager>();

        switch (variableManager.GetVariableValue("currentCharacterPlaying"))
        {
            case "MILLIA":
                characterId = 1;
                break;

            case "STEPHAN":
                characterId = 2;
                break;

            case "VERA":
                characterId = 3;
                break;

            case "PASCHA":
                characterId = 4;
                break;

            case "ZURAB":
                characterId = 5;
                break;
        }

        audioManager.StartMusicDuet(characterId);

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
