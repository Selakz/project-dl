public class SceneLoadParameters
{
    public readonly string sceneName;
    public readonly string prefabName;
    public readonly string triggerName = "SceneLoadDone";

    public SceneLoadParameters(string sceneName, string prefabName, string triggerName)
    {
        this.sceneName = sceneName;
        this.prefabName = prefabName;
        this.triggerName = triggerName;
    }
}
