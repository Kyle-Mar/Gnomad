using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatScript : MonoBehaviour
{

    public PlayerStateMachine psm;

    bool sliding = false;

    private void Start()
    {
        if (this.name == "Slide Collider")
        {
            Debug.LogWarning("Sliding");
            sliding = true;
        }
    }

    /// <summary>
    /// This is the Player Hurt Box
    /// When the player does an attack (Slash, Ground Pound, etc)
    /// A Collider2D will be enabled and look for collisions
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Part of the Health System (In Utils Folder)
        // Set to null initially
        // If there is a collision between an object, like an enemy
        // It will try and get its 'IDamageable' component and 
        // set this 'damageable' variable to it
        IDamageable damageable = null;
        IKnockable knockable = null;
        Vector3 KBVector = Vector3.zero;
        // Maybe add a tag called "EnemyHitBox"
        // So we can be more specific in what we are colliding with
        // And potentially isolating our collisions in case we don't want
        // To collide with specific colliders on the enemy

        // Checks to see what tag the collider has
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Breakable")
        {
            // Tries to get the 'IDamageable' component off of the supposed 'Enemy'
            // If it gets it successfully, then it will cause damage to the 'Enemy'
            // If it can't, it will error
            if (collision.gameObject.TryGetComponent<IDamageable>(out damageable))
            {
                var collisionPoint = collision.ClosestPoint(transform.position);
                
                

                if (sliding)
                {
                    if (collision.gameObject.tag == "Enemy")
                    {
                        collision.gameObject.GetComponentInChildren<EnemyStateMachine>().IsSlidedInto = true;
                        //Debug.LogWarning("Slide True");
                    }

                    if (collisionPoint.x - transform.position.x < 0)
                    {
                        //Debug.Log("Sliding Left");
                        damageable.Damage(MovementStats.baseSlashDamage, psm.SlideCollider, new Vector3(0.3f * 1f, 4.0f));
                        KBVector = new Vector3(0.165f * 1f, 1.35f);
                    }
                    else
                    {
                        //Debug.Log("Sliding Right");
                        damageable.Damage(MovementStats.baseSlashDamage, psm.SlideCollider, new Vector3(0.3f * -1f, 4.0f));
                        KBVector = new Vector3(0.165f * -1f, 1.35f);
                    }
                }
                else
                {
                    damageable.Damage(MovementStats.baseSlashDamage, psm.SlashCollider, new Vector3(collisionPoint.x - transform.position.x, 0.5f));
                    KBVector = new Vector3(collisionPoint.x - transform.position.x, 0.5f).normalized;
                    //Debug.Log(KBVector.x);
                }
                if (collision.gameObject.TryGetComponent<IKnockable>(out knockable))
                {
                    knockable.Knockback(KBVector);
                }
                //Debug.Log("Damaging Enemy");
            }
        }
    }

}
