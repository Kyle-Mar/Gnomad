using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



/// <summary>
/// The GameObject this script / class is attatched to should
/// have the same name as the Scene Object you are trying to maintain.
/// </summary>

public class SceneInfo : MonoBehaviour
{

    [SerializeField] List<SceneInfo> adjacentScenes; 

    public bool isLoaded { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            LevelManager.UpdateLoadedScenes(adjacentScenes, this);
        }
    }

    public void LoadScene()
    {
        StartCoroutine(LevelManager.LoadSceneAsync(gameObject.name));
        isLoaded = true;
    }

    public void UnloadScene()
    {
        StartCoroutine(LevelManager.UnloadSceneAsync(gameObject.name));
        isLoaded = false;
    }

}
