using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkSceneManagerBase
{
    FusionCallbacks m_Callbacks;
    Scene loadedScene;

    public void LoadLevel(int levelIndex)
    {
        Runner.SetActiveScene(levelIndex);
    }

    protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
    {
        if (m_Callbacks == null)
        {
            m_Callbacks = GetComponent<FusionCallbacks>();
        }
        m_Callbacks.SetConnectionStatus(FusionCallbacks.ConnectionStatus.Loading, "");

        if (loadedScene != default)
        {
            yield return SceneManager.UnloadSceneAsync(loadedScene);
        }
        loadedScene = default;
        List<NetworkObject> sceneObjects = new List<NetworkObject>();
        if (newScene > 0)
        {
            yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
            loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
            sceneObjects = FindNetworkObjects(loadedScene, disable: false);
            // Delay one frame
            yield return null;
            m_Callbacks.SetConnectionStatus(FusionCallbacks.ConnectionStatus.Loaded, "");
         
        }
        finished(sceneObjects);
    }
}
