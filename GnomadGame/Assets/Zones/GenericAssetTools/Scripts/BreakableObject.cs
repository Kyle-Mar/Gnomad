using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEngine.ParticleSystem;

public class BreakableObject : WhackableObject
{
    [SerializeField] protected int HitsBeforeBreaking=1;
    [SerializeField] protected AudioClip BreakSound;
    [SerializeField] protected ParticleSystem BreakParticles;
    [SerializeField] protected bool PlayHitNoiseOnBreak;
    [SerializeField] SpriteRenderer sprite;
    // This should take a loot table, but they are not implemented yet
    // See DoBreak for use of this var
    //[SerializeField] protected GameObject LootTable;

    private void Start()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
            Assert.IsNotNull(sprite);
        }
        // https://forum.unity.com/threads/do-particle-systems-render-sprites-at-a-different-scale.896513/
        var main = BreakParticles.main;
        //main.startSize = new ParticleSystem.MinMaxCurve(sprite.sprite.rect.width / sprite.sprite.pixelsPerUnit);
        main.startSize = new ParticleSystem.MinMaxCurve(100f/282f);

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnterTarget");
        // Never calls the base function... maybe could cause issues if expanded
        //if (!collision.gameObject.CompareTag("PlayerAttack")) { return; }
        HitsBeforeBreaking--;
        if(HitsBeforeBreaking <= 0)
        {
            //prevents base from playing hit noise on the break hit
            if (BreakSound != null && !PlayHitNoiseOnBreak)
            {
                HitSound = null;
            }
            SpawnWhackEffects();
            DoBreak();
            return;
        }
        SpawnWhackEffects();

    }

    protected void DoBreak()
    {
        if (BreakParticles != null)
        {
            //ParticleSystem breakParticles = Instantiate(BreakParticles);
            //breakParticles.Play();
            foreach (SpriteRenderer s in sprites)
            {
                Destroy(s);
            }
            sprite.enabled = false;
            Destroy(GetComponent<GroundPoundBouncer>());
            BreakParticles.Play();
        }
        if (BreakSound != null)
        {
            AudioPlayer.PlayOneShot(BreakSound);
        }
        if (ActivationLink != null) { ActivationLink.Activate(); }
        //create loot from loot table
        Invoke("Die", 5);


    }

    protected void Die()
    {
        Destroy(gameObject);
    }
}
