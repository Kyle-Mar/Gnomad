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
    [SerializeField] CompositeCollider2D tilemapCollider;
    // THIS PROBABLY SHOULDN'T BE HERE BUT I CAN'T FIGURE OUT A BETTER WAY.
    [SerializeField] CompositeCollider2D boundingCollider;
    public CompositeCollider2D TilemapCollider { get => tilemapCollider; set => tilemapCollider = value; }
    bool isEnteredByPlayer = false;

    //When the player enters a new room, update the currently loaded scenes.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //Debug.Log(collision.name + sceneInfo.scene.Name);

            if (isEnteredByPlayer)
            {
                return;
            }
            isEnteredByPlayer = true;
            LevelManager.UpdateLoadedScenes(sceneInfo.adjacentScenes, sceneInfo, this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isEnteredByPlayer = false;   
    }

    object objLock = new();

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
        lock (objLock)
        {
            //Assure that this only runs once on the new Occupied Scene.
            //This is called after UpdateLoadedScenes because
            //Update Loaded Scenes is what loads and unloads the scenes.
            //So The event is only invoked once the scenes loaded and the occupiedscene is now set.
            if (LevelManager.OccupiedScene != sceneInfo)
            {
                return;
            }
            // Don't want to run this for newly loaded scenes.
            if (scene.name == sceneInfo.scene.Name)
            {
                return;
            }

            // Find the adjacent scene where the name matches the just loaded scene name.
            int idx = sceneInfo.adjacentScenes.FindIndex(0, x => x.scene.Name == scene.name);
            // if we can't find it, it wasn't a room.
            if (idx == -1)
            {
                return;
            }
            Vector3 doorPosition = sceneInfo.DoorPositions[idx];

            #region FindCenters
            // Get root objects and search for the composite collider which has the information of 
            // the size of the tilemap and not the tilemap collider for some reason.
            Vector3 curRoomCenter = Vector3.zero;
            var otherObjectsList = SceneManager.GetSceneByName(scene.name).GetRootGameObjects();
            Debug.Log(sceneInfo.scene.Name);
            var thisObjectsList = SceneManager.GetSceneByName(sceneInfo.scene.Name).GetRootGameObjects();

            CompositeCollider2D? otherTilemapCollider = null;
            SceneLoader? sceneLoader = null;

            foreach (var x in otherObjectsList)
            {
                if (x.TryGetComponent(out sceneLoader))
                {
                    Debug.Log(sceneLoader.name);
                    otherTilemapCollider = sceneLoader.TilemapCollider;
                    Debug.Log(otherTilemapCollider.name);
                    break;
                }
            }
            if (sceneLoader == null)
            {
                Debug.LogError($"[SceneLoader.cs] There is no GameObject in the top level of the {scene.name} Hierarchy with a SceneLoader component.");
            }
            if (otherTilemapCollider == null)
            {
                Debug.LogError($"[SceneLoader.cs] The CompositeCollider2D of {scene.name} is not attached.");
            }

            // This Room's center based on the collider.    Top Left + half extents = center
            if (!tilemapCollider)
            {
                Debug.Log("HELLO I AM MISSING");
                return;
            }
            curRoomCenter = tilemapCollider.gameObject.transform.position + tilemapCollider.bounds.extents;
            var otherRoomCenter = otherTilemapCollider.bounds.center;
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

            Debug.DrawRay(Utils.Vector3ToVector3Int(curRoomCenter + doorPosition - otherDoorPosition - tilemapCollider.bounds.center), Vector3.up, Color.green, 10f);
            #endregion

            //-orc - (odp - orc) + dp + objPos + odp + (oDP - oRC)
            //-otherRoomCenter -(otherDoorPosition - otherRoomCenter) + (doorPosition)
            //var offset = -3 * otherRoomCenter + otherDoorPosition + doorPosition + tilemapCollider.transform.parent.parent.transform.position;
            //var offset = -otherRoomCenter - (otherDoorPosition - otherRoomCenter) + doorPosition + tilemapCollider.transform.parent.parent.transform.position + otherDoorPosition + (otherDoorPosition - otherRoomCenter);
            var offset = -otherRoomCenter - (otherDoorPosition - otherRoomCenter) + doorPosition + tilemapCollider.transform.parent.parent.transform.position;
            //Scene starts at 0,0,0
            //Debug.Log(tilemapCollider.transform.parent.parent.transform.position);
            // move every GO by the center of the current room + the current rooms door position - the other room door's position.
            Debug.Log($"{scene.name}: Other Center : {otherRoomCenter}, Other Door: {otherDoorPosition - otherRoomCenter}, This Door: {doorPosition}, This Pos: {tilemapCollider.transform.parent.parent.transform.position}");

            foreach (var x in otherObjectsList)
            {
                //x.transform.position += curRoomCenter + doorPosition;
                x.transform.position += Utils.Vector3ToVector3Int(offset);
            }
            if (boundingCollider)
            {
                Debug.Log("Found the bounding Collider");
                LevelManager.onEnterNewRoom?.Invoke(boundingCollider);
            }
        }
    }

    public void LoadScene(SceneInfo scene)
    {
        scene.isLoaded = true;
        StartCoroutine(LevelManager.LoadSceneAsync(scene.scene.Name));
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
