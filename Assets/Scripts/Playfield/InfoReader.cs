using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;

//Read and store all level-relative information.
public class InfoReader : MonoBehaviour
{
    // Setting and resources
    public MusicSetting musicSetting;
    public Texture2D cover;
    public AudioClip music;

    // Chart
    public float offset;
    public Volume volume;
    public Dictionary<char, List<BaseNote>> NoteList;
    public List<Track> trackList;

    // InfoPasser
    string songID = "";
    int difficulty = 1;

    // Static

    // Defined Function
    public void Read()
    {
        (songID, difficulty) = InfoPasser.GetInfo();
        string levelPath = Application.dataPath + "/Resources/Levels/" + songID;

        StartCoroutine(GetCover(levelPath));
        StartCoroutine(GetMusic(levelPath));

        ReadSetting();
        (offset, volume, NoteList, trackList) = ChartReader.Read(levelPath, difficulty, musicSetting.speed);
    }

    void ReadSetting()
    {
        string stringSetting = File.ReadAllText(Application.dataPath + "/InfoData/MusicSetting.json");
        musicSetting = JsonConvert.DeserializeObject<MusicSetting>(stringSetting);
    }

    IEnumerator GetCover(string levelPath)
    {
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(levelPath + "/cover.jpg");
        yield return uwr.SendWebRequest();
        if (uwr.result != UnityWebRequest.Result.ProtocolError)
        {
            cover = DownloadHandlerTexture.GetContent(uwr);
        }
    }

    IEnumerator GetMusic(string levelPath)
    {
        UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(levelPath + "/music.mp3", AudioType.MPEG);
        yield return uwr.SendWebRequest();
        if (uwr.result != UnityWebRequest.Result.ProtocolError)
        {
            music = DownloadHandlerAudioClip.GetContent(uwr);
        }
    }

    // System Function
    void Awake()
    {
        Read();
    }
}
