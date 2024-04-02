using System.Collections.Generic;
using UnityEngine;
using static InGameParameters;

public class THold : BaseNote
{
    public float timeEnd;
    public float scaleStart;
    public List<(float time, float speedRate)> headSpeedList = new();
    public List<(float time, float speedRate)> tailSpeedList = new();
    public List<(float timeStart, float startY)> headMoveList = new();
    public List<(float timeStart, float scale)> tailScaleList = new();

    public void InitMoveAndScale(float speed, float timeStart = -3.0f)
    {
        //Required that the speedList is ready.
        if (headSpeedList.Count < 2 || tailSpeedList.Count < 2) Debug.LogError("Invalid speedList before initialize the moveList. Please check the ChartReader.");

        bool isTIConfirmed = false; //is TimeInstantiate Confirmed
        float actualSpeed = (gameHeightSize - judgeLinePos) / GetAdvance(speed);

        //The first step: Init the holdMoveList and timeInstantiate
        float currentY = judgeLinePos;
        headMoveList.Add((timeJudge, currentY));
        for (int i = headSpeedList.Count - 2; i >= 0; i--)
        {
            float temp = currentY;

            currentY += actualSpeed * headSpeedList[i].speedRate * (headSpeedList[i + 1].time - headSpeedList[i].time);

            //The timeInstantiate is the first time the note appears in the screen
            //Surely the note will shuttle from the edge many times, so judge the time every loop.
            if (temp < gameHeightSize && currentY >= gameHeightSize)
            {
                isTIConfirmed = true;
                timeInstantiate = headSpeedList[i + 1].time - (gameHeightSize - temp) / actualSpeed / headSpeedList[i].speedRate;
            }
            else if (temp > -gameHeightSize && currentY <= -gameHeightSize)
            {
                isTIConfirmed = true;
                timeInstantiate = headSpeedList[i + 1].time + (temp - (-gameHeightSize)) / actualSpeed / headSpeedList[i].speedRate;
                //In this case, the speedRate should be minus zero
            }
            headMoveList.Add((headSpeedList[i].time, currentY));
        }
        headMoveList.Reverse();

        if (!isTIConfirmed) //When the song or trail starts, the note is already in the screen
        {
            timeInstantiate = timeStart;
        }

        //The second step: Init the tailScaleList and scaleStart
        float currentScale = 0;
        tailScaleList.Add((timeEnd, currentScale));
        for (int i = tailSpeedList.Count - 2; i >= 0; i--)
        {
            currentScale += actualSpeed * tailSpeedList[i].speedRate * (tailSpeedList[i + 1].time - tailSpeedList[i].time);
            tailScaleList.Add((tailSpeedList[i].time, currentScale));
        }
        tailScaleList.Reverse();

        scaleStart = currentScale;
    }
}
