using System.Collections.Generic;
using static InGameParameters;

public abstract class BaseNote
{
    public NoteType noteType;
    public int number;
    public float timeJudge;
    public float timeInstantiate;   //The time when it appears in screen
    public int belongingTrack;
}
