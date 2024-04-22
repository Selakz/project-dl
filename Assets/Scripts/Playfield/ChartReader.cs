using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static InGameParameters;

public class ChartReader : MonoBehaviour
{
    public static ChartReader chartReader;

    static string[] heads = { "offset", "-", "unote", "pos", "speed", "track", "lpos", "rpos", "tnote", "thold" };

    ChartInfo chartInfo = new();
    List<BaseNote> uNoteList = new();
    List<BaseNote> btNoteList = new();
    List<BaseNote> rtNoteList = new();
    List<BaseNote> dtNoteList = new();
    Dictionary<char, List<BaseNote>> noteList = new();
    List<Track> trackList = new();
    Volume volume = new();
    float offset = 0;

    int ptr = 0; //Global variable indicating current chart line.
    int all = 0;

    public static ChartReader Instance
    {
        get
        {
            if (!chartReader)
            {
                GameObject gameObject = new() { name = "ChartReader" };
                chartReader = gameObject.AddComponent<ChartReader>();
            }
            return chartReader;
        }
    }

    public static ChartInfo Read(string levelPath, int difficulty, float speed)
    {
        return Instance.InClassRead(levelPath, difficulty, speed);
    }

    ChartInfo InClassRead(string levelPath, int difficulty, float speed)
    {
        string[] chart = File.ReadAllLines(levelPath + "/" + difficulty + ".dlf");

        noteList.Add('t', uNoteList);
        noteList.Add('b', btNoteList);
        noteList.Add('r', rtNoteList);
        noteList.Add('d', dtNoteList);

        for (ptr = 0; ptr < chart.Length; ptr++)
        {
            var line = chart[ptr].Trim();
            foreach (var head in heads)
            {
                if (line.StartsWith(head))
                {
                    HandleLine(chart, head, line, speed);
                    break;
                }
            }
        }

        uNoteList.Sort(CmpTimeInNote);
        btNoteList.Sort(CmpTimeInNote);
        rtNoteList.Sort(CmpTimeInNote);
        dtNoteList.Sort(CmpTimeInNote);
        trackList.Sort(CmpTimeInTrack);

        chartInfo = new()
        {
            offset = offset,
            volume = volume,
            noteList = noteList,
            trackList = trackList
        };
        return chartInfo;
    }

    void HandleLine(string[] chart, string head, string line, float speed)
    {
        switch (head)
        {
            case "offset":
                CaseOffsetInit(line);
                break;

            case "unote":
                CaseUNoteInit(chart, line, speed);
                break;

            case "track":
                CaseTrackInit(chart, line, speed);
                break;

            default:
                break;
        }
    }

    void CaseOffsetInit(string line)
    {
        string[] rData = GetParaString(line);
        //rData[0] = offset
        offset = int.Parse(rData[0]) / 1000f;
    }

