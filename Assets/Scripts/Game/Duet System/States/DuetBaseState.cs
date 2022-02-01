using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DuetBaseState
{
    public abstract void EnterState(DuetStateManager duet);

    public abstract void UpdateState(DuetStateManager duet);
}
