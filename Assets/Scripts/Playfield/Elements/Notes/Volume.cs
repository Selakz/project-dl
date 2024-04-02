using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InGameParameters;

//Volume means the count of notes in a chart. It's not the intensity of sound qwq
public class Volume
{
    int all;
    int uNote;
    int tNote;
    int btNote;
    int rtNote;
    int dtNote;

    public Volume()
    {
        all = uNote = tNote = btNote = rtNote = dtNote = 0;
    }

    public void AddNote(NoteType noteType)
    {
        all++;
        switch (noteType)
        {
            case NoteType.UNote:
                uNote++;
                break;
            case NoteType.BTNote:
                btNote++;
                tNote++;
                break;
            case NoteType.RTNote:
                rtNote++;
                tNote++;
                break;
            case NoteType.DTNote:
                dtNote++;
                tNote++;
                break;
            default:
                Debug.LogError("Invalid input in volume");
                break;
        }
    }

    public int GetNote(NoteType noteType)
    {
        return noteType switch
        {
            NoteType.Any => all,
            NoteType.UNote => uNote,
            NoteType.TNote => tNote,
            NoteType.BTNote => btNote,
            NoteType.RTNote => rtNote,
            NoteType.DTNote => dtNote,
            _ => 0,
        };
    }
}

