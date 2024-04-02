using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static InGameParameters;
using UnityEngine.UI;

//Functions: Initialize Level, controls pauseMenu, handle player input.
//Things become cumbersome here...
public class LevelManager : MonoBehaviour
{
    //Serializable and Public
    public GameObject uNote, btNote, rtNote, dtNote, btHold, rtHold, dtHold;
    public GameObject track;
    public GameObject pauseMenu;
    public GameObject countdown;
    public TimeProvider timeProvider;
    public InfoReader infoReader;
    public RawImage cover;
    public float current;
    public float visionDeviation = 0f;
    //TODO: vision judge height calculate. audioDeviation is in TimeProvider

    //Private
    bool isPaused = false;
    float timediff;
    float UPerfectScore;
    float TPerfectScore;
    Volume volume = new();
    Dictionary<char, List<float>> inputArray = new();
    Dictionary<char, List<BaseNote>> noteArray = new();
    Dictionary<char, List<GameObject>> noteCreated = new();
    Dictionary<char, int> noteIndex = new();  //Used to create note
    Dictionary<char, int> targetIndex = new();    //Used to judge which note to transfer input
    List<Track> trackArray = new();
    Dictionary<int, GameObject> trackCreated = new();
    int trackIndex = 0;

    //Static
    readonly char[] colorSet = { 't', 'b', 'r', 'd' };

    //Defined Function

    private void CreateTrack(int index)
    {
        //Create a track in game, and store its gameobject in a dictionary
        GameObject tempTrack = Instantiate(track);
        tempTrack.GetComponent<TrackController>().InfoInit(trackArray[index]);
        trackCreated.Add(trackArray[index].number, tempTrack);
    }

    private void CreateNote(char c, int index)
    {
        //Create a note in game, and store its gameobject in an array
        GameObject note = Instantiate(c switch
        {
            't' => uNote,
            'b' => (noteArray[c][index] is Note) ? btNote : btHold,
            'r' => (noteArray[c][index] is Note) ? rtNote : rtHold,
            'd' => (noteArray[c][index] is Note) ? dtNote : dtHold,
            _ => null
        });
        if (noteArray[c][index] is Note) note.GetComponent<NoteController>().InfoInit(noteArray[c][index]);
        else note.GetComponent<HoldController>().InfoInit(noteArray[c][index]);

        noteCreated[c].Add(note);
        note.transform.SetParent(trackCreated[noteArray[c][index].belongingTrack].transform, false);
    }

    //It's so silly... But I can't find how to transfer custom parameter into the method...
    public void StoreInputT(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    if (!isPaused)
                    {
                        inputArray['t'].Add(current);
                    }
                    break;
                default:
                    break;
            }
        }
    }
    public void StoreInputB(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    if (!isPaused)
                    {
                        inputArray['b'].Add(current);
                    }
                    break;
                default:
                    break;
            }
        }
    }
    public void StoreInputR(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    if (!isPaused)
                    {
                        inputArray['r'].Add(current);
                    }
                    break;
                default:
                    break;
            }
        }
    }
    public void StoreInputD(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    if (!isPaused)
                    {
                        inputArray['d'].Add(current);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void HandleInput()
    {
        foreach (var c in colorSet)
        {
            foreach (float timeInput in inputArray[c])
            {
                //If the screen has none of this color, skip the input
                if (noteCreated[c].Count == 0 || targetIndex[c] >= noteCreated[c].Count)
                {
                    continue;
                }
                //Find the nearest note when input
                timediff = Math.Abs(noteArray[c][targetIndex[c]].timeJudge - timeInput);
                while (targetIndex[c] < noteCreated[c].Count - 1 && Math.Abs(noteArray[c][targetIndex[c] + 1].timeJudge - timeInput) < timediff)
                {
                    timediff = Math.Abs(noteArray[c][targetIndex[c] + 1].timeJudge - timeInput);
                    targetIndex[c]++;
                }
                //If the nearest note is destroyed, find next active note
                while (targetIndex[c] < noteCreated[c].Count && noteCreated[c][targetIndex[c]] == null)
                {
                    targetIndex[c]++;
                }
                if (targetIndex[c] < noteCreated[c].Count)
                {
                    //See if the input is valid
                    if (noteCreated[c][targetIndex[c]].TryGetComponent<NoteController>(out var noteController))
                    {
                        if (noteController.HandleInput(timeInput, c switch { 't' => UPerfectScore, _ => TPerfectScore }))
                            targetIndex[c]++;
                    }
                    else
                    {
                        if (noteCreated[c][targetIndex[c]].GetComponent<HoldController>().HandleInput(timeInput, c switch { 't' => UPerfectScore, _ => TPerfectScore }))
                            targetIndex[c]++;
                    }
                }
            }
            inputArray[c].Clear();
        }
    }

    private void InstantiateNoteAndTrack()
    {
        //Instantiate tracks
        while (trackIndex < trackArray.Count && trackArray[trackIndex].timeStart < current)
        {
            CreateTrack(trackIndex);
            trackIndex++;
        }

        //Instantiate notes
        foreach (var c in colorSet)
        {
            while (noteIndex[c] < noteArray[c].Count && noteArray[c][noteIndex[c]].timeInstantiate < current)
            {
                CreateNote(c, noteIndex[c]);
                noteIndex[c]++;
            }
        }
    }

    public void DetectPause(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    if (isPaused == false && countdown.activeInHierarchy == false)
                    {
                        EventManager.Trigger(EventManager.EventName.Pause);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        //Maybe this method will get longer after.
        isPaused = false;
    }

    public void CalcScore()
    {
        int all = volume.GetNote(NoteType.Any), tnote = volume.GetNote(NoteType.TNote);
        if (all != 0)
        {
            if (tnote != 0)
            {
                UPerfectScore = aveScore / all;
                TPerfectScore = aveScore / all + tNoteScore / tnote;
            }
            else
            {
                UPerfectScore = maxScore / all;
            }
        }
    }

    public void LevelInit()
    {
        //Init chart
        volume = infoReader.volume;
        noteArray = infoReader.NoteList;
        trackArray = infoReader.trackList;

        //Init real time chart info and input
        inputArray.Clear();
        noteCreated.Clear();
        noteIndex.Clear();
        targetIndex.Clear();
        foreach (var color in colorSet)
        {
            inputArray.Add(color, new());
            noteCreated.Add(color, new());
            noteIndex.Add(color, 0);
            targetIndex.Add(color, 0);
        }
        trackCreated.Clear();
        trackIndex = 0;

        //Others
        cover.texture = infoReader.cover;
        CalcScore();
        isPaused = false;
    }

    //System Function
    void Awake()
    {
        //TODO: Read from Settings.json to gain levelPath, deviations and more.
        EventManager.AddListener(EventManager.EventName.LevelInit, LevelInit);
        EventManager.AddListener(EventManager.EventName.Pause, Pause);
        EventManager.AddListener(EventManager.EventName.Resume, Resume);
        pauseMenu.SetActive(false);
    }

    void Start()
    {
        EventManager.Trigger(EventManager.EventName.LevelInit);
    }

    void Update()
    {
        if (!isPaused) //It's theoretically unnecessary, but maybe leaving it can optimize performance? (a little?)
        {
            current = timeProvider.ChartTime;
            InstantiateNoteAndTrack();
            HandleInput();
        }
    }
}
