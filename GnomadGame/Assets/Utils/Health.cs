using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public delegate void OnDeath();
    public OnDeath onDeath;

    #region Health Properties
    [Header("Health")] // I don't know why this doesn't show up.
    [SerializeField] const float MAX_HEALTH = 100f;
    [SerializeField] float health = 100f;
    [SerializeField] bool canTakeDamage = true;
    [SerializeField] float cooldownTime = 1.0f;
    #endregion

    #region Health Fields
    public float MaxHealth { get => MAX_HEALTH; }
    float IDamageable.Health { get => health; set => health = value; }
    public bool CanTakeDamage { get => canTakeDamage; set => canTakeDamage = value; }
    public float CooldownTime { get => cooldownTime; set => cooldownTime = value; }
    #endregion
    public virtual void Damage(float amount)
    {
        if (canTakeDamage)
        {
            HealthUtil.Damage(this, amount);
            StartCoroutine(DoCooldownTimer());
        }
    }
    
    void Awake()
    {
        health = MaxHealth;
    }

    public void Update()
    {
    }

    public virtual void Die()
    {
        if(onDeath.GetInvocationList().Length > 0)
        {
            onDeath?.Invoke();
            return;
        }
        Destroy(gameObject);
    }

    IEnumerator DoCooldownTimer()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(CooldownTime);
        canTakeDamage = true;
    }
}
