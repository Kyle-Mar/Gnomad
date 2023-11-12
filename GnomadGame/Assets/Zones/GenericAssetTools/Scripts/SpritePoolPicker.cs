#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class SpritePoolPicker : MonoBehaviour
{
    [Header("Self deleting script that randomly\n selects sprite from pool")]

    SpriteRenderer sprite;

    [SerializeField]
    [Tooltip("randomly choose between currently assigned sprite and these")]
    List<Sprite> alternateSprites;


    private void Randomize()
    {
        //randomize sprite
        alternateSprites.Add(sprite.sprite);
        sprite.sprite = alternateSprites[Random.Range(0, alternateSprites.Count)];

    }
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Randomize();
        DestroyImmediate(this);
    }

}
#endif