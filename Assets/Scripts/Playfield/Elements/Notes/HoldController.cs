using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static InGameParameters;

public class HoldController : BaseNoteController
{
    //Serializable and Public

    //Private
    bool isJudged = false; //Used to activate hold activities
    bool isReleased = false;
    JudgeRange judgeRange;
    float current;
    float timeEnd;
    float lastHoldRelease = 0f;
    float scaleStart;
    float score;
    int moveIndex = 0;
    int scaleIndex = 0;
    List<(float timeStart, float startY)> moveList;
    List<(float timeStart, float scale)> scaleList;

    Vector2 departScale;
    Vector2 destScale;

    float scaleEarly;
    float timeEarly;

    //Static

    //Defined Function
    public override void InfoInit(BaseNote baseNote)
    {
        if (baseNote is THold thold)
        {
            noteType = thold.noteType;
            noteNumber = thold.number;
            timeJudge = thold.timeJudge;
            timeEnd = thold.timeEnd;
            scaleStart = thold.scaleStart;
            moveList = thold.headMoveList;
            scaleList = thold.tailScaleList;
            moveIndex = 0;
            scaleIndex = 0;
        }
        else Debug.LogError("Parameter should be THold.");
    }

    public override void SpriteInit()
    {
        sprite = transform.Find("Sprite");
        sprite.localScale = new Vector2(m_camera.G2WPosX(0.95f), m_camera.G2WPosY(1.0f));
        sprite.localPosition = new Vector2(0, m_camera.G2WPosY(0.5f));
        transform.localScale = new Vector2(1, scaleStart);
    }

    public override bool HandleInput(float timeInput, float scorePerfect)
    {
        //The return value indicates if the input is valid
        foreach (JudgeRange jr in rangeList)
        {
            if (timeJudge - timeInput >= jr.start
             && timeJudge - timeInput < jr.end)
            {
                Debug.Log("note no." + noteNumber + " judged.");
                isJudged = true;
                judgeRange = jr;
                score = scorePerfect;

                if (current < timeJudge)
                {
                    timeEarly = current;
                    scaleEarly = scaleStart + m_camera.W2GPosY(transform.localPosition.y) - judgeLinePos;
                }

                return true;
            }
        }
        return false;
    }

    public override void UpdatePos()
    {
        //Update position or scale
        if (current < timeJudge && !isJudged)
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

        else if (current < timeJudge && isJudged)
        {
            transform.localPosition = new Vector2(0, m_camera.G2WPosY(judgeLinePos));
            departScale = new(1, scaleEarly);
            destScale = new(1, scaleStart);
            float t = (current - timeEarly) / (timeJudge - timeEarly);
            transform.localScale = Vector2.LerpUnclamped(departScale, destScale, t);
        }

        else if (current >= timeJudge && current < timeEnd)
        {
            transform.localPosition = new Vector2(0, m_camera.G2WPosY(judgeLinePos));
            while (scaleIndex < scaleList.Count - 2 && current > scaleList[scaleIndex + 1].timeStart)
            {
                scaleIndex++;
            }
            departScale = new(1, scaleList[scaleIndex].scale);
            destScale = new(1, scaleList[scaleIndex + 1].scale);
            float t = (current - scaleList[scaleIndex].timeStart) / (scaleList[scaleIndex + 1].timeStart - scaleList[scaleIndex].timeStart);
            transform.localScale = Vector2.LerpUnclamped(departScale, destScale, t);
        }
    }

    public void DetectHold(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    if (isJudged)
                    {
                        isReleased = false;
                        Debug.Log("Continued!");
                        //Activate animation
                    }
                    break;
                case InputActionPhase.Canceled:
                    if (isJudged)
                    {
                        isReleased = true;
                        lastHoldRelease = current;
                        Debug.Log("Released!");
                        //Inactivate animation
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void HandleHold()
    {
        if ((isJudged && isReleased && current > lastHoldRelease + timeHoldInterval && current > timeJudge + timeHoldInterval)
        || (!isJudged && current > timeJudge + rangeList[^1].end))
        {
            isJudged = false;
            StatInvoke(false, 0);
            Debug.Log("hold failed.");
        }
    }

    //System Function
    void Start()
    {
        timeProvider = GameObject.Find("/TimeProvider").GetComponent<TimeProvider>();
        m_camera = GameObject.Find("/Main Camera").GetComponent<PositionConverter>();
        SpriteInit();

        while (moveIndex < moveList.Count - 2 && moveList[moveIndex + 1].startY > gameHeightSize)
        {
            moveIndex++;
        }
    }

    void Update()
    {
        current = timeProvider.ChartTime;

        if (current > timeEnd)
        {
            if (isJudged) StatInvoke(judgeRange.isCombo, score * judgeRange.scoreRate);
            Destroy(gameObject);
            return;
        }

        UpdatePos();
        HandleHold();
    }
}
