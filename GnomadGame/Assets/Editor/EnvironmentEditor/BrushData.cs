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
}
