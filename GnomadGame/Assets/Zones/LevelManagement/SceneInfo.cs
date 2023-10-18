using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eflatun.SceneReference;
using Gnomad.Utils;
using System;



/// <summary>
/// The GameObject this script / class is attatched to should
/// have the same name as the Scene Object you are trying to maintain.
/// </summary>



[CreateAssetMenu(menuName = "SceneInfo/New", order = 1)]
public class SceneInfo : ScriptableObject
{
    // For this to be put in the inspector it must be in the build settings.
    public SceneReference scene;
    public IDictionary<int, int> IntIntDictionary
    {
        get { return DoorConnections; }
        set { DoorConnections.CopyFrom(value); }
    }
    public List<SceneInfo> adjacentScenes;
    public bool isLoaded { get; set; }
    public List<Vector3> DoorPositions = new(); // this is related in order to the sceneInfo's adjacent scene list
    [Header("KEY: Idx Of Our Door,                                                  VALUE: Idx Of Adjacent Scene's door")]
    public IntIntDictionary DoorConnections = new IntIntDictionary{ { 0,0 } }; // Key = idx of our adjacent scenes, Value = other adjacent scenes idx.
}
