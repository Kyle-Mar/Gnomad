using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Assertions;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] Collider2D hitBox;
    //[SerializeField] ParticleSystem breakEffects;
    [SerializeField] ParticleSystem hitEffects;
    [SerializeField] AudioSource soundEffect;
 
    private void Awake()
    {
        if (hitBox == null)
        {
            hitBox = GetComponent<Collider2D>();
        }
        Assert.IsNotNull(hitBox);
        if (soundEffect == null)
        {
            soundEffect = GetComponent<AudioSource>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerAttack")
        {
            if (hitEffects != null)
            {
                ParticleSystem BreakParticles = Object.Instantiate(hitEffects, transform.position, Quaternion.identity);
                //drop any loot
            }
            soundEffect.Play();
            Destroy(gameObject);
        }
    }
}
