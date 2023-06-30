using System.Collections;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject fireEffect;
    [SerializeField] GameObject player;
    [SerializeField] GameObject slidingShit;
    [SerializeField] GameObject bulletImpact;

    [Header("Transforms")]
    Transform enemiesHead, headBlood; // to blow the head off

    [Header("Other Stuff")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] ParticleSystem smokeParticles;
    [SerializeField] ParticleSystem bloodParticles;
    [SerializeField] PlayerUI playerUI;
    Ray[] bulletsRays = new Ray[12];

    [HideInInspector] public WeaponDefault weaponDefault;

    public Animator animator { get; private set; }

    [SerializeField] int damage = 4;
    [HideInInspector] public int ammo;
    public int maxAmmo { get; private set; } = 100;

    float ySpreadMultiplier = 0.3f; // bigger this value is, bigger the Y spread is
    float spread = 0.2f;
    float numberOfBulletsInHead;
    const float numberOfBulletsInHeadToBlowHeadOff = 3;

    bool toMakeEnemyBleedOnce;

    private void Start()
    {
        animator = GetComponent<Animator>();
        weaponDefault = GetComponent<WeaponDefault>();
    }

    // All the visual shooting effects must be called here
    IEnumerator ShootingVisual()
    {
        smokeParticles.Play();
        fireEffect.SetActive(true);

        yield return new WaitForSeconds(0.025f);
        fireEffect.SetActive(false);    

        //StartCoroutine(mainCamera.GetComponent<CameraController>().Shaking(0.25f));
        string animationToPlayName = animator.GetBool("toAim") ? "AimShot" : "Shot";
        animator.Play(animationToPlayName);
    }

    void Shooting()
    {
        ammo--;
        toMakeEnemyBleedOnce = true;
        RaycastHit hitPoint;
        Target target = null;

        // Checking for an enemy hit by one of the rays
        for (int i = 0; i < bulletsRays.Length; i++)
        {
            Vector3 rayDirection = mainCamera.transform.forward +
                        transform.InverseTransformVector(UnityEngine.Random.Range(-spread, spread), UnityEngine.Random.Range(-spread * ySpreadMultiplier, spread * ySpreadMultiplier), 0);

            bulletsRays[i] = new Ray(mainCamera.transform.position, rayDirection);

            if (Physics.Raycast(bulletsRays[i], out hitPoint, 200, targetLayer))
            {
                if (hitPoint.transform.gameObject.layer == 6 || hitPoint.transform.gameObject.layer == 20)
                {
                    target = hitPoint.transform.gameObject.GetComponent<Target>();

                    // If we hit an actual enemy
                    if (hitPoint.transform.GetComponent<DefaultEnemyClass>() != null)
                    {
                        if (toMakeEnemyBleedOnce)
                        {
                            hitPoint.transform.GetComponent<DefaultEnemyClass>().toStartBleed = true;
                            toMakeEnemyBleedOnce = false;
                        }
                        ParticleSystem bloodParticlesInstance = Instantiate(bloodParticles, hitPoint.point, Quaternion.LookRotation(-mainCamera.transform.forward));
                        bloodParticlesInstance.Play();
                        Destroy(bloodParticlesInstance.gameObject, 1);
                    }
                    if (hitPoint.collider.gameObject.tag == "Head")
                    {
                        target.PerformAction(damage * 3);

                        numberOfBulletsInHead++;
                        enemiesHead = hitPoint.collider.transform;
                        headBlood = hitPoint.collider.transform.GetChild(1);
                    }
                    else
                    {
                        target.PerformAction(damage);
                    }
                }
                else
                {
                    GameObject bulletImpactExample = Instantiate(bulletImpact, hitPoint.point, Quaternion.LookRotation(hitPoint.normal));
                    float deltaToSubtractFromDecalPositionNotToStuckInWall = 0.01f;
                    if (hitPoint.normal.x < 0 || hitPoint.normal.y < 0 || hitPoint.normal.z < 0)
                    {
                        deltaToSubtractFromDecalPositionNotToStuckInWall = -0.01f;
                    }

                    bulletImpactExample.transform.position =
                        new Vector3(bulletImpactExample.transform.position.x + deltaToSubtractFromDecalPositionNotToStuckInWall,
                        bulletImpactExample.transform.position.y + deltaToSubtractFromDecalPositionNotToStuckInWall, bulletImpactExample.transform.position.z + deltaToSubtractFromDecalPositionNotToStuckInWall);
                    Destroy(bulletImpactExample, 3f);
                }
            }
        }

        if (numberOfBulletsInHead >= numberOfBulletsInHeadToBlowHeadOff)
        {
            enemiesHead.gameObject.SetActive(false);
            headBlood.gameObject.SetActive(true);

            target.PerformAction(10000); // To kill
        }
        numberOfBulletsInHead = 0;

        StartCoroutine(ShootingVisual());
    }

    void Aiming()
    {
        animator.SetBool("toAim", true);
    }

    private void Update()
    {
        ammo = Mathf.Clamp(ammo, 0, maxAmmo);
        playerUI.ammoText.text = ammo.ToString();
        if (Input.GetMouseButtonDown(0) && ammo > 0)
        {
            Shooting();
        }

        if (Input.GetMouseButton(1))
        {
            Aiming();
        }
        else
        {
            animator.SetBool("toAim", false);
        }
    }
}
