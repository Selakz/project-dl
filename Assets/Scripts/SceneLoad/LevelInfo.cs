using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class LevelInfo : IPassInfo
{
    public MusicSetting musicSetting;
    public Texture2D cover;
    public AudioClip music;
    public SongInfo songInfo;
    public ChartInfo chartInfo;
    public int difficulty;

    public LevelInfo(SongInfo songInfo, int difficulty = 3)
    {
        this.songInfo = songInfo;
        this.difficulty = difficulty;
        string loadPath = "Levels/" + songInfo.id + "/";
        string readPath = Application.dataPath + "/Resources/Levels/" + songInfo.id;

        cover = Resources.Load<Texture2D>(loadPath + "cover");
        music = Resources.Load<AudioClip>(loadPath + "music");

        ReadSettings();
        chartInfo = ChartReader.Read(readPath, difficulty, musicSetting.speed);
    }

    //This is not in SongInfo because only in a level do you know which difficulty you select.
    public Difficulty GetDifficulty()
    {
        foreach (var singleDiff in songInfo.difficulties)
        {
            if (singleDiff.difficulty == difficulty)
            {
                return singleDiff;
            }
        }
        return new Difficulty();
    }

    private void ReadSettings()
    {
        string json = File.ReadAllText(Application.dataPath + "/InfoData/MusicSetting.json");
        musicSetting = JsonConvert.DeserializeObject<MusicSetting>(json);
    }

    // Obselete: StartCoroutine() needs MonoBehaviour. It's bad.
    //
    // IEnumerator GetCover(string levelPath)
    // {
    //     UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(levelPath + "/cover.jpg");
    //     yield return uwr.SendWebRequest();
    //     if (uwr.result != UnityWebRequest.Result.ProtocolError)
    //     {
    //         cover = DownloadHandlerTexture.GetContent(uwr);
    //     }
    // }

    // IEnumerator GetMusic(string levelPath)
    // {
    //     UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(levelPath + "/music.mp3", AudioType.MPEG);
    //     yield return uwr.SendWebRequest();
    //     if (uwr.result != UnityWebRequest.Result.ProtocolError)
    //     {
    //         music = DownloadHandlerAudioClip.GetContent(uwr);
    //     }
    // }
}
