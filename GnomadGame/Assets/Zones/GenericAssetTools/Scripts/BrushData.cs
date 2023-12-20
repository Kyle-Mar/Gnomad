using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EBrush", menuName = "EnvironmentBrushData", order = 1)]

public class BrushData: ScriptableObject
{
    [SerializeField]
    [Tooltip("Is this object intended to be flipped randomly?")]
    public bool RandomFlipX = true;

    [SerializeField]
    [Tooltip("Is this object intended to be flipped randomly?")]
    public bool RandomFlipY = true;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("Amount sprite hue can vary from default")]
    public float HueJitterRange;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("How high can the saturation go? (White is the default sprite)")]
    public float SaturationJitterRange;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("How high can the saturation go? (White is the default sprite)")]
    public float BrightnessJitterRange;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("Maxiumum sprite scale variance. Max is 1.5x larger/smaller")]
    public float ScaleJitterRange;

    //[SerializeField]
    //[Range(0f, 1f)]
    //[Tooltip("Maximum random offset of painted objects")]
    //public float ScatterRange;

    //[SerializeField]
    //[Range(0f, 1f)]
    //[Tooltip("")]
    //public float Spacing;

    [SerializeField]
    [Range(0f, 360f)]
    [Tooltip("Maxiumum sprite rotation variance. Max full rotation")]
    public float RotationJitterRange;

    public void Init(
        bool randomFlipX, bool randomFlipY,
        float hueJitterRange, float brightnessJitterRange, float saturationJitterRange,
        float scaleJitterRange,float rotationJitterRange
        )
    {
        RandomFlipX = randomFlipX;
        RandomFlipY = randomFlipY;
        HueJitterRange = hueJitterRange;
        BrightnessJitterRange = brightnessJitterRange;
        SaturationJitterRange = saturationJitterRange;
        ScaleJitterRange = scaleJitterRange;
        RotationJitterRange = rotationJitterRange;
    }

    public BrushData Copy()
    {
        BrushData copy = ScriptableObject.CreateInstance<BrushData>();
        copy.Init(RandomFlipX, RandomFlipY,
        HueJitterRange, BrightnessJitterRange, SaturationJitterRange,
        ScaleJitterRange, RotationJitterRange);
        return copy;
    }

    public static void ApplyBrushToObject(BrushData bd, GameObject o)
    {

        float hueJitterRange = UnityEngine.Random.Range(-bd.HueJitterRange, bd.HueJitterRange);
        float saturationJitterRange = UnityEngine.Random.Range(0, bd.SaturationJitterRange);
        float brightnessJitterRange = UnityEngine.Random.Range(0, bd.BrightnessJitterRange);

        float scaleJitterRange = UnityEngine.Random.Range(1f - (bd.ScaleJitterRange * 0.15f), 1f + (bd.ScaleJitterRange * 0.15f));

        float rotationJitterRange = UnityEngine.Random.Range(-bd.RotationJitterRange, bd.RotationJitterRange);
        //float positionOffset = UnityEngine.Random.Range(0, bd.scatterRange);
        if (false)
        {
            Debug.Log("Scale Jitter Range: " + scaleJitterRange);
            Debug.Log("Rotation Jitter Range: " + rotationJitterRange);
            Debug.Log("Saturation Jitter Range: " + saturationJitterRange);
            Debug.Log("Brightness Jitter Range: " + brightnessJitterRange);
            Debug.Log("Hue Final Value: " + (hueJitterRange + 1) / 2f);
        }

        //randomize attributes by jitter ammount
        List<SpriteRenderer> sprites = new List<SpriteRenderer>();
        SpriteRenderer sprite = o.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprites.Add(sprite);
        }

        //in case of compound objects, add all sprites
        sprites.AddRange(o.GetComponentsInChildren<SpriteRenderer>());
        foreach (SpriteRenderer s in sprites)
        {
            s.color = Color.HSVToRGB((hueJitterRange + 1) / 2f, saturationJitterRange, 1 - brightnessJitterRange);
        }
        o.transform.localScale = new Vector3(scaleJitterRange, scaleJitterRange, 1);
        o.transform.RotateAroundLocal(Vector3.back, rotationJitterRange * Mathf.Deg2Rad);
        // o.transform.position += positionOffset;

        //instead of flipping the sprite, flip the whole object.
        //That way we account for any child sprites in compound objects
        if (bd.RandomFlipX)
        {
            //sprite.flipX = UnityEngine.Random.value > 0.5f;
            Vector3 ls = o.transform.transform.localScale;
            ls = new Vector3(ls.x * UnityEngine.Random.Range(0, 2) * 2 - 1, ls.y, ls.z);

        }
        if (bd.RandomFlipY)
        {
            //sprite.flipY = UnityEngine.Random.value > 0.5f;
            Vector3 ls = o.transform.transform.localScale;
            ls = new Vector3(ls.x, ls.y * UnityEngine.Random.Range(0, 2) * 2 - 1, ls.z);
        }
    }
}

