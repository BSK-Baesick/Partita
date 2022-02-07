using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissCheck : MonoBehaviour
{
    [SerializeField]
    public DuetController duet;

    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "PlayNote")
        {
            duet.scoreFloat -= Time.deltaTime * 0.8f;
        }
    }
}
