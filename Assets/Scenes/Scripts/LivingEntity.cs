using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Inherits from MonoBehaviour and IDamagable.
public class LivingEntity : MonoBehaviour, IDamagable
{
    public float startingHealth;
    //Accessible from this class, and derived class instances (enemy, player)
    protected float health;
    protected bool dead;

    //Delegate event that listeners can pay attention to.
    public event System.Action OnDeath;

    //protected start method that can be accessed and overridden by player and enemy classes
    protected virtual void Start()
    {
        health = startingHealth;
    }

    //TakeHit function that applies to player and enemy classes.
    public void TakeHit(float damage, RaycastHit hit)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        //if no health is left, call die method
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    //protected, accessible to this class and derived class instances. If OnDeath hasn't been called yet, call it, signalling listeners to trigger their methods.
    protected void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        //desotry this object
        GameObject.Destroy(gameObject);
    }
}
