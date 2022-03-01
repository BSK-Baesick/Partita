using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Async;
using Naninovel;
using Ink.Runtime;

public class DuetController : MonoBehaviour
{
    public int score;
    public float scoreFloat;
    public Slider scoreBar;

    public float timer;

    public int circleState;
    public float circleSpeed;

    public GameObject miss1;
    public GameObject miss2;
    public GameObject miss3;
    public GameObject miss4;

    public Transform pos1;
    public Transform pos2;
    public Transform pos3;
    public Transform pos4;

    private int characterId;

    private FmodAudioManager fmodAudioManager;

    void Start()
    {
        fmodAudioManager = Engine.GetService<FmodAudioManager>();
        fmodAudioManager.StartMusicDuet();
        score = 0;
    }

    void Update()
    {
        //set the nani variable
        var variableManager = Engine.GetService<ICustomVariableManager>();
        variableManager.TrySetVariableValue("duetScore", score);

        timer -= Time.deltaTime;
        score = Mathf.RoundToInt(scoreFloat);
        scoreBar.value = scoreFloat;

        if(timer <= 0)
        {
            timer = 0;
            var switchCommand = new SwitchToNovel();
            switchCommand.ExecuteAsync().Forget();
        }

        //move circle
        if(Input.GetKeyDown("s"))
            circleState++;
        if(Input.GetKeyDown("w"))
            circleState--;

        if(circleState >= 4)
            circleState = 4;
        if(circleState <= 1)
            circleState = 1;

        
        switch(circleState)
        {
            case 1:
                transform.position = Vector3.Lerp(transform.position, pos1.position, circleSpeed);
                miss1.SetActive(false);
                miss2.SetActive(true);
                miss3.SetActive(true);
                miss4.SetActive(true);
                break;
            case 2:
                transform.position = Vector3.Lerp(transform.position, pos2.position, circleSpeed);
                miss1.SetActive(true);
                miss2.SetActive(false);
                miss3.SetActive(true);
                miss4.SetActive(true);
                break;
            case 3:
                transform.position = Vector3.Lerp(transform.position, pos3.position, circleSpeed);
                miss1.SetActive(true);
                miss2.SetActive(true);
                miss3.SetActive(false);
                miss4.SetActive(true);
                break;
            case 4:
                transform.position = Vector3.Lerp(transform.position, pos4.position, circleSpeed);
                miss1.SetActive(true);
                miss2.SetActive(true);
                miss3.SetActive(true);
                miss4.SetActive(false);
                break;
        }

        if(scoreFloat <= 0)
            scoreFloat = 0;
        if(scoreFloat >= 100)
            scoreFloat = 100;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "PlayNote") //good audio
        {
            fmodAudioManager.ChangeDuetPlaybackStatus(1);
            fmodAudioManager.ChangeTrack(6);
            scoreFloat += Time.deltaTime * 0.8f;
        }

        if(col.tag == "SilenceNote") //bad audio
        {
            fmodAudioManager.ChangeDuetPlaybackStatus(0);
            scoreFloat -= Time.deltaTime * 5;
        }

        if(col.tag == "BonusNote"  && Input.GetKey("space")) //bonus
        {
            fmodAudioManager.ChangeDuetPlaybackStatus(1);
            var variableManager = Engine.GetService<ICustomVariableManager>();

            switch (variableManager.GetVariableValue("currentCharacterPlaying"))
            {
                case "MILLIA":
                    characterId = 1;
                    fmodAudioManager.StartMinigameMillia();
                    break;

                case "STEPHAN":
                    characterId = 2;
                    fmodAudioManager.StartMinigameStephan();
                    break;

                case "VERA":
                    characterId = 3;
                    fmodAudioManager.StartMinigameVera();
                    break;

                case "PASCHA":
                    characterId = 4;
                    fmodAudioManager.StartMinigamePascha();
                    break;

                case "ZURAB":
                    characterId = 5;
                    fmodAudioManager.StartMinigameZurab();
                    break;
            }

            fmodAudioManager.ChangeTrack(characterId);
            scoreFloat += Time.deltaTime * 2;
        }
    }
}
