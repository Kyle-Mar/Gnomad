using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BreakableObject : WhackableObject
{
    [SerializeField] protected uint HitsBeforeBreaking=1;
    [SerializeField] protected AudioClip BreakSound;
    [SerializeField] protected ParticleSystem BreakParticles;
    [SerializeField] protected bool PlayHitNoiseOnBreak;

    // This should take a loot table, but they are not implemented yet
    // See DoBreak for use of this var
    //[SerializeField] protected GameObject LootTable;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // Never calls the base function... maybe could cause issues if expanded
        if (!collision.gameObject.CompareTag("PlayerAttack")) { return; }
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
            Instantiate(BreakParticles, transform.position, Quaternion.identity);
        }
        if (BreakSound != null)
        {
            AudioPlayer.PlayOneShot(BreakSound);
        }

        //create loot from loot table

        Destroy(gameObject);

    }
}
