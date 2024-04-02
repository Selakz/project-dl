using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track
{
    public int number;
    public float timeStart;
    public float timeEnd;
    public List<(float time, float pos, char shape)> lPosList = new();
    public List<(float time, float pos, char shape)> rPosList = new();
    public char color;
    public bool isVisible;
    public bool isPreAnimate;
    public bool isPostAnimate;
}
