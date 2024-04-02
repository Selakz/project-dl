using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InGameParameters;

public abstract class BaseNoteController : MonoBehaviour
{
    //Judge Range
    [System.Serializable]
    public class JudgeRange
    {
        public JudgeType judgeType;
        public float start; //Includes
        public float end;   //Not include
        public float scoreRate;
        public bool isCombo;
    }
    public List<JudgeRange> rangeList;

    // Serializable and Public
    public TimeProvider timeProvider;
    public PositionConverter m_camera;
    [SerializeField] protected int noteNumber;
    [SerializeField] protected NoteType noteType;
    [SerializeField] protected float timeJudge = 0f;
    [SerializeField] protected Transform sprite;
    [SerializeField] protected Transform hitEffect;

    // Private
    protected Vector2 departPos;
    protected Vector2 destPos;

    // Static

    // Defined Function
    public abstract void InfoInit(BaseNote note);
    public abstract void SpriteInit();
    public abstract void UpdatePos();
    public abstract bool HandleInput(float timeInput, float scorePerfect);

    public void StatInvoke(bool isCombo, float score)
    {
        EventManager.Trigger(EventManager.EventName.UpdateCombo, isCombo);
        EventManager.Trigger(EventManager.EventName.UpdateScore, score);
    }

    // System Function
}
