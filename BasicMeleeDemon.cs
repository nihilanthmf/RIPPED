using UnityEngine;
using System.Collections;

public class BasicMeleeDemon : MonoBehaviour
{
    [SerializeField] float damage;
    bool hasCooledDown = true;

    [SerializeField] PlayerController player;

    EnemyPathFinder pathFinder;

    Animator animator;

    private void Start()
    {
        pathFinder = GetComponent<EnemyPathFinder>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (pathFinder.isThere && hasCooledDown && !player.dead)
        {
            Attack();
        }
    }

    void DecreasePlayersHealth()
    {
        if (pathFinder.isThere)
        {
            player.TakingDamage(damage);
        }
    }

    IEnumerator WaitFewSecondsBeforeNextHit()
    {
        hasCooledDown = false;
        yield return new WaitForSeconds(0); 
        hasCooledDown = true;

        animator.SetBool("ToAttack", false);
    }

    void Attack()
    {
        animator.SetBool("ToAttack", true);
        StartCoroutine(WaitFewSecondsBeforeNextHit());
    }
}
