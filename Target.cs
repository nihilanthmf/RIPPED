using UnityEngine;
using System;

public class Target : MonoBehaviour
{
    //[Header("The health not for an enemy")]
    //[SerializeField] float health = 10;
    float health = 1;

    Action<float> methodToExecute;

    ExplosiveBarrel explosiveBarrel;
    Crate destroyableObject;
    DefaultEnemyClass enemy;

    bool hasAlreadyBroken;

    private void Start()
    {
        if (GetComponent<ExplosiveBarrel>() != null)
        {
            explosiveBarrel = GetComponent<ExplosiveBarrel>();

            methodToExecute = Explode;
        }
        else if (tag == "Enemy")
        {
            enemy = GetComponent<DefaultEnemyClass>();

            methodToExecute = enemy.TakingDamage;
        }
        else if (GetComponent<Crate>() != null)
        {
            destroyableObject = GetComponent<Crate>();

            methodToExecute = DestroyableObjectAction;
        }
    }

    public void PerformAction(float damage)
    {
        methodToExecute(damage);
    }

    // Barrel
    void Explode(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            explosiveBarrel.Explode();
        }
    }

    void DestroyableObjectAction(float damage)
    {
        health -= damage;
    }

    private void Break()
    {
        if (health <= 0)
        {
            destroyableObject.OnHit();
        }
    }

    private void Update()
    {
        if (methodToExecute == DestroyableObjectAction && health <= 0 && !hasAlreadyBroken)
        {
            Break();
            hasAlreadyBroken = true;
        }
    }
}
