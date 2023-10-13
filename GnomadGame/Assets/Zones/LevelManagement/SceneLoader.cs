using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Eflatun.SceneReference;
using Gnomad.Utils;

public class SceneLoader : MonoBehaviour
{
    public SceneInfo sceneInfo;
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
        if(LevelManager.OccupiedScene != sceneInfo)
        {
            return;
        }
        if(scene.name == sceneInfo.scene.Name)
        {
            return;
        }


        int idx = sceneInfo.adjacentScenes.FindIndex(0, x => x.name == scene.name);
        if(idx == -1)
        {
            return;
        }
        Vector3 doorPosition = sceneInfo.DoorPositions[idx];

        #region FindCenters
        Vector3 curRoomCenter = Vector3.zero;
        var otherObjectsList = SceneManager.GetSceneByName(scene.name).GetRootGameObjects();
        var thisObjectsList = SceneManager.GetSceneByName(sceneInfo.scene.Name).GetRootGameObjects();

        CompositeCollider2D otherCol = new();
        CompositeCollider2D thisCol = new();


        foreach (var x in otherObjectsList)
        {
            Grid grid;
            if (x.TryGetComponent(out grid))
            {
                otherCol = x.GetComponentInChildren<CompositeCollider2D>();
                break;
            }
        }

        foreach (var x in thisObjectsList)
        {
            Grid grid;
            if (x.TryGetComponent(out grid))
            {
                thisCol = x.GetComponentInChildren<CompositeCollider2D>();
                break;
            }
        }


        curRoomCenter = thisCol.gameObject.transform.position + thisCol.bounds.extents;
        #endregion
        #region CalculateOffset

        var otherDoorIdx = sceneInfo.DoorConnections[idx];
        var adjacentScene = sceneInfo.adjacentScenes[idx];
        var otherDoorPosition = adjacentScene.DoorPositions[otherDoorIdx];

        Vector3 sign = Utils.Sign(doorPosition - curRoomCenter);
        if (sign.x == 0)
        {
            sign.x = 1;
        }
        if (sign.y == 0)
        {
            sign.y = 1;
        }
        
        //Debug.DrawRay(Utils.Vector3ToVector3Int(curRoomCenter + doorPosition + (-otherDoorPosition)), Vector3.up, Color.green, 10f);
#endregion

        foreach (var x in otherObjectsList)
        {
            x.transform.position += Utils.Vector3ToVector3Int(curRoomCenter + doorPosition + (-otherDoorPosition));
        }
    }

    public void LoadScene(SceneInfo scene)
    {
        StartCoroutine(LevelManager.LoadSceneAsync(scene.scene.Name));
        scene.isLoaded = true;
        return;
    }

    public void UnloadScene(SceneInfo scene)
    {
        scene.isLoaded = false;
        if (!SceneManager.GetSceneByName(scene.name).isLoaded)
        {
            return;
        }
        StartCoroutine(LevelManager.UnloadSceneAsync(scene.scene.Name));
    }
}
