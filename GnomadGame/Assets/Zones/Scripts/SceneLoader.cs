using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneLoader : MonoBehaviour
{
    public SceneInfo sceneInfo;

    public List<GameObject> DoorPositions = new(); // this is related in order to the sceneInfo's adjacent scene list
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            LevelManager.UpdateLoadedScenes(sceneInfo.adjacentScenes, sceneInfo, this);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLoadScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded += OnLoadScene;
    }

    void OnLoadScene(Scene scene, LoadSceneMode mode)
    {


        // Get root objects and search for the composite collider which has the information of 
        // the size of the tilemap and not the tilemap collider for some reason.
        var objectsList = SceneManager.GetSceneByName(scene.name).GetRootGameObjects();
        CompositeCollider2D col = new();
        
        int idx = sceneInfo.adjacentScenes.FindIndex(0, x => x.name == scene.name);
        GameObject doorPos = DoorPositions[idx];
        Vector3 doorPosition = doorPos.transform.position;
        Vector3 curRoomCenter = Vector3.zero;


        foreach (var x in objectsList)
        {
            Grid grid;
            if (x.TryGetComponent(out grid))
            {
                col = x.GetComponentInChildren<CompositeCollider2D>();
                Debug.Log(col);
                break;
            }
        }
        foreach (var x in objectsList)
        {
            Vector3 w = curRoomCenter + doorPosition;


            Vector3 sign = Utils.Sign(doorPosition - curRoomCenter);
            if (sign.x == 0)
            {
                sign.x = 1;
            }
            if (sign.y == 0)
            {
                sign.y = 1;
            }
            Vector3 a = new(col.bounds.extents.x * sign.x , (int)col.bounds.extents.y/2, 0);
            Debug.DrawRay((w+a), Vector3.up, Color.blue, 10f);
            x.transform.position = (w+a);
            //x.transform.position += col.bounds.center - x.transform.position;
        }
    }

    public void LoadScene(SceneInfo scene)
    {
        StartCoroutine(LevelManager.LoadSceneAsync(scene.name));
        scene.isLoaded = true;
        return;
    }

    public void UnloadScene(SceneInfo scene)
    {
        StartCoroutine(LevelManager.UnloadSceneAsync(scene.name));
        scene.isLoaded = false;
    }
}
