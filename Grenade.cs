using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] Transform player;
    PlayerMovement playerMovement;
    PlayerController playerController;
    ParticleSystem blowupParticles;
    CameraController cameraController;

    [SerializeField] Collider explosionColldier;

    Vector3 playerForceDirection;

    const float maxDamage = 125;

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerController = player.GetComponent<PlayerController>();
        blowupParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    public void BlowUpVisuals()
    {
        blowupParticles.gameObject.SetActive(true);
        blowupParticles.transform.SetParent(null);
    }

    public void BlowUp()
    {
        BlowUpVisuals();
        if (Vector3.Distance(transform.position, playerMovement.transform.position) <= 25)
        {
            StartCoroutine(cameraController.Shaking(.3f, 1, gameObject));
        }
        explosionColldier.enabled = true;
        Destroy(gameObject, 0.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 20)
        {
            BlowUp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        playerForceDirection = (other.transform.position - transform.position + Vector3.up * playerMovement.verticalExplosionForceMultiplier) 
                / Vector3.Distance(transform.position, other.transform.position);

        if (other.gameObject.layer == 6 || other.gameObject.layer == 20)
        {
            // The While is used to get a parent with Target in case we hit its child
            Transform targetHolder = other.transform;
            Transform currentTargetHolder = other.transform;
            while (currentTargetHolder.parent != null)
            {
                if (currentTargetHolder.parent.gameObject.GetComponent<Target>()) /*(currentTargetHolder.parent.gameObject.layer == 6)*/
                {
                    targetHolder = currentTargetHolder.parent;
                    break;
                }
                currentTargetHolder = currentTargetHolder.parent;
            }

            targetHolder.GetComponent<Target>().PerformAction(maxDamage / Vector3.Distance(transform.position, other.transform.position));

            // To Fully Gib an enemy if we hit one
            if (targetHolder.tag == "Enemy")
            {
                DefaultEnemyClass defaultEnemyClass = targetHolder.GetComponent<DefaultEnemyClass>();
                if (defaultEnemyClass.health <= 0)
                {
                    defaultEnemyClass.FullyGib(transform.position);
                }
            }
        }
        else if (other.tag == "Player")
        {
            playerMovement.ExplosionJump(playerForceDirection);
            playerController.TakingDamage(maxDamage / Vector3.Distance(transform.position, other.transform.position));
        }
    }
}
