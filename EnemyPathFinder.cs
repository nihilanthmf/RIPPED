using UnityEngine;
using UnityEngine.AI;

public class EnemyPathFinder : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] Transform destinationPlayer;
    [SerializeField] PlayerController playerController;
    public EnemyTrigger triggerToActivate;
    DefaultEnemyClass defaultEnemy;
    Animator animator;

    float startSpeed;
    float startAcceleration;

    bool hasActivated;

    [SerializeField] float activationDistance;

    public bool isThere { get; private set; }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        defaultEnemy = GetComponent<DefaultEnemyClass>();
        animator = GetComponent<Animator>();

        startSpeed = agent.speed;
        startAcceleration = agent.acceleration;
    }

    private void Move()
    {
        transform.LookAt(playerController.transform.position);
        if (!playerController.dead)
        {
            agent.SetDestination(destinationPlayer.position);
        }

        if (Vector3.Distance(transform.position, agent.destination) <= 2)
        {
            isThere = true;
            agent.speed = 0;
        }
        else
        {
            isThere = false;
            agent.speed = startSpeed;
        }
    }

    void StopWalking() // via Animator
    {
        if (agent)
        {
            agent.acceleration = int.MaxValue;
            agent.speed = 0;
            agent.isStopped = true;
        }
    }

    void ResumeWalking()
    {
        if (agent)
        {
            agent.isStopped = false;
            agent.acceleration = startAcceleration;
            agent.speed = startSpeed;
        }
    }

    private void Update()
    {
        if (!defaultEnemy.isDead)
        {
            if (triggerToActivate.hasBeenEntered)
            {
                animator.SetBool("ToIdle", false);
                agent.isStopped = false;
                Move();
                hasActivated = true;
            }
            else
            {
                if (!hasActivated)
                {
                    agent.isStopped = true;
                    animator.SetBool("ToIdle", true);
                }
            }
        }
    }
}
