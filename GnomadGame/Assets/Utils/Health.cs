using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public float MAX_HEALTH { get; set; } = 100f;
    public float health { get; set; }
    public float damageCooldown = 1.0f;
    public bool canTakeDamage = true;
    public virtual void Damage(float amount)
    {
        if (!canTakeDamage)
        {
            return;
        }
        StartCoroutine(DoCooldownTimer());

        health -= amount;
        //Debug.Log(health);
        if (health <= 0)
        {
            Die();
        }
    }
    
    void Awake()
    {
        health = MAX_HEALTH;
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    IEnumerator DoCooldownTimer()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
}