    void CaseUNoteInit(string[] chart, string line, float speed)
    {
        string[] rData = GetParaString(line);
        //rData[0] = timeJudge, [1] = width, [2] = posEnd, [3] = speed(optional)

        float rTimeJudge = int.Parse(rData[0]) / 1000f;
        float width = float.Parse(rData[1]);
        float posEnd = float.Parse(rData[2]);

        Track rTrack = new()
        {
            number = trackList.Count,
            timeEnd = rTimeJudge,
            color = 't',
            isVisible = false,
            isPreAnimate = false,
            isPostAnimate = false,
        };
        trackList.Add(rTrack);

        Note rNote = new()
        {
            noteType = NoteType.UNote,
            number = ++all,
            timeJudge = rTimeJudge,
            belongingTrack = rTrack.number,
        };
        noteList['t'].Add(rNote);
        volume.AddNote(NoteType.UNote);

        rNote.speedList.Add((-timePreAnimation, 1));
        rTrack.lPosList.Add((-timePreAnimation, posEnd - width / 2, 's'));
        rTrack.rPosList.Add((-timePreAnimation, posEnd + width / 2, 's'));

        if (rData.Length == 4)
        {
            //No speed() line. The speed can be done right now.
            float speedRate = float.Parse(rData[3]);
            rNote.speedList[0] = (-timePreAnimation, speedRate);
            rNote.speedList.Add((rTimeJudge, 1));
            rNote.InitMoveList(speed);
        }

        //Read following info if any
        while (ptr + 1 < chart.Length)
        {
            string nextLine = chart[ptr + 1].Trim();
            if (IsNewElement(nextLine)) break;

            if (nextLine.StartsWith("pos"))
            {
                string[] posList = GetParaString(nextLine);
                for (int i = 0; i < posList.Length; i += 3)
                {
                    //Each group is like (time, pos, shape)
                    float t = int.Parse(posList[i]) / 1000f;
                    float p = float.Parse(posList[i + 1]);
                    char shape = char.Parse(posList[i + 2]);
                    if (t > rTimeJudge) continue;

                    rTrack.lPosList.Add((t, p - width / 2, shape));
                    rTrack.rPosList.Add((t, p + width / 2, shape));
                }
                float pos1 = float.Parse(posList[1]);
                rTrack.lPosList[0] = (-timePreAnimation, pos1 - width / 2, 's');
                rTrack.rPosList[0] = (-timePreAnimation, pos1 + width / 2, 's');
            }

            if (nextLine.StartsWith("speed"))
            {
                string[] speedList = GetParaString(nextLine);
                for (int i = 0; i < speedList.Length; i += 2)
                {
                    //Each group is like (time, speedRate)
                    float t = int.Parse(speedList[i]) / 1000f;
                    float s = float.Parse(speedList[i + 1]);
                    if (t > rTimeJudge) continue;

                    rNote.speedList.Add((t, s));
                }
                float sr1 = float.Parse(speedList[1]);
                rNote.speedList[0] = (-timePreAnimation, sr1);
                rNote.speedList.Add((rTimeJudge, 1));
                rNote.InitMoveList(speed);
            }

            if (nextLine.StartsWith("trail"))
            {
                string[] moveList = GetParaString(nextLine);
                rNote.moveList.Add((-timePreAnimation, 10));
                rNote.timeInstantiate = -timePreAnimation;
                for (int i = 0; i < moveList.Length; i += 2)
                {
                    //Each group is like (time, y)
                    float t = int.Parse(moveList[i]) / 1000f;
                    float y = float.Parse(moveList[i + 1]);
                    if (t > rTimeJudge) continue;

                    rNote.moveList.Add((t, y));
                }
                rNote.moveList.Add((rTimeJudge, judgeLinePos));
            }

            ptr++;
        }

        rTrack.timeStart = rNote.timeInstantiate;
        rTrack.lPosList.Add((rTimeJudge, posEnd - width / 2, 's'));
        rTrack.rPosList.Add((rTimeJudge, posEnd + width / 2, 's'));
    }

