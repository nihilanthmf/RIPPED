using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class DefaultEnemyClass : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;

    public bool isDead { get; private set; }
    public bool toStartBleed;
    public bool bodySplited;

    public float health;

    Vector3 gidForceVector;

    [SerializeField] GameObject[] bloodOnFloor;
    [SerializeField] Rigidbody[] gibs;
    GameObject[] bodyParts, bloodOfBodyParts;
    Collider[] bodyPartColliders;
    Gibs[] gibsScript;

    GameObject head;
    GameObject headBlood;

    [SerializeField] LayerMask floorLayer;
    public GameObject mesh;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform headBloodParent;
    [SerializeField] AnimationClip takingDamageAnimation;
    [SerializeField] Rigidbody[] headParts;
    EnemyTrigger enemyTrigger;

    public GameObject[] decals;
    [SerializeField] LayerMask toLeaveDecalOn;
    NavMeshAgent navMeshAgent;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyTrigger = GetComponent<EnemyPathFinder>().triggerToActivate;
        navMeshAgent = GetComponent<NavMeshAgent>();

        bodyParts = new GameObject[gibs.Length];
        bloodOfBodyParts = new GameObject[gibs.Length];
        bodyPartColliders = new Collider[gibs.Length];
        gibsScript = new Gibs[gibs.Length];

        for (int i = 0; i < gibs.Length; i++)
        {
            bodyParts[i] = gibs[i].transform.parent.gameObject;
            bloodOfBodyParts[i] = bodyParts[i].transform.GetChild(1).gameObject;
            bodyPartColliders[i] = bodyParts[i].GetComponent<Collider>();
            gibsScript[i] = gibs[i].GetComponent<Gibs>();
            gibsScript[i].bloodDecals = decals;

            if (bodyParts[i].tag == "Head")
            {
                head = bodyParts[i].gameObject;
                headBlood = bloodOfBodyParts[i];
            }
        }
    }

    public void BlowingHead()
    {
        GameObject headBlood = head.transform.GetChild(1).gameObject;
        Vector3 headBloodScale = headBlood.transform.localScale;
        headBlood.transform.parent = headBloodParent;
        headBlood.transform.localScale = headBloodScale;
        headBlood.SetActive(true);
        foreach (var headPart in headParts)
        {
            headPart.gameObject.SetActive(true);
            headPart.AddExplosionForce(100f, headPart.transform.parent.position, 1f);
            headPart.transform.parent = null;
        }
    }

    public void TakingDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !isDead)
        {
            isDead = true;
        }
        if (!head.activeSelf)
        {
            headBlood.transform.parent = headBloodParent;
            headBlood.transform.localScale = new Vector3(1, 1, 1);
        }

        LeavingDecals();

        Bleeding();
        animator.SetBool("ToTakeDamage", true);
        StartCoroutine(WaitToStopTakingDamage());

        enemyTrigger.hasBeenEntered = true;
    }

    void Bleeding()
    {
        if (toStartBleed)
        {
            RaycastHit hit;
            if (Physics.Raycast(groundCheck.transform.position, -Vector3.up, out hit, 1000, floorLayer))
            {
                Instantiate(bloodOnFloor[Random.Range(0, bloodOnFloor.Length - 1)], hit.point + Vector3.up * 0.025f, Quaternion.LookRotation(hit.normal)).SetActive(true);
            }

            toStartBleed = false;
        }
    }

    void LeavingDecals()
    {
        GameObject randomDecal = decals[Random.Range(0, decals.Length)];

        // To leave decals on the ground beneath the enemy
        RaycastHit hitFloor;
        if (Physics.Raycast(transform.position, -Vector3.up, out hitFloor, 1000, toLeaveDecalOn))
        {
            GameObject currentDecal = Instantiate(randomDecal);
            currentDecal.SetActive(true);
            currentDecal.transform.position = hitFloor.point + Vector3.up * 0.015f + // not to be on the same level as ground
                new Vector3(Random.Range(0, 1), 0, Random.Range(0, 1));
            currentDecal.transform.eulerAngles = new Vector3(90, Random.Range(0, 180), 0);

            float randomXYScaleNumber = Random.Range(0.5f, 0.75f); // Random.Range(0.675f, 1)
            currentDecal.transform.localScale = new Vector3(randomXYScaleNumber, randomXYScaleNumber, 1); 
        }
        // To leave decals on the wall that the enemy is next to
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, 3.5f, toLeaveDecalOn))
        {
            GameObject currentDecal = Instantiate(randomDecal, hit.point, Quaternion.LookRotation(hit.normal));

            float deltaToSubtractFromDecalPositionNotToStuckInWall = 0.01f;

            if (hit.normal.x < 0 || hit.normal.z < 0)
            {
                deltaToSubtractFromDecalPositionNotToStuckInWall = -0.01f;
            }

            currentDecal.transform.position =
                new Vector3(currentDecal.transform.position.x + deltaToSubtractFromDecalPositionNotToStuckInWall,
                currentDecal.transform.position.y, currentDecal.transform.position.z + deltaToSubtractFromDecalPositionNotToStuckInWall);

            float randomRangeForDecal = 45;

            currentDecal.transform.position +=
                new Vector3(Mathf.Abs(hit.normal.z) * deltaToSubtractFromDecalPositionNotToStuckInWall * Random.Range(-randomRangeForDecal, randomRangeForDecal),
                Random.Range(-randomRangeForDecal * 0.01f, randomRangeForDecal * 0.01f),
                Mathf.Abs(hit.normal.x) * deltaToSubtractFromDecalPositionNotToStuckInWall * Random.Range(-randomRangeForDecal, randomRangeForDecal));

            float randomXYScaleNumber = Random.Range(0.5f, 0.75f); // Random.Range(0.675f, 1)
            currentDecal.transform.localScale = new Vector3(randomXYScaleNumber, randomXYScaleNumber, 1);

            currentDecal.SetActive(true);
        }
    }

    public void FullyGib(Vector3 explosionPosition)
    {
        for (int i = 0; i < gibs.Length; i++)
        {
            if (bodyParts[i].activeSelf)
            {
                gibs[i].transform.parent = null;
                gibs[i].gameObject.SetActive(true);
                bodyParts[i].SetActive(false);

                gidForceVector = new Vector3(Random.Range(-1.1f, 1.1f), 0.4f, Random.Range(-1.1f, 1.1f));
                gibs[i].gameObject.SetActive(true);
                //gibs[i].AddExplosionForce(gidForceVector * gibForce);
                gibs[i].AddExplosionForce(gibsScript[i].kickForce, transform.position, 1.5f);
            }
        }

        for (int i = 0; i < bloodOfBodyParts.Length; i++)
        {
            bloodOfBodyParts[i].SetActive(false);
        }
        DeathDefault();
    }

    void Death()
    {
        DeathDefault();

        for (int i = 0; i < bodyPartColliders.Length; i++)
        {
            bodyPartColliders[i].enabled = false;
        }

        animator.SetBool("ToTakeDamage", false);
        animator.SetBool("ToDie", true);
    }

    void DeathDefault()
    {
        Destroy(navMeshAgent);

        health = 0;
        isDead = true;
        rb.useGravity = false;

        //mesh.SetActive(false);

        Destroy(this);
    }


    IEnumerator WaitToStopTakingDamage()
    {
        yield return new WaitForSeconds(takingDamageAnimation.length - 0.325f);

        animator.SetBool("ToTakeDamage", false);
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            Death();
        }
    }
}
