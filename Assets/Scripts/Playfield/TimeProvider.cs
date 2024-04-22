using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InGameParameters;

//The TimeProvider provides both actual music time and chart time,
//so it also takes the role to play the music.
public class TimeProvider : MonoBehaviour
{
    // Serializable and Public
    [SerializeField] float _audioTime = 0f;
    public InfoReader infoReader;
    public AudioSource audioSource;
    public float AudioTime
    {
        get
        {
            return _audioTime;
        }
    }
    public float ChartTime
    {
        get
        {
            return _audioTime - audioDeviation - offset;
            //audioDeviation > 0: Chart appears later; < 0: The opposite.
        }
    }

    public float audioDeviation = 0f;

    // Private
    LevelInfo levelInfo;
    bool isPaused = false;
    float dspTimeStart = 0f;
    float offset = 0f;

    // Static
    const float timeScheduled = 0.050f; //Used for AudioSource.PlayScheduled().

    // Defined Function
    void TimeInit()
    {
        audioSource.clip = levelInfo.music;
        audioSource.time = 0f;
        audioDeviation = levelInfo.musicSetting.audioDeviation;

        _audioTime = 0f;
        dspTimeStart = (float)AudioSettings.dspTime + timePreAnimation + timeScheduled;
        audioSource.PlayScheduled(dspTimeStart);

        isPaused = false;
    }

    void MusicPause()
    {
        _audioTime = (float)(AudioSettings.dspTime - dspTimeStart);
        audioSource.Stop();
        audioSource.time = _audioTime + timeScheduled;

        isPaused = true;
    }

    void MusicResume()
    {
        float current = (float)AudioSettings.dspTime;
        audioSource.PlayScheduled(current + timeScheduled);
        dspTimeStart = current - _audioTime;

        isPaused = false;
    }

    // System Function
    void Awake()
    {
        EventManager.AddListener(EventManager.EventName.LevelInit, TimeInit);
        EventManager.AddListener(EventManager.EventName.Pause, MusicPause);
        EventManager.AddListener(EventManager.EventName.Resume, MusicResume);
        EventManager.AddListener(EventManager.EventName.SetOffset, (object t) => { offset = (float)t; });

        infoReader = GameObject.Find("InfoReader").GetComponent<InfoReader>();
        levelInfo = infoReader.ReadInfo<LevelInfo>();
    }

    void Update()
    {
        if (!isPaused)
        {
            _audioTime = (float)(AudioSettings.dspTime - dspTimeStart);
        }
    }
}
