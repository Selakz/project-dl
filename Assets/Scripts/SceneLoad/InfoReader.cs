using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;

//Read and store all level-relative information.
public class InfoReader : MonoBehaviour
{
    // Serializable and Public
    private bool isRead = false;

    // Private
    IPassInfo info = null;

    // Defined Function
    public void SetInfo(IPassInfo info)
    {
        this.info = info;
    }

    public I ReadInfo<I>() where I : IPassInfo
    {
        isRead = true;
        if (info == null) Debug.LogError("Read an information before set it.");

        return (I)info;
    }

    public bool HasRead()
    {
        return isRead;
    }

    // System Function

}
