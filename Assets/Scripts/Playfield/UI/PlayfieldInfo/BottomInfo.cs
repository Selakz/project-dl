using TMPro;
using UnityEngine;

public class BottomInfo : MonoBehaviour
{
    // Serializable and Public
    public TMP_Text songName;
    public TMP_Text difficulty;

    // Private

    // Static
    readonly string[] diffName = { "Special", "Basic", "Advanced", "Master", "Extra" };

    // Defined Function

    // System Function
    void Awake()
    {
        LevelInfo levelInfo = GameObject.Find("InfoReader").GetComponent<InfoReader>().ReadInfo<LevelInfo>();
        songName.text = levelInfo.songInfo.title.GetString() + " - " + levelInfo.songInfo.composer.GetString();
        Difficulty diff = levelInfo.GetDifficulty();
        difficulty.text = diffName[diff.difficulty] + " Lv." + diff.rating;
    }
}
