using System.Collections.Generic;

public class ChartInfo
{
    public float offset = 0f;
    public Volume volume;
    public Dictionary<char, List<BaseNote>> noteList;
    public List<Track> trackList;
}
