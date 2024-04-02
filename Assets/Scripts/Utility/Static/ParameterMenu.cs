using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "InGameParameters", menuName = "Parameters/InGame", order = 0)]
public class InGameParameters : ScriptableObject
{
    //F = Fast; L = Late; E = Exact
    public enum JudgeType { UPerfect, UMiss, TPerfectE, TPerfectF, TPerfectL, TGreatF, TGreatL, TBad, TMiss };
    public enum NoteType { UNote, TNote, BTNote, RTNote, DTNote, Any };

    public const float maxScore = 1010000f, aveScore = 950000f, tNoteScore = 60000f;

    public const float gameWidthSize = 3.0f;
    public const float gameHeightSize = 5.0f;
    public const float judgeLinePos = -4.0f;

    public const float timePreAnimation = 3.000f;
    public const float timeUNoteMiss = -0.300f;
    public const float timeHoldInterval = 0.250f;

    public static float GetAdvance(float speed)
    {
        return (float)(2.5 * Math.Sin((0.85 + 0.056 * speed) * Math.PI) + 2.5);
    }
}

