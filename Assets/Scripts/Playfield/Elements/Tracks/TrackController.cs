using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class TrackController : MonoBehaviour
{
    // Serializable and Public
    public TimeProvider timeProvider;
    public PositionConverter m_camera;
    public int trackNumber;
    [SerializeField] float timeStart;
    [SerializeField] float timeEnd;
    [SerializeField] List<(float time, float pos, char shape)> lPosList;
    [SerializeField] List<(float time, float pos, char shape)> rPosList;

    // Private
    float current;
    float width;
    float x;
    char color = 'b';
    bool isVisible;
    bool isPreAnimate;
    bool isPostAnimate;
    int lPosIndex = 0;
    int rPosIndex = 0;
    Vector2 pos;
    Transform sprite;

    // Static
    readonly float timeAfterEnd = 0.500f; //Track will be destroyed a bit later to avoid advanced note destruction

    // Defined Function
    public void InfoInit(Track track)
    {
        trackNumber = track.number;
        timeStart = track.timeStart;
        timeEnd = track.timeEnd;
        lPosList = track.lPosList;
        rPosList = track.rPosList;
        color = track.color;
        isVisible = track.isVisible;
        isPreAnimate = track.isPreAnimate;
        isPostAnimate = track.isPostAnimate;

        pos = new(0, 0);
        lPosIndex = 0;
        rPosIndex = 0;
    }

    void SpriteInit()
    {
        sprite = transform.Find("Sprite");
        sprite.localScale = new Vector2(m_camera.G2WPosX(1f), m_camera.G2WPosY(10f));
        sprite.GetComponent<SpriteRenderer>().color = color switch
        {
            'r' => new Color32(234, 103, 228, 150),
            'b' => new Color32(0, 226, 240, 150),
            'd' => new Color32(130, 0, 10, 200),
            _ => Color.clear,
        };

        if (!isVisible)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }

    float CalcPos(float t1, float p1, char shape, float t2, float p2)
    {
        float t = (current - t1) / (t2 - t1);
        return shape switch
        {
            's' => Mathf.LerpUnclamped(p1, p2, t),
            'i' => (float)((p2 - p1) * Math.Sin(Math.PI / 2 * t) + p1),
            'o' => (float)((p2 - p1) * (1 - Math.Sin(Math.PI / 2 * t + Math.PI / 2)) + p1),
            'b' => (float)((p2 - p1) / 2 * (1 - Math.Sin(Math.PI * t + Math.PI / 2)) + p1),
            _ => 0,
        };
    }

    void UpdatePosAndWidth()
    {
        //The last of xPos is only used to compare and should not be entered
        while (lPosIndex < lPosList.Count - 2 && current > lPosList[lPosIndex + 1].time)
        {
            lPosIndex++;
        }
        while (rPosIndex < rPosList.Count - 2 && current > rPosList[rPosIndex + 1].time)
        {
            rPosIndex++;
        }
        var (lt1, lp1, lshape) = lPosList[lPosIndex];
        var (lt2, lp2, _) = lPosList[lPosIndex + 1];
        var (rt1, rp1, rshape) = rPosList[rPosIndex];
        var (rt2, rp2, _) = rPosList[rPosIndex + 1];

        var lx = CalcPos(lt1, lp1, lshape, lt2, lp2);
        var rx = CalcPos(rt1, rp1, rshape, rt2, rp2);
        x = (lx + rx) / 2;
        width = Math.Abs(lx - rx);

        pos.x = m_camera.G2WPosX(x);
        transform.position = pos;
        transform.localScale = new Vector2(width, 1);
    }

    // System Function
    void Start()
    {
        timeProvider = GameObject.Find("/TimeProvider").GetComponent<TimeProvider>();
        m_camera = GameObject.Find("/Main Camera").GetComponent<PositionConverter>();

        SpriteInit();

        pos.x = m_camera.G2WPosX((lPosList[0].pos + rPosList[0].pos) / 2);
        transform.position = pos;
        transform.localScale = new Vector2(0, 1);

        //TODO: Other
    }

    void Update()
    {
        current = timeProvider.ChartTime;
        if (current < timeEnd)
        {
            UpdatePosAndWidth();
        }
        if (current > timeEnd || current < timeStart)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.clear;
        }
        if (current > timeEnd + timeAfterEnd)
        {
            Destroy(gameObject);
        }
    }
}
