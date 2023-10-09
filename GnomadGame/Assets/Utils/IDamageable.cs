using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float MaxHealth { get; }
    public float Health { get; set; }
    public bool CanTakeDamage { get; set; }

    public void Damage(float amount)
    {
        HealthUtil.Damage(this, amount);
    }
    public void Die();
}

public static class HealthUtil
{
    public static void Damage(this IDamageable dmg, float amount)
    {
        if (!dmg.CanTakeDamage)
        {
            return;
        }
        dmg.Health -= amount;
        if (dmg.Health <= 0)
        {
            dmg.Die();
        }
    }

    public static void Die(this IDamageable dmg)
    {
        
    }
}