    void CaseTrackInit(string[] chart, string line, float speed)
    {
        string[] rData = GetParaString(line);
        //rData[0] = timeStart, [1] = timeEnd, [2] = lPosEnd, [3] = rPosEnd, [4] = color,
        //rData[5] = isVisible, [6] = isPreAnimate, [7] = isPostAnimate

        float rTimeStart = int.Parse(rData[0]) / 1000f;
        float rTimeEnd = int.Parse(rData[1]) / 1000f;
        float lPosEnd = float.Parse(rData[2]);
        float rPosEnd = float.Parse(rData[3]);
        char rColor = char.Parse(rData[4]);
        bool visible = bool.Parse(rData[5]);
        bool preAnimate = bool.Parse(rData[6]);
        bool postAnimate = bool.Parse(rData[7]);

        Track rTrack = new()
        {
            number = trackList.Count,
            timeStart = rTimeStart,
            timeEnd = rTimeEnd,
            color = rColor,
            isVisible = visible,
            isPreAnimate = preAnimate,
            isPostAnimate = postAnimate,
        };
        trackList.Add(rTrack);

        rTrack.lPosList.Add((rTimeStart, lPosEnd, 's'));
        rTrack.rPosList.Add((rTimeStart, rPosEnd, 's'));

        //Read following info if any
        while (ptr + 1 < chart.Length)
        {
            string nextLine = chart[ptr + 1].Trim(); //nextLine is relative to current ptr
            if (IsNewElement(nextLine)) break;

            if (nextLine.StartsWith("lpos"))
            {
                string[] posList = GetParaString(nextLine);
                for (int i = 0; i < posList.Length; i += 3)
                {
                    //Each group is like (time, pos, shape)
                    float t = int.Parse(posList[i]) / 1000f;
                    float p = float.Parse(posList[i + 1]);
                    char shape = char.Parse(posList[i + 2]);
                    if (t > rTimeEnd || t < rTimeStart) continue;

                    rTrack.lPosList.Add((t, p, shape));
                }
                float pos1 = float.Parse(posList[1]);
                rTrack.lPosList[0] = (rTimeStart, pos1, 's');
            }

            if (nextLine.StartsWith("rpos"))
            {
                string[] posList = GetParaString(nextLine);
                for (int i = 0; i < posList.Length; i += 3)
                {
                    //Each group is like (time, pos, shape)
                    float t = int.Parse(posList[i]) / 1000f;
                    float p = float.Parse(posList[i + 1]);
                    char shape = char.Parse(posList[i + 2]);
                    if (t > rTimeEnd || t < rTimeStart) continue;

                    rTrack.rPosList.Add((t, p, shape));
                }
                float pos1 = float.Parse(posList[1]);
                rTrack.rPosList[0] = (rTimeStart, pos1, 's');
            }

            if (nextLine.StartsWith("tnote"))
            {
                string[] noteData = GetParaString(nextLine);
                //noteData[0] = timeJudge, [1] = speed(optional)

                float rTimeJudge = int.Parse(noteData[0]) / 1000f;

                NoteType rNoteType = rColor switch
                {
                    'b' => NoteType.BTNote,
                    'r' => NoteType.RTNote,
                    'd' => NoteType.DTNote,
                    _ => NoteType.Any,
                };

                Note rNote = new()
                {
                    noteType = rNoteType,
                    number = ++all,
                    timeJudge = rTimeJudge,
                    belongingTrack = rTrack.number,
                };
                noteList[rColor].Add(rNote);
                volume.AddNote(rNoteType);

                rNote.speedList.Add((rTimeStart, 1));

                if (noteData.Length == 2)
                {
                    //No speed() line. The speed can be done right now.
                    float speedRate = float.Parse(noteData[1]);
                    rNote.speedList[0] = (rTimeStart, speedRate);
                    rNote.speedList.Add((rTimeJudge, 1));
                    rNote.InitMoveList(speed, rTimeStart);
                }
                else
                {
                    ptr++; //There should be a speed or trail line.
                    string noteLine = chart[ptr + 1].Trim();

                    if (noteLine.StartsWith("speed"))
                    {
                        string[] speedList = GetParaString(noteLine);
                        for (int i = 0; i < speedList.Length; i += 2)
                        {
                            //Each group is like (time, speedRate)
                            float t = int.Parse(speedList[i]) / 1000f;
                            float s = float.Parse(speedList[i + 1]);
                            if (t > rTimeJudge || t < rTimeStart) continue;

                            rNote.speedList.Add((t, s));
                        }
                        float sr1 = float.Parse(speedList[1]);
                        rNote.speedList[0] = (rTimeStart, sr1);
                        rNote.speedList.Add((rTimeJudge, 1));
                        rNote.InitMoveList(speed, rTimeStart);
                    }

                    if (noteLine.StartsWith("trail"))
                    {
                        string[] moveList = GetParaString(noteLine);
                        rNote.moveList.Add((rTimeStart, 10));
                        rNote.timeInstantiate = rTimeStart;
                        for (int i = 0; i < moveList.Length; i += 2)
                        {
                            //Each group is like (time, y)
                            float t = int.Parse(moveList[i]) / 1000f;
                            float y = float.Parse(moveList[i + 1]);
                            if (t > rTimeJudge) continue;

                            rNote.moveList.Add((t, y));
                        }
                        rNote.moveList.Add((rTimeJudge, judgeLinePos));
                    }
                }
            }

            if (nextLine.StartsWith("thold"))
            {
                string[] noteData = GetParaString(nextLine);
                //noteData[0] = timeJudge, [1] = timeEnd, [2] = speed(optional)

                float rTimeJudge = int.Parse(noteData[0]) / 1000f;
                float hold_rTimeEnd = int.Parse(noteData[1]) / 1000f;

                NoteType rNoteType = rColor switch
                {
                    'b' => NoteType.BTNote,
                    'r' => NoteType.RTNote,
                    'd' => NoteType.DTNote,
                    _ => NoteType.Any,
                };

                THold rHold = new()
                {
                    noteType = rNoteType,
                    number = ++all,
                    timeJudge = rTimeJudge,
                    belongingTrack = rTrack.number,
                    timeEnd = hold_rTimeEnd,
                };
                noteList[rColor].Add(rHold);
                volume.AddNote(rNoteType);

                rHold.headSpeedList.Add((rTimeStart, 1));
                rHold.tailSpeedList.Add((rTimeJudge, 1));

                if (noteData.Length == 3)
                {
                    //No speed() line. The speed can be done right now.
                    float speedRate = float.Parse(noteData[2]);
                    rHold.headSpeedList[0] = (rTimeStart, speedRate);
                    rHold.headSpeedList.Add((rTimeJudge, 1));
                    rHold.tailSpeedList[0] = (rTimeJudge, speedRate);
                    rHold.tailSpeedList.Add((hold_rTimeEnd, 1));
                    rHold.InitMoveAndScale(speed, rTimeStart);
                }
                else
                {
                    ptr++; //There should be a speed or trail line.
                    string noteLine = chart[ptr + 1].Trim();

                    if (noteLine.StartsWith("speed"))
                    {
                        string[] speedList = GetParaString(noteLine);
                        for (int i = 0; i < speedList.Length; i += 2)
                        {
                            //Each group is like (time, speedRate)
                            float t = int.Parse(speedList[i]) / 1000f;
                            float s = float.Parse(speedList[i + 1]);
                            if (t >= rTimeStart && t < rTimeJudge) rHold.headSpeedList.Add((t, s));
                            if (t >= rTimeJudge && t < rTimeEnd) rHold.tailSpeedList.Add((t, s));
                        }
                        float sr1 = float.Parse(speedList[1]);
                        float sr2 = rHold.headSpeedList[^1].speedRate;
                        rHold.headSpeedList[0] = (rTimeStart, sr1);
                        rHold.headSpeedList.Add((rTimeJudge, 1));
                        rHold.tailSpeedList[0] = (rTimeJudge, sr2);
                        rHold.tailSpeedList.Add((rTimeEnd, 1));
                        rHold.InitMoveAndScale(speed, rTimeStart);
                    }

                    if (noteLine.StartsWith("trail"))
                    {
                        //TODO: How to elegantly solve this? Can one line solve all the things??
                    }
                }
            }

            ptr++; //To next line
        }

        rTrack.lPosList.Add((rTrack.timeEnd, lPosEnd, 's'));
        rTrack.rPosList.Add((rTrack.timeEnd, rPosEnd, 's'));
    }

    static string[] GetParaString(string line)
    {
        return line.Split('(', ')')[1].Split(',');
        //A single line is like: head(para1,para2,para3,...);
        //The first step: get "para1,para2,para3,...";
        //The second step: get "para1","para2","para3"...
    }

    static bool IsNewElement(string line)
    {
        return line.StartsWith("unote") || line.StartsWith("track");
    }

    static int CmpTimeInNote(BaseNote x, BaseNote y)
    {
        return x.timeInstantiate > y.timeInstantiate ? 1 : -1;
    }

    static int CmpTimeInTrack(Track x, Track y)
    {
        return x.timeStart > y.timeStart ? 1 : -1;
    }
}
