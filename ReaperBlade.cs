using UnityEngine;

public class ReaperBlade : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer, playerLayer, bouncableLayers;
    [SerializeField] ParticleSystem bloodParticles;
    [SerializeField] Reaper reaper;
    [SerializeField] GameObject trails;
    [SerializeField] Collider ricochetCollider;
    GameObject mainCamera;

    Collider projectileCollider;
    Rigidbody rb;

    Vector3 defaultPosition;

    [SerializeField] float returnForce = 47.5f;

    public float damage = 100;
    float startBladeDamage;

    float currentTimeInAir;
    float timeInAirConst = 1;
    [HideInInspector] public bool toChangeTime;
    [HideInInspector] public bool inTheAir;
    Vector3 startRotation;

    Vector3 forceVector;

    int startLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();
        defaultPosition = transform.localPosition;
        mainCamera = Camera.main.gameObject;

        projectileCollider.enabled = false;
        startRotation = transform.localEulerAngles;
        startLayer = gameObject.layer;
        startBladeDamage = damage;
    }

    private void Update()
    {
        if (currentTimeInAir >= timeInAirConst)
        {
            Return();
        }

        if (toChangeTime)
        {
            currentTimeInAir += Time.deltaTime;
        }

        if (inTheAir)
        {
            trails.SetActive(true);

            transform.Rotate(0, 0, 80f);
            trails.transform.Rotate(0, 0, -80f);
        }
        else
        {
            transform.localEulerAngles = startRotation;
            trails.SetActive(false);
        }

        Ricochet();
    }

    public void Shoot()
    {
        gameObject.layer = 9;
        rb.isKinematic = false;
        transform.parent = null;
        forceVector = mainCamera.transform.forward;
        rb.AddForce(forceVector * reaper.forwardForce, ForceMode.Impulse);

        projectileCollider.enabled = true;
        inTheAir = true;
    }

    void Ricochet()
    {
        if (inTheAir)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.right);
            if (Physics.Raycast(ray, out hit, 1f, LayerMask.GetMask("Static")))
            {
                Vector3 reflectDirection = Vector3.Reflect(ray.direction, hit.normal);

                forceVector = reflectDirection;
                rb.AddForce(forceVector * reaper.forwardForce, ForceMode.Impulse);
            }
        }
    }


    void Return()
    {
        rb.isKinematic = true;

        transform.position = Vector3.MoveTowards(transform.position, reaper.transform.position, returnForce * Time.deltaTime);

        if (Physics.CheckSphere(transform.position, 1f, playerLayer))
        {
            transform.parent = reaper.transform;
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = defaultPosition;
            projectileCollider.enabled = false;

            toChangeTime = false;
            currentTimeInAir = 0;

            inTheAir = false;

            gameObject.layer = startLayer;

            // to reset the alt fire power up
            damage = startBladeDamage;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //print(other.gameObject + " " + other.gameObject.layer + " " + gameObject.layer);
        if (other.transform.gameObject.layer == 6 || other.gameObject.layer == 20)
        {
            Physics.IgnoreCollision(ricochetCollider, other);

            Transform targetHolder = other.transform;
            Transform currentTargetHolder = other.transform;
            while (currentTargetHolder.parent != null)
            {
                if (currentTargetHolder.parent.gameObject.layer == 6 || currentTargetHolder.parent.gameObject.layer == 20)
                {
                    targetHolder = currentTargetHolder.parent;
                }
                currentTargetHolder = currentTargetHolder.parent;
            }

            Target target = targetHolder.gameObject.GetComponent<Target>();

            // If we hit an actual enemy
            if (targetHolder.GetComponent<DefaultEnemyClass>() != null)
            {
                DefaultEnemyClass defaultEnemyClass = targetHolder.GetComponent<DefaultEnemyClass>();
                defaultEnemyClass.toStartBleed = true;
                ParticleSystem bloodParticlesInstance = Instantiate(bloodParticles, other.ClosestPoint(transform.position), Quaternion.LookRotation(-transform.forward));
                bloodParticlesInstance.Play();
                Destroy(bloodParticlesInstance.gameObject, 1);

                if (other.transform != targetHolder)
                {
                    if (other.gameObject.tag == "Torso") // if its torso Torso
                    {
                        defaultEnemyClass.FullyGib(transform.position);
                    }
                    else if (other.gameObject.tag != "Inseparable Body Part")
                    {
                        Rigidbody gib = other.transform.GetChild(0).GetComponent<Rigidbody>();
                        Gibs gibScript = gib.GetComponent<Gibs>();
                        Transform blood = other.transform.GetChild(1);

                        gib.GetComponent<Gibs>().bloodDecals = defaultEnemyClass.decals;

                        gib.gameObject.SetActive(true); // Gib

                        blood.gameObject.SetActive(true);

                        blood.parent = targetHolder;
                        blood.localScale = new Vector3(1, 1, 1);

                        gib.transform.parent = null;
                        gib.AddExplosionForce(gibScript.kickForce, transform.position, 1);

                        other.gameObject.SetActive(false);

                        //defaultEnemyClass.decapitated = true;
                    }
                }                
            }

            if (other.gameObject.tag == "Head")
            {
                target.PerformAction(damage * 10);
            }
            else
            {
                target.PerformAction(damage);
            }
        }
    }
}
