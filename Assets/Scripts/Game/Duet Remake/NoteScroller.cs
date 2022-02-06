using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteScroller : MonoBehaviour
{
    public float tempo;
    public bool started;

    // Start is called before the first frame update
    void Start()
    {
        tempo = tempo / 60f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position -= new Vector3(tempo * Time.deltaTime, 0f, 0f);
    }
}
