using UnityEngine;
using UnityEngine.UI;

public class Reaper : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] PlayerUI playerUI;
    [SerializeField] Slider powerupBar;
    [SerializeField] PlayerMovement playerMovement;
    Animator animator;
    ReaperBlade reaperBlade;
    Camera mainCamera;
    CameraController cameraController;
    Vector3 fullyChargedPosition;
    [SerializeField] Vector3 shotPosition;

    public float forwardForce;

    bool toShoot;
    bool bladeIsOnPlace;

    int ammo = 1;
    public float powerUp = 1;
    const float powerUpDelta = 0.7f;
    const float maxPowerUp = 2.5f;

    float startBladeDamage;
    float startCameraFieldOfView;
    float startForwardForce;

    private void Start()
    {
        reaperBlade = projectile.GetComponent<ReaperBlade>();
        animator = GetComponent<Animator>();
        startBladeDamage = reaperBlade.damage;
        mainCamera = Camera.main;
        cameraController = mainCamera.GetComponent<CameraController>();
        startCameraFieldOfView = mainCamera.fieldOfView;
        startForwardForce = forwardForce;
        powerupBar.maxValue = maxPowerUp - 1;
        fullyChargedPosition = transform.localPosition + new Vector3(0, 0, -0.666f);
    }

    public void Shoot()
    {
        reaperBlade.transform.localPosition = new Vector3(0.0715641528f, -0.00706735253f, 0.0173553359f);
        reaperBlade.toChangeTime = true;
        animator.Play("Shoot");

        cameraController.toResetFieldOfView = true;

        powerupBar.gameObject.SetActive(false);
        powerUp = 1;

        reaperBlade.Shoot();
    }

    float timeButtonPressed;
    void PoweringUp()
    {
        if (Input.GetMouseButtonDown(0) && bladeIsOnPlace)
        {
            timeButtonPressed = Time.time;
        }

        if (Input.GetMouseButton(0) && bladeIsOnPlace)
        {
            if (Time.time >= timeButtonPressed + 0.1f)
            {
                powerUp += powerUpDelta * Time.deltaTime;

                reaperBlade.damage = startBladeDamage * powerUp;
                powerupBar.gameObject.SetActive(true);
                animator.enabled = false;

                transform.localPosition = Vector3.Slerp(transform.localPosition, fullyChargedPosition, 0.75f * Time.deltaTime);
            }
        }

        if (powerUp != 1)
        {
            playerMovement.velocity = playerMovement.startVelocity / (powerUp * 1.5f);
            mainCamera.fieldOfView = startCameraFieldOfView / ((1 + powerUp) * 0.5f);
            forwardForce = startForwardForce * (powerUp * 0.85f);
        }

        powerupBar.value = powerUp - 1;

        powerUp = Mathf.Clamp(powerUp, 1, maxPowerUp);
    }


    private void Update()
    {
        ammo = bladeIsOnPlace ? 1 : 0;
        playerUI.ammoText.text = ammo.ToString();

        if (Input.GetMouseButtonUp(0) && bladeIsOnPlace)
        {
            toShoot = true;
            animator.enabled = true;
        }

        PoweringUp();

        //shotDistanceCheck = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, 1.5f, LayerMask.GetMask("Static"));
        bladeIsOnPlace = reaperBlade.transform.parent == transform ? true : false;
    }

    private void FixedUpdate()
    {
        if (toShoot)
        {
            Shoot();
            toShoot = false;
        }
    }
}
