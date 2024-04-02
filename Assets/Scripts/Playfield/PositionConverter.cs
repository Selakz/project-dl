using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionConverter : MonoBehaviour
{
    // Serializable and Public
    public Camera m_camera;

    // Private

    // Static

    // Defined Function
    public float G2WPosX(float input)
    {
        //G2W = Game2World
        float size = m_camera.orthographicSize;
        float aspect = m_camera.aspect;
        return input / InGameParameters.gameWidthSize * size * aspect;
    }

    public float G2WPosY(float input)
    {
        //G2W = Game2World
        float size = m_camera.orthographicSize;
        return input / InGameParameters.gameHeightSize * size;
    }

    public float W2GPosX(float input)
    {
        //W2G = World2Game
        float size = m_camera.orthographicSize;
        float aspect = m_camera.aspect;
        return input * InGameParameters.gameWidthSize / size / aspect;
    }

    public float W2GPosY(float input)
    {
        //W2G = World2Game
        float size = m_camera.orthographicSize;
        return input * InGameParameters.gameHeightSize / size;
    }

    // System Function
}
