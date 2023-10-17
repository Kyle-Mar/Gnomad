using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Eflatun.SceneReference;
using Gnomad.Utils;

[RequireComponent(typeof(Collider2D))]
public class SceneLoader : MonoBehaviour
{
    //The relevant data about the room.
    public SceneInfo sceneInfo;
    //When the player enters a new room, update the currently loaded scenes.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            LevelManager.UpdateLoadedScenes(sceneInfo.adjacentScenes, sceneInfo, this);
        }
    }

    //Add the SceneLoader to Unity's Scene Manager Event.
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLoadScene;
    }

    //Remove it when the SceneLoader is disabled.
    private void OnDisable()
    {
        SceneManager.sceneLoaded += OnLoadScene;
    }

    void OnLoadScene(Scene scene, LoadSceneMode mode)
    {

        //Assure that this only runs once on the new Occupied Scene.
        //This is called after UpdateLoadedScenes because
        //Update Loaded Scenes is what loads and unloads the scenes.
        //So The event is only invoked once the scenes loaded and the occupiedscene is now set.
        if(LevelManager.OccupiedScene != sceneInfo)
        {
            return;
        }
        // Don't want to run this for newly loaded scenes.
        if(scene.name == sceneInfo.scene.Name)
        {
            return;
        }

        // Find the adjacent scene where the name matches the just loaded scene name.
        int idx = sceneInfo.adjacentScenes.FindIndex(0, x => x.scene.Name == scene.name);
        // if we can't find it, it wasn't a room.
        if(idx == -1)
        {
            return;
        }
        Vector3 doorPosition = sceneInfo.DoorPositions[idx];

        #region FindCenters
        // Get root objects and search for the composite collider which has the information of 
        // the size of the tilemap and not the tilemap collider for some reason.
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

        // This Room's center based on the collider.    Top Left + half extents = center
        curRoomCenter = thisCol.gameObject.transform.position + thisCol.bounds.extents;
        #endregion
        #region CalculateOffset

        // Get the other door's index from the door connections dictionary
        var otherDoorIdx = sceneInfo.DoorConnections[idx];
        // get the other scene info from the adjacent scenes based on the index that we got.
        // this means that the value position in the adjacent scenes list must match what value we put in the 
        // door connections dictionary other wise it will load the wrong scene.
        var adjacentScene = sceneInfo.adjacentScenes[idx];
        // get the doorposition that we want to connect to in the other scene
        // Same consequence as above. Door position idx must match adjacent scene idx and doorposition value.
        var otherDoorPosition = adjacentScene.DoorPositions[otherDoorIdx];
        
        //Debug.DrawRay(Utils.Vector3ToVector3Int(curRoomCenter + doorPosition + (-otherDoorPosition)), Vector3.up, Color.green, 10f);
#endregion

        //Scene starts at 0,0,0
        // move every GO by the center of the current room + the current rooms door position - the other room door's position.
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
