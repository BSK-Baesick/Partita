using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;
using Naninovel;
using Naninovel.Commands;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    [SerializeField] private GameObject walkSprite;
    [SerializeField]private GameObject idleSprite;

    public StringParameter ScriptName;
    public StringParameter Label;

    void Update()
    {
        var m_Input = Input.GetAxis("Horizontal");
        transform.position += new Vector3(m_Input, 0, 0) * Time.deltaTime * m_Speed;

        switch(m_Input)
        {
            case 0:
                walkSprite.SetActive(false);
                idleSprite.SetActive(true);
                break;
            default:
                walkSprite.SetActive(true);
                idleSprite.SetActive(false);
                break;
        }


        if(!Mathf.Approximately(0, m_Input))
            transform.rotation = m_Input > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(Input.GetKeyDown("space"))
        {
            Debug.Log("talking");
            var switchCommand = new SwitchToNovelMode { ScriptName = "Start Ink" };
	        switchCommand.ExecuteAsync().Forget();
        }
    }
}
