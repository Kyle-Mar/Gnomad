#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class FoliageAttributeJitter : MonoBehaviour
{
    [Header("Self deleting script that randomly\n jitters color and size of sprites slightly")]

    SpriteRenderer sprite;

    [SerializeField]
    [Tooltip("randomly choose between currently assigned sprite and these")]
    List<Sprite> alternateSprites;

    [SerializeField]
    [Tooltip("Is this object intended to be flipped randomly?")]
    bool randomFlipX = true;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("Amount sprite hue can vary from default")]
    float hueJitterRange;
   
    [SerializeField][Range(0f,1f)]
    [Tooltip("How high can the saturation go? (White is the default sprite)")]
    float saturationJitterRange;

    [SerializeField][Range(0f,0.5f)]
    [Tooltip("Maxiumum sprite scale variance. Max is 1.5x larger/smaller")]
    float sizeJitterRange;

    private void Randomize()
    {
        //randomize sprite
        alternateSprites.Add(sprite.sprite);
        sprite.sprite = alternateSprites[Random.Range(0, alternateSprites.Count)];
        //randomize jitter ammount within range
        hueJitterRange = Random.Range(-hueJitterRange, hueJitterRange);
        saturationJitterRange = Random.Range(0f, saturationJitterRange);
        sizeJitterRange = Random.Range(1f-sizeJitterRange,1f+sizeJitterRange);

        //randomize attributes by jitter ammount
        sprite.color = Color.HSVToRGB((hueJitterRange + 1) / 2f, saturationJitterRange, 1);
        transform.localScale = new Vector3(sizeJitterRange, sizeJitterRange, 1);
        if (!randomFlipX) { return; }
        sprite.flipX = Random.value > 0.5f;
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