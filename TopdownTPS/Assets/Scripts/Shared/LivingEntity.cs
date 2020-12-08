using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamagable
{
    public float health;
    [HideInInspector] public float currentHealth { get; protected set; }
    protected bool dead;

    public event System.Action OnDeath;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        dead = false;
        currentHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        //DoStuff
        TakeDmg(damage);
    }

    public virtual void TakeDmg(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !dead)
        {
            currentHealth = 0;
            Die();
        }
    }
    [ContextMenu("SelfDestruct")]
    protected void Die()
    {
        dead = true;
        if(OnDeath != null)
        {
            OnDeath();
        }
        //Destroy(gameObject);
    }

}
