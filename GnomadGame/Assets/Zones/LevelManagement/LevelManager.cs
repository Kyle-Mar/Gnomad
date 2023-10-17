using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelManager
{

    [SerializeField] static List<SceneInfo> loadedScenes = new List<SceneInfo>();
    [SerializeField] static float timeTillNextUpdate = 0f;
    const float NextUpdateOffset = 1f;
    public static SceneInfo OccupiedScene;

    /// <summary>
    /// Updates the loaded scenes.
    /// </summary>
    /// <param name="connectedScenes">All the connected scenes (rooms) to the root room.</param>
    /// <param name="occupiedScene">The scene the player is presently in.</param>
    public static void UpdateLoadedScenes(List<SceneInfo> connectedScenes, SceneInfo occupiedScene, SceneLoader loader)
    {
        // list of scenes we plan to remove.
        List<SceneInfo> removeScenes = new List<SceneInfo>();
        if (timeTillNextUpdate > 0f)
        {
            return;
        }
        // check to see if the occupied scene is loaded already.
        // if not load it.
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
        // set the LevelManager's occupied scene to the paramter.
        OccupiedScene = occupiedScene;
        // unload each scene that isn't in the occupied scene's list
        // and is currently a loaded scene
        foreach (SceneInfo scene in loadedScenes)
        {
            if (!connectedScenes.Contains(scene) && scene != occupiedScene)
            {
                loader.UnloadScene(scene);
                removeScenes.Add(scene);
            }
        }
        //load each scene that isn't loaded already and add it to the loaded scenes list
        foreach (SceneInfo scene in connectedScenes)
        {
            if (!loadedScenes.Contains(scene))
            {
                Debug.Log(scene.name);
                loadedScenes.Add(scene);
                loader.LoadScene(scene);
            }
        }
        // remove each scene from loaded scenes that we unloaded.
        foreach (SceneInfo scene in removeScenes)
        {
            loadedScenes.Remove(scene);
        }
    }

    public static IEnumerator LoadSceneAsync(string sceneName)
    {
        
        if (sceneName == "reloadScene")
        {
            sceneName = SceneManager.GetActiveScene().name;
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!op.isDone)
        {
            yield return null;
        }
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
