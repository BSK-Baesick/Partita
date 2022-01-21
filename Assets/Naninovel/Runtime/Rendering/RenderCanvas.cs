// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// Allows specifying a custom render canvas size.
    /// </summary>
    public class RenderCanvas : MonoBehaviour
    {
        public Vector2 Size = Vector2.one;
        
        private void OnDrawGizmos ()
        {
            Gizmos.DrawWireCube(transform.position, Size);
        }
    }
}
