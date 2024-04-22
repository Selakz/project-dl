using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Switch scene and pass information through scenes.<para/>
/// It's considered that the animation it used only needs one trigger from start state to end.<para/>
/// </summary>
public class SceneLoader : MonoBehaviour
{
    // Serializable and Public

    // Private
    private bool isLoadStart = false;
    private IPassInfo info = default;
    private PackagedAnimator sceneSwitchAnimator;
    private AsyncOperation asyncLoad;
    private InfoReader infoReader;

    // Static

    // Defined Function
    public void SetInfo(IPassInfo info)
    {
        this.info = info;
    }

    public void LoadScene(object sceneLoadParameters)
    {
        SceneLoadParameters paras = (SceneLoadParameters)sceneLoadParameters;

        isLoadStart = true;

        sceneSwitchAnimator = new("Shutter", transform, false);
        sceneSwitchAnimator.Play();

        // Destroy all the infoReader in the scene to avoid taking it to next scene.
        foreach (var irc in FindObjectsByType<InfoReader>(FindObjectsSortMode.None))
        {
            Destroy(irc.gameObject);
        }
        // Then add the actual infoReader to next scene
        GameObject ir = new() { name = "InfoReader" };
        ir.transform.parent = gameObject.transform;
        infoReader = ir.AddComponent<InfoReader>();
        infoReader.SetInfo(info);

        StartCoroutine(ILoadScene(paras.sceneName, paras.triggerName));
    }

    IEnumerator ILoadScene(string sceneToLoad, string triggerParam)
    {
        yield return new WaitForSeconds(0.5f);
        asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        sceneSwitchAnimator.SetParam(triggerParam);
    }

    void DestroyWhenOver()
    {
        //It requires that the scene switch prefab has DestroyAfterAnimEnd.
        if (sceneSwitchAnimator.IsOver() && asyncLoad.isDone)
        {
            // Throw infoReader out to avoid being destroyed.
            infoReader.gameObject.transform.SetParent(null);
            Destroy(gameObject);
        }
    }

    // System Function
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (isLoadStart)
        {
            DestroyWhenOver();
        }
    }
}
