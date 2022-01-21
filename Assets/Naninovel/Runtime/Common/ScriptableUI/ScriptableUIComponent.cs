// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine.EventSystems;

namespace Naninovel
{
    public abstract class ScriptableUIComponent<T> : ScriptableUIBehaviour where T : UIBehaviour
    {
        public virtual T UIComponent => uiComponent ? uiComponent : (uiComponent = GetComponent<T>());

        private T uiComponent;
    }
}
