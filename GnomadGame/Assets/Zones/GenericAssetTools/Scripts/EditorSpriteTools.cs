#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditorSpriteTools : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] sprites;
    [Range(-1f, 1f)] public float HueOffset=0f;
    [Range(0f, 1f)] public float Saturation=0.1f;
    private bool alreadyInitialized;
    [SerializeField] bool randomizeColor;

    private void OnEnable()
    {
        
        sprites = GetComponents<SpriteRenderer>();

    }
    private void Awake()
    {
        alreadyInitialized=true;
        randomizeColor = true;
        OnValidate();

    }
    private void OnValidate()
    {//randomize on creation and when randomize button is clicked
        if (Application.isPlaying) return;
        if (!alreadyInitialized || randomizeColor)
        {
            randomize();
        }

        //update sprites with current values
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.color = Color.HSVToRGB((HueOffset+1)/2f, Saturation, 1);
        }
    }
    private void randomize()
    {
        randomizeColor = false;
        HueOffset = Random.Range(-1f, 1f);
        Saturation = Random.Range(0f, 0.3f);
    }
}
#endif