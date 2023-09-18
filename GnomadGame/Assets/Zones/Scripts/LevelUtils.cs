using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelUtils
{

    public static IEnumerator LoadNewSceneAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);


        while (!op.isDone)
        {
            yield return null;
        }
    }


}
