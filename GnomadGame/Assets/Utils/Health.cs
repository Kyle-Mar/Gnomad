using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public delegate void OnDeath();
    public OnDeath onDeath;
    public delegate void OnDamage(float amt, Collider2D collider, Vector3 dir);
    public OnDamage onDamage;
    public delegate void OnHeal();
    public OnDamage onHeal;
    public List<Collider2D> MyColliders = new();


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
    public virtual void Damage(float amount, Collider2D collider, Vector3 dir)
    {
        if (canTakeDamage)
        {
            if(collider == null)
            {
                return;
            }
            if (MyColliders.Contains(collider))
            {
                return;
            }
            HealthUtil.Damage(this, amount, collider, dir);
            StartCoroutine(DoCooldownTimer());
            if (onDamage.GetInvocationList().Length > 0)
            {
                if (dir != null)
                {
                    onDamage?.Invoke(amount,collider, dir);
                }
                else
                {
                    Debug.Log("I AM IN NO DIR");
                    onDamage.Invoke(amount, collider, Vector3.zero);
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
            onHeal?.Invoke(-amount, null, Vector3.zero);
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
