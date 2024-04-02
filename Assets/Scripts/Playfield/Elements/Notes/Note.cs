using System.Collections.Generic;
using UnityEngine;
using static InGameParameters;

public class Note : BaseNote
{
    public List<(float time, float speedRate)> speedList = new();
    public List<(float timeStart, float startY)> moveList = new();

    public void InitMoveList(float speed, float timeStart = timePreAnimation)
    {
        //Required that the speedList is ready.
        if (speedList.Count < 2) Debug.LogError("Invalid speedList before initialize the moveList. Please check the ChartReader.");

        bool isTIConfirmed = false; //is TimeInstantiate Confirmed
        float actualSpeed = (gameHeightSize - judgeLinePos) / GetAdvance(speed);
        float currentY = judgeLinePos;
        moveList.Add((timeJudge, currentY));
        for (int i = speedList.Count - 2; i >= 0; i--)
        {
            float temp = currentY;

            currentY += actualSpeed * speedList[i].speedRate * (speedList[i + 1].time - speedList[i].time);

            //The timeInstantiate is the first time the note appears in the screen
            //Surely the note will shuttle from the edge many times, so judge the time every loop.
            if (temp < gameHeightSize && currentY >= gameHeightSize)
            {
                isTIConfirmed = true;
                timeInstantiate = speedList[i + 1].time - (gameHeightSize - temp) / actualSpeed / speedList[i].speedRate;
            }
            else if (temp > -gameHeightSize && currentY <= -gameHeightSize)
            {
                isTIConfirmed = true;
                timeInstantiate = speedList[i + 1].time + (temp - (-gameHeightSize)) / actualSpeed / speedList[i].speedRate;
                //In this case, the speedRate should be minus zero
            }
            moveList.Add((speedList[i].time, currentY));
        }
        moveList.Reverse();

        if (!isTIConfirmed) //When the song or trail starts, the note is already in the screen
        {
            timeInstantiate = timeStart;
        }
        //TODO: Fix the magic number
    }
}
