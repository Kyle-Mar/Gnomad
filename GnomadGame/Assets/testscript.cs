 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class testscript : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(GetComponent<CompositeCollider2D>().bounds.center);
    }
}
