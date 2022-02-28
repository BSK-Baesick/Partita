using UnityEngine.Events;
using DuetSystem.Datas;

namespace DuetSystem.Utilities
{
    public class DuetGameEvents
    {
        [System.Serializable] public class DuetGameStateEvent : UnityEvent<DuetGameState, DuetGameState> { }
    }
}