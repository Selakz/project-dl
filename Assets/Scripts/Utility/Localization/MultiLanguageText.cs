using UnityEngine;
using static IMultiLanguage;

public class MultiLanguageText : IMultiLanguage
{
    public string GetString()
    {
        int language = PlayerPrefs.GetInt("Language");
        // Find an arrangement to read from texts.
        throw new System.NotImplementedException();
    }
}
