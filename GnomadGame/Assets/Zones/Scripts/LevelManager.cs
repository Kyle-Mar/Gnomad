using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelManager
{

    [SerializeField] static List<SceneInfo> loadedScenes = new List<SceneInfo>();
    [SerializeField] static float timeTillNextUpdate = 0f;
    const float NextUpdateOffset = 1f;

    /// <summary>
    /// Updates the loaded scenes.
    /// </summary>
    /// <param name="connectedScenes">All the connected scenes (rooms) to the root room.</param>
    /// <param name="occupiedScene">The scene the player is presently in.</param>
    public static void UpdateLoadedScenes(List<SceneInfo> connectedScenes, SceneInfo occupiedScene, SceneLoader loader)
    {
        List<SceneInfo> removeScenes = new List<SceneInfo>();
        if (timeTillNextUpdate > 0f)
        {
            return;
        }
        if (SceneManager.GetSceneByName(occupiedScene.name).isLoaded)
        {
            loader.sceneInfo.isLoaded = true;
            loadedScenes.Add(loader.sceneInfo);
        }
        else
        {
            if (!occupiedScene.isLoaded)
            {
                loadedScenes.Add(occupiedScene);
                loader.LoadScene(occupiedScene);
            }
        }
        foreach (SceneInfo scene in loadedScenes)
        {
            if (!connectedScenes.Contains(scene) && scene != occupiedScene)
            {
                loader.UnloadScene(scene);
                removeScenes.Add(scene);
            }
        }

        foreach (SceneInfo scene in connectedScenes)
        {
            if (!loadedScenes.Contains(scene))
            {
                Debug.Log(scene.name);
                loadedScenes.Add(scene);
                loader.LoadScene(scene);
            }
        }

        foreach (SceneInfo scene in removeScenes)
        {
            loadedScenes.Remove(scene);
        }
    }

    public static IEnumerator LoadSceneAsync(SceneInfo sceneInfo)
    {
        var sceneName = sceneInfo.name;
        
        int idx = sceneInfo.adjacentScenes.FindIndex(0, x => x.name == sceneName);
        if (idx == -1)
        {
            yield return null;
        }
        
        var doorPos = sceneInfo.DoorPositions[idx];
        if (sceneName == "reloadScene")
        {
            sceneName = SceneManager.GetActiveScene().name;
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!op.isDone)
        {
            yield return null;
        }
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        GameObject[] gameObjects = loadedScene.GetRootGameObjects();
        CompositeCollider2D col = new();

        Vector3 doorPosition = doorPos.transform.position;
        Vector3 curRoomCenter = Vector3.zero;
    }

    public static IEnumerator UnloadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);

        while (!op.isDone)
        {
            yield return null;
        }
    }

}
