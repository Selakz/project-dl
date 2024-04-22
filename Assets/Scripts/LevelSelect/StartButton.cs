using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // Serializable and Public
    public SceneLoader sceneLoader;

    // Private

    // Static

    // Defined Function
    public void StartGame()
    {
        SongInfo songInfo = SongList.GetSongInfo(0);
        LevelInfo levelInfo = new(songInfo, 1);
        SceneLoadParameters parameters = new("Playfield", "Shutter", "SceneLoadDone");
        sceneLoader.SetInfo(levelInfo);
        sceneLoader.LoadScene(parameters);
    }

    // System Function
}
