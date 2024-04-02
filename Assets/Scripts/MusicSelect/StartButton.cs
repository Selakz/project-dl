using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // Serializable and Public
    public GameObject floatOver;

    // Private

    // Static

    // Defined Function
    public void StartGame()
    {
        InfoPasser.SetInfo("TestLevel", 1);

        Transform canvas = GameObject.Find("FloatCanvas").transform;
        Instantiate(floatOver, canvas);
        DontDestroyOnLoad(canvas);
        SceneManager.LoadScene("Playfield");
    }

    // System Function
}
