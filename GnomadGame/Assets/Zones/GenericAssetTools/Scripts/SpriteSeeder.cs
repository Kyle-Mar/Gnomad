#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[ExecuteInEditMode]
//editor script to randomize small asset locations while furnishing levels
public class SpriteSeeder : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] spritePool;
    [SerializeField] public uint index;
    [SerializeField] bool randomize;
    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnValidate()
    {//if an attribut has been changed, either randomize or set to new index
     //will also randomize when new sprites are added. No efficient way to stop this
        if (spritePool.Length == 0) { return; }
        if (randomize == true)
        {
            randomize = false;
            uint seed = (uint)UnityEngine.Random.Range(0, uint.MaxValue);
            spriteRenderer.sprite = spritePool[seed % spritePool.Length];
        }
        else
        {
            spriteRenderer.sprite = spritePool[index % spritePool.Length];
        }
    }


}
#endif