using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*EventManager is used to handle events.
 *It's required that one and only one EventManger is active in scene.
 *Current EventManager can handle methods of no param and of one param(any type).
 */
public class EventManager : MonoBehaviour
{
    // Serializable and Public
    public enum EventName
    {
        //In LevelManager
        LevelInit,
        Pause,
        Resume,
        //In ComboController
        UpdateCombo,
        //In ScoreController
        UpdateScore,
        //In TimeProvider
        SetOffset,
    };
    [SerializeField] EventName checkEventName; //Only used in inspector to quickly check current event names.
    // Private
    Dictionary<EventName, UnityEvent> events;
    Dictionary<EventName, UnityEvent<object>> paraEvents;
    static EventManager eventManager;

    // Static

    // Defined Funtion
    private static EventManager Instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (!eventManager)
                    Debug.LogError("There needs to be one and only one active EventManager script on a GameObject in the scene.");
                else
                    eventManager.Init();
            }
            return eventManager;
        }
    }

    void Init()
    {
        events ??= new();
        paraEvents ??= new();
    }

    public static void AddListener(EventName evtName, UnityAction listener)
    {
        if (Instance.events.TryGetValue(evtName, out UnityEvent evt))
        {
            evt.AddListener(listener);
        }
        else
        {
            evt = new UnityEvent();
            evt.AddListener(listener);
            Instance.events.Add(evtName, evt);
        }
    }
    public static void AddListener(EventName evtName, UnityAction<object> listener)
    {
        if (Instance.paraEvents.TryGetValue(evtName, out UnityEvent<object> evt))
        {
            evt.AddListener(listener);
        }
        else
        {
            evt = new();
            evt.AddListener(listener);
            Instance.paraEvents.Add(evtName, evt);
        }
    }

    public static void RemoveListener(EventName evtName, UnityAction listener)
    {
        if (eventManager == null) return;
        if (Instance.events.TryGetValue(evtName, out UnityEvent evt))
        {
            evt.RemoveListener(listener);
        }
        else
        {
            Debug.LogWarning("Invalid removing listener.");
        }
    }
    public static void RemoveListener(EventName evtName, UnityAction<object> listener)
    {
        if (eventManager == null) return;
        if (Instance.paraEvents.TryGetValue(evtName, out UnityEvent<object> evt))
        {
            evt.RemoveListener(listener);
        }
        else
        {
            Debug.LogWarning("Invalid removing listener.");
        }
    }

    public static void Trigger(EventName evtName)
    {
        if (Instance.events.TryGetValue(evtName, out UnityEvent evt))
        {
            evt.Invoke();
        }
        else
        {
            Debug.LogWarning("Invalid trigger event.");
        }
    }
    public static void Trigger(EventName evtName, object data)
    {
        if (Instance.paraEvents.TryGetValue(evtName, out UnityEvent<object> evt))
        {
            evt.Invoke(data);
        }
        else
        {
            Debug.LogWarning("Invalid event triggering.");
        }
    }

    // System Funtion
}
