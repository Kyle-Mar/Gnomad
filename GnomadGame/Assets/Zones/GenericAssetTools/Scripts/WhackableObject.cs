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
    }

    protected void SpawnWhackEffects()
    {
        if (HitParticles != null)
        {
            Instantiate(HitParticles, transform.position, Quaternion.identity);
        }
        if (HitSound != null)
        {
            AudioPlayer.PlayOneShot(HitSound); 
        }
    }
}
