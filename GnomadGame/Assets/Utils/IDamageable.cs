using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamageable
{
    public float MAX_HEALTH { get; }
    public float health { get; set; }
    public void Damage(float amount);
}
