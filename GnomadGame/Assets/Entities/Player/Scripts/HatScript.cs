using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatScript : MonoBehaviour
{

    [SerializeField] bool canDamage = false;

   

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

        // Maybe add a tag called "EnemyHitBox"
        // So we can be more specific in what we are colliding with
        // And potentially isolating our collisions in case we don't want
        // To collide with specific colliders on the enemy

        // Checks to see what tag the collider has
        if (collision.gameObject.tag == "Enemy")
        {
            // Tries to get the 'IDamageable' component off of the supposed 'Enemy'
            // If it gets it successfully, then it will cause damage to the 'Enemy'
            // If it can't, it will error
            if (collision.gameObject.TryGetComponent<IDamageable>(out damageable))
            {
                damageable.Damage(10f);
                Debug.Log("Damaging Enemy");
            }
        }
    }

}