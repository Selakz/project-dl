using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A common InfoPasser which can transfer information in different scenes?
public class InfoPasser : MonoBehaviour
{
    // Serializable and Public
    string songID = string.Empty;
    int difficulty = 0;
    static InfoPasser infoPasser;

    // Private

    // Static

    // Defined Function
    private static InfoPasser Instance
    {
        get
        {
            if (!infoPasser)
            {
                GameObject gameObject = new() { name = "InfoPass" };
                DontDestroyOnLoad(gameObject);
                infoPasser = gameObject.AddComponent<InfoPasser>();
            }
            return infoPasser;
        }
    }

    public static void SetInfo(string songID, int difficulty)
    {
        Instance.songID = songID;
        Instance.difficulty = difficulty;
    }

    public static (string songID, int difficulty) GetInfo()
    {
        return (Instance.songID, Instance.difficulty);
    }

    // System Function
}
