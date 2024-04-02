using System;
using System.Collections.Generic;
using UnityEngine;
using static InGameParameters;

public class NoteController : BaseNoteController
{
    //Serializable and Public

    //Private
    bool isJudged = false;
    float current;
    int moveIndex = 0;
    List<(float timeStart, float startY)> moveList;

    //Static

    //Defined Function
    public override void InfoInit(BaseNote baseNote)
    {
        if (baseNote is Note note)
        {
            noteType = note.noteType;
            noteNumber = note.number;
            timeJudge = note.timeJudge;
            moveList = note.moveList;
            moveIndex = 0;
        }
        else Debug.LogError("Parameter should be Note.");
    }

    public override void SpriteInit()
    {
        sprite.localScale = new Vector2(m_camera.G2WPosX(0.95f), m_camera.G2WPosY(0.8f));
    }

    public override bool HandleInput(float timeInput, float scorePerfect)
    {
        //The return value indicates if the input is valid
        foreach (JudgeRange jr in rangeList)
        {
            if (timeJudge - timeInput >= jr.start
             && timeJudge - timeInput < jr.end)
            {
                StatInvoke(jr.isCombo, scorePerfect * jr.scoreRate);
                Debug.Log("note no." + noteNumber + " judged.");
                isJudged = true;
                sprite.gameObject.SetActive(false);
                hitEffect.gameObject.SetActive(true);
                return true;
            }
        }
        return false;
    }

    public override void UpdatePos()
    {
        //Update y value
        while (moveIndex < moveList.Count - 2 && current > moveList[moveIndex + 1].timeStart)
        {
            moveIndex++;
        }
        departPos = new(0, m_camera.G2WPosY(moveList[moveIndex].startY));
        destPos = new(0, m_camera.G2WPosY(moveList[moveIndex + 1].startY));
        float t = (current - moveList[moveIndex].timeStart) / (moveList[moveIndex + 1].timeStart - moveList[moveIndex].timeStart);
        transform.localPosition = Vector2.LerpUnclamped(departPos, destPos, t);
    }

    //System Function
    void Start()
    {
        timeProvider = GameObject.Find("/TimeProvider").GetComponent<TimeProvider>();
        m_camera = GameObject.Find("/Main Camera").GetComponent<PositionConverter>();
        hitEffect = transform.Find("HitEffect");
        SpriteInit();

        while (moveIndex < moveList.Count - 2 && moveList[moveIndex + 1].startY > gameHeightSize)
        {
            moveIndex++;
        }
    }

    void Update()
    {
        if (!isJudged)
        {
            current = timeProvider.ChartTime;
            UpdatePos();
        }

        if (timeJudge - current < timeUNoteMiss)
        {
            if (!isJudged) StatInvoke(false, 0);
            Destroy(gameObject);
            return;
        }

    }
}
