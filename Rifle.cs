using System.Collections;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    [Header("GameObjects")]
    Camera mainCamera;
    CameraController cameraController;
    [SerializeField] GameObject fireEffect;
    [SerializeField] GameObject player;
    [SerializeField] GameObject bulletImpact;
    [SerializeField] PlayerMovement playerMovement;
    [HideInInspector] public WeaponDefault weaponDefault;

    [Header("Other Stuff")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] ParticleSystem smokeParticles;
    [SerializeField] ParticleSystem bloodParticles;
    [SerializeField] PlayerUI playerUI;

    public Animator animator { get; private set; }

    [SerializeField] int damage = 20;
    public int ammo;
    public int maxAmmo { get; private set; } = 100;

    bool toMakeEnemyBleedOnce;
    bool isAimed;

    float timeToFire;
    [SerializeField] float fireRate;

    private void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        weaponDefault = GetComponent<WeaponDefault>();  

        cameraController = mainCamera.GetComponent<CameraController>();
    }

    // All the visual shooting effects must be called here
    IEnumerator ShootingVisual()
    {
        smokeParticles.Play();
        fireEffect.SetActive(true);

        yield return new WaitForSeconds(0.025f);
        fireEffect.SetActive(false);


        //string animationToPlayName = animator.GetBool("toAim") ? "AimShot" : "Shot";
        // the code above is if we have aiming animation, the code below is if we don't
        string animationToPlayName;
        if (isAimed)
        {
            animationToPlayName = "AimShoot";
        }
        else
        {
            animationToPlayName = "Shoot";
        }
        animator.Play(animationToPlayName);
    }

    void Shooting()
    {
        ammo--;
        toMakeEnemyBleedOnce = true;
        RaycastHit hitPoint;
        Target target = null;
        timeToFire = Time.time + fireRate;

        // Checking for an enemy hit by one of the rays
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hitPoint, 200, targetLayer))
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
                    target.PerformAction(damage * 100);
                    target.GetComponent<DefaultEnemyClass>().BlowingHead();

                    hitPoint.collider.transform.gameObject.SetActive(false); // turning the head off
                    hitPoint.collider.transform.GetChild(1).gameObject.SetActive(true); // letting the head blod flow
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

        StartCoroutine(ShootingVisual());
    }

    void Aiming()
    {
        animator.SetBool("toAim", true);
        mainCamera.fieldOfView = Mathf.MoveTowards(mainCamera.fieldOfView, 40, 135 * Time.deltaTime);
        playerMovement.velocity = playerMovement.startVelocity / 2.3f;

        isAimed = true;
    }

    void UnAim()
    {
        animator.SetBool("toAim", false);
        cameraController.toResetFieldOfView = true;

        isAimed = false;
    }

    private void Update()
    {
        ammo = Mathf.Clamp(ammo, 0, maxAmmo);
        playerUI.ammoText.text = ammo.ToString();
        if (Input.GetMouseButtonDown(0) && ammo > 0 && Time.time >= timeToFire)
        {
            Shooting();
        }

        if (Input.GetMouseButton(1))
        {
            Aiming();
        }
        else
        {
            UnAim();
        }
    }
}
