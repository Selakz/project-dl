using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Directly instantiating a SongList will get an empty songList.
/// </summary>
public class SongList
{
    //TODO: 选关界面排序思路：选关界面用一个List<SongInfo>决定展示的歌。先GetSongList()，然后
    // 对这个SongList进行或排序或筛选的函数，用函数返回的List进行展示。而内部实际上是一直存着所有的songs，
    // 筛选则用songs生成新的filteredSongs，排序则是直接对filteredSongs进行排序。
    public List<SongInfo> songs;
    private List<SongInfo> filteredSongs;

    /// <summary>
    /// The return list should not be modified.
    /// </summary>
    public static SongList GetSongList()
    {
        string json = File.ReadAllText(Application.dataPath + "/InfoData/SongList.json");
        SongList songList = JsonConvert.DeserializeObject<SongList>(json);
        return songList;
    }

    public static SongInfo GetSongInfo(int idx)
    {
        SongList songList = GetSongList();
        // Because idx is non-negative, so it's guaranteed that list index <= idx for any SongInfo.
        for (int i = Math.Min(idx, songList.songs.Count - 1); i >= 0; i--)
        {
            if (songList.songs[i].idx == idx)
            {
                return songList.songs[i];
            }
        }
        return new();
    }
    public static SongInfo GetSongInfo(int idx, List<SongInfo> songList)
    {
        // Because idx is non-negative, so it's guaranteed that list index <= idx for any SongInfo.
        for (int i = Math.Min(idx, songList.Count - 1); i >= 0; i--)
        {
            if (songList[i].idx == idx)
            {
                return songList[i];
            }
        }
        return new();
    }
    public static SongInfo GetSongInfo(string id)
    {
        SongList songList = GetSongList();
        for (int i = songList.songs.Count - 1; i >= 0; i--)
        {
            if (songList.songs[i].id == id)
            {
                return songList.songs[i];
            }
        }
        return new();
    }
    public static SongInfo GetSongInfo(string id, List<SongInfo> songList)
    {
        for (int i = songList.Count - 1; i >= 0; i--)
        {
            if (songList[i].id == id)
            {
                return songList[i];
            }
        }
        return new();
    }

    public static void SaveScore(int idx, int difficulty, int score)
    {
        SongList songList = GetSongList();
        SongInfo songInfo = GetSongInfo(idx, songList.songs);
        foreach (Difficulty diff in songInfo.difficulties)
        {
            if (diff.difficulty == difficulty) diff.score = score;
        }
        SaveSongList(songList.songs);
    }

    /// <summary>
    /// Save a SongInfo list to SongList.json. The list will be sorted by idx.<br/>
    /// Warn that this will directly influence the game's reading songs,
    /// so never use it without GetSongList()!!!
    /// </summary>
    private static void SaveSongList(List<SongInfo> toSaveSongs)
    {
        // TODO: Safety check
        toSaveSongs.Sort((SongInfo x, SongInfo y) => { return x.idx < y.idx ? -1 : 1; });
        SongList toWrite = new() { songs = toSaveSongs };
        File.WriteAllText(Application.dataPath + "/InfoData/SongList.json", JsonConvert.SerializeObject(toWrite, Formatting.Indented));
    }
}

/// <summary>
/// All information about a song.
/// </summary>
public class SongInfo : IPassInfo
{
    // TODO: multi-language support.
    public int idx = 0;
    public string id = "Default";
    public MultiLanguageString title;
    public MultiLanguageString composer;
    public string bpmDisplay;
    public int bpmDefault;
    public string set = "base";
    public MultiLanguageString description;
    public List<Difficulty> difficulties;
}

/// <summary>
/// Information in one single difficulty.
/// </summary>
public class Difficulty
{
    public int difficulty = 3;
    public string chart = "<missing charter>";
    public string illust = "<missing illustrator>";
    public int rating = 0;
    public bool ratingPlus = false;
    public int score = 0;
}