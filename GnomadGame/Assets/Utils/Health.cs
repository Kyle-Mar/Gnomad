using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public delegate void OnDeath();
    public OnDeath onDeath;
    public delegate void OnDamage(float amt, Vector3 dir);
    public OnDamage onDamage;
    public delegate void OnHeal();
    public OnDamage onHeal;

    #region Health Properties
    [Header("Health")] // I don't know why this doesn't show up.
    [SerializeField] float MAX_HEALTH = 100f; //had to take away const here to access in editor :P ~Elijah
    [SerializeField] public float health = 100f;//I had to access this from HUD. Let me know if there is a better w
    [SerializeField] bool canTakeDamage = true;
    [SerializeField] float cooldownTime = 1.0f;
    #endregion

    #region Health Fields
    public float MaxHealth { get => MAX_HEALTH; }
    float IDamageable.Health { get => health; set => health = value; }
    public bool CanTakeDamage { get => canTakeDamage; set => canTakeDamage = value; }
    public float CooldownTime { get => cooldownTime; set => cooldownTime = value; }
    #endregion
    public virtual void Damage(float amount, Vector3? dir = null)
    {
        if (canTakeDamage)
        {
            HealthUtil.Damage(this, amount);
            StartCoroutine(DoCooldownTimer());
            if (onDamage.GetInvocationList().Length > 0)
            {
                if (dir.HasValue)
                {
                    onDamage?.Invoke(amount, dir.Value);
                }
                else
                {
                    onDamage.Invoke(amount, Vector3.zero);
                }
                return;
            }
        }
    }

    public virtual void Heal(float amount)
    {
        HealthUtil.Heal(this, amount);

        if (onHeal.GetInvocationList().Length > 0)
        {
            onHeal?.Invoke(-amount, Vector3.zero);
            return;
        }
    }

    void Awake()
    {
        health = MaxHealth;
    }

    public void Update()
    {
        if (this.transform.tag == "Enemy" && health <= 0f)
        {
            Die();
        }
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
