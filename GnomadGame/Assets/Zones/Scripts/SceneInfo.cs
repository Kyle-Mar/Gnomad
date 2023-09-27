using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



/// <summary>
/// The GameObject this script / class is attatched to should
/// have the same name as the Scene Object you are trying to maintain.
/// </summary>



[CreateAssetMenu(menuName = "SceneInfo/New", order = 1)]
public class SceneInfo : ScriptableObject
{
    public List<SceneInfo> adjacentScenes;
    public bool isLoaded { get; set; }
}
