using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ComboController : MonoBehaviour
{
    // Serializable and Public
    public TMP_Text combo;
    // Private
    int comboValue = 0;
    /*int maxCombo*/

    // Static
    // Defined Function
    void UpdateCombo(object data)
    {
        bool isCombo = (bool)data;
        if (isCombo)
        {
            comboValue++;
        }
        else
        {
            comboValue = 0;
        }
        combo.text = string.Format("x{0:D}", comboValue);
    }

    void ComboInit()
    {
        comboValue = 0;
        combo.text = string.Format("x{0:D}", comboValue);
    }

    // System Function
    void Awake()
    {
        EventManager.AddListener(EventManager.EventName.UpdateCombo, UpdateCombo);
        EventManager.AddListener(EventManager.EventName.LevelInit, ComboInit);
        ComboInit();
    }
}
