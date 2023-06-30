using UnityEngine;

public class Gibs : MonoBehaviour
{
    bool toStartDestroyingGibs;
    Rigidbody rb;
    [HideInInspector] public float kickForce = 3250;
    ParticleSystem bloodParticles;
    LayerMask playerMask, staticMask;
    float timeToSpawnDecalWhileFlying;

    Transform mainCamera;

    [HideInInspector] public GameObject[] bloodDecals;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bloodParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
        playerMask = LayerMask.GetMask("Player");
        staticMask = LayerMask.GetMask("Static");
        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        if (toStartDestroyingGibs)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0, 0, 0), 3 * Time.deltaTime);
        }

        if (rb.velocity.magnitude >= 0.01f)
        {
            LeavingDecalsWhileFlying();
        }
    }

    void LeavingDecalsWhileFlying()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5, staticMask) && Time.time >= timeToSpawnDecalWhileFlying)
        {
            GameObject randomDecal = bloodDecals[Random.Range(0, bloodDecals.Length)];
            GameObject currentDecal = Instantiate(randomDecal);
            currentDecal.SetActive(true);
            currentDecal.transform.position = hit.point + Vector3.up * 0.015f;
            currentDecal.transform.eulerAngles = hit.normal;

            currentDecal.transform.eulerAngles = new Vector3(90, Random.Range(0, 180), 0);
            currentDecal.transform.localScale = new Vector3(Random.Range(0.1f, 0.2f), Random.Range(0.1f, 0.2f), 1);

            timeToSpawnDecalWhileFlying = Time.time + 0.225f;
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 1, Vector3.up, out hit, 1, playerMask))
        {
            //rb.AddForce(new Vector3(hit.transform.forward.x, 0.6f, hit.transform.forward.z) * kickForce);
            //bloodParticles.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11)
        {
            GameObject randomDecal = bloodDecals[Random.Range(0, bloodDecals.Length)];
            GameObject currentDecal = Instantiate(randomDecal);
            currentDecal.SetActive(true);

            currentDecal.transform.localScale = new Vector3(Random.Range(0.25f, 0.4f), Random.Range(0.25f, 0.4f), 1);
            currentDecal.transform.rotation = Quaternion.LookRotation(collision.contacts[0].normal);

            float deltaToSubtractFromDecalPositionNotToStuckInWall = 0.1f;

            if (collision.contacts[0].normal.x < 0 || collision.contacts[0].normal.z < 0)
            {
                deltaToSubtractFromDecalPositionNotToStuckInWall = -0.01f;
            }

            currentDecal.transform.position =
                new Vector3(collision.contacts[0].point.x + deltaToSubtractFromDecalPositionNotToStuckInWall,
                collision.contacts[0].point.y + 0.1f, collision.contacts[0].point.z + deltaToSubtractFromDecalPositionNotToStuckInWall);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag == "Reaper Blade" || other.transform.gameObject.layer == 18)
        {
            rb.AddForce(new Vector3(other.transform.forward.x, 0.6f, other.transform.forward.z) * kickForce);
            bloodParticles.Play();
        }
        if (other.transform.gameObject.tag == "Godsmack Kill Area")
        {
            rb.AddForce(new Vector3(mainCamera.forward.x, 0.6f, mainCamera.forward.z) * kickForce);
            bloodParticles.Play();
        }
        if (other.transform.gameObject.tag == "Explosion")
        {
            rb.AddExplosionForce(kickForce * 2, other.transform.position, 5f, 1.25f);

            //Vector3 forceVector = new Vector3(transform.position.x - other.transform.position.x, 1, transform.position.z - other.transform.position.z).normalized;
            //forceVector.y = 1.1f;
            //rb.AddForce(forceVector * kickForce);

            bloodParticles.Play();
        }
    }
}
