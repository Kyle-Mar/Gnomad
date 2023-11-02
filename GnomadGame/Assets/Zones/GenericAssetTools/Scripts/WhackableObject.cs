using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WhackableObject : MonoBehaviour
{
    [SerializeField] protected Collider2D HitBox;
    [SerializeField] protected AudioClip HitSound;
    [SerializeField] protected ParticleSystem HitParticles;
    [SerializeField] protected bool IsBounceable;
    [SerializeField] protected SpriteRenderer[] sprites;
    [Tooltip("Activatable object upon hit")]
    [SerializeField] protected ActivatableObject ActivationLink;
    protected AudioSource AudioPlayer;

    private void Awake()
    {
        if (IsBounceable)
        {
            this.gameObject.AddComponent<GroundPoundBouncer>();
        }
        if (HitBox == null)
        {
            HitBox = GetComponent<Collider2D>();
        }
        Assert.IsNotNull(HitBox);

        AudioPlayer = this.gameObject.AddComponent<AudioSource>();
    }
    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("PlayerAttack")) {return;}
        SpawnWhackEffects();
        if(ActivationLink != null) { ActivationLink.Activate(); }
        Debug.Log("Object activated");
    }

    protected void SpawnWhackEffects()
    {
        if (HitParticles != null)
        {
            HitParticles.Play();
        }
        if (HitSound != null)
        {
            AudioPlayer.PlayOneShot(HitSound); 
        }
    }
}
