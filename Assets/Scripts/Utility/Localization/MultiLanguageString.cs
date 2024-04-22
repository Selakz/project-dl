using UnityEngine;
using static IMultiLanguage;

public class MultiLanguageString : IMultiLanguage
{
    private string _en = "<missing base text>";
    private string _zh_s = null;
    private string _zh_t = null;

    public string en { get { return _en; } set { _en = value; } }
    public string zh_s { get { return _zh_s ?? _en; } set { _zh_s = value; } }
    public string zh_t { get { return _zh_t ?? _en; } set { _zh_t = value; } }

    public string GetString()
    {
        int language = PlayerPrefs.GetInt("Language");
        return language switch
        {
            (int)LanguageLabel.en => en,
            (int)LanguageLabel.zh_s => zh_s,
            (int)LanguageLabel.zh_t => zh_t,
            _ => en
        };
    }
}
