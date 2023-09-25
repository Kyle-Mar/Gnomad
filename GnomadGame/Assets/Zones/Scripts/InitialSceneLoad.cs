using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSceneLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LevelManager.LoadSceneAsync("TestScene1"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
