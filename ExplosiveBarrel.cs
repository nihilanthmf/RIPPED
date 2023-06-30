using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    Collider explosionCollider;
    [SerializeField] PlayerMovement playerMovement;
    CameraController cameraController;
    [SerializeField] ParticleSystem explosionParticles;

    Vector3 forceVector;

    float maxDamage = 20;

    float initialDamage = 100;

    [SerializeField] GameObject toxicPuddle;

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        explosionCollider = GetComponent<SphereCollider>();
    }

    bool alreadyDestroyed;
    public void Explode()
    {
        if (!alreadyDestroyed)
        {
            explosionCollider.enabled = true;
            explosionParticles.transform.parent = null;
            explosionParticles.Play();

            if (Vector3.Distance(transform.position, playerMovement.transform.position) <= 25)
            {
                StartCoroutine(cameraController.Shaking(.3f, 1, gameObject));
            }

            if (toxicPuddle != null)
            {
                toxicPuddle.SetActive(true);
                toxicPuddle.transform.parent = null;
            }

            Destroy(gameObject, 0.2f);
            Destroy(explosionParticles.gameObject, 0.75f);
            Destroy(toxicPuddle, 25);
            alreadyDestroyed = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        forceVector = (other.transform.position - transform.position) / Vector3.Distance(transform.position, other.transform.position);

        Transform targetHolder = other.transform;
        Transform currentTargetHolder = other.transform;

        if (other.gameObject.layer == 6)
        {
            while (currentTargetHolder.parent != null)
            {
                if (currentTargetHolder.parent.gameObject.layer == 6)
                {
                    targetHolder = currentTargetHolder.parent;
                }
                currentTargetHolder = currentTargetHolder.parent;
            }
        }

        if (targetHolder.tag == "Enemy")
        {
            targetHolder.GetComponent<Target>().PerformAction(maxDamage / Vector3.Distance(transform.position, other.transform.position) * 1000);
            DefaultEnemyClass defaultEnemyClass = targetHolder.GetComponent<DefaultEnemyClass>();
            if (defaultEnemyClass.health <= 0)
            {
                defaultEnemyClass.FullyGib(transform.position);
            }
        }
        else if (other.tag == "Player")
        {
            playerMovement.ExplosionJump(forceVector + Vector3.up * playerMovement.verticalExplosionForceMultiplier);
            other.gameObject.GetComponent<PlayerController>().TakingDamage(initialDamage / Vector3.Distance(transform.position, other.transform.position));
        }
    }
}
