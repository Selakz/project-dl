using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScoreController : MonoBehaviour
{
    // Serializable and Public
    public TMP_Text score;
    // Private
    float scoreValue = 0;
    // Static
    // Defined Function
    void UpdateScore(object data)
    {
        float addScore = (float)data;
        scoreValue += addScore;
        score.text = string.Format("{0:D7}", Mathf.RoundToInt(scoreValue));
    }

    /*
    void UpdateScore_Decline(){}
    */

    void ScoreInit()
    {
        scoreValue = 0;
        score.text = string.Format("{0:D7}", 0);
    }

    // System Function
    void Awake()
    {
        EventManager.AddListener(EventManager.EventName.UpdateScore, UpdateScore);
        EventManager.AddListener(EventManager.EventName.LevelInit, ScoreInit);
        ScoreInit();
    }
}
