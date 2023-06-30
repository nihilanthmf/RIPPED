using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject weaponCamera;

    [Header("Weapons")]
    public Shotgun shotgun;
    public Rifle rifle;

    [SerializeField] PlayerUI playerUI;

    float interactionDistance = 3;

    public bool dead { get; private set; }

    public float health { get; set; } = 100;

    float timeAfterCollideWithFloor = 0;
    float deltaTimeBetweenTakingDamageFromDamagingFloor = 0.9f;

    //Other
    [Header("Other stuff")]
    [SerializeField] LayerMask interactiveObjectsMask;

    [SerializeField] Animator deathTintAnimation;

    Animator animator;

    Camera mainCamera;

    CharacterController character;


    private void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        character = GetComponent<CharacterController>();
    }

    void InteractingRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionDistance, interactiveObjectsMask))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                hit.transform.GetComponent<InteractiveObject>().PerformAction();
            }
        }
    }

    public void TakingDamage(float damage)
    {
        health -= damage;
        health = (int)health;

        playerUI.TakeDamageEffect();
        if (health <= 0 && !dead)
        {
            health = 0;
            Death();
        }
    }

    void Death()
    {
        character.height = 0.1f;
        animator.Play("Death");
        playerUI.hasDied = true;
        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>(); // Assigning a Rigidbody for player to fall on the ground
        rigidbody.mass = 100;
        rigidbody.drag = 0.2f;

         BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, 0.55f, 0);
        boxCollider.size = Vector3.one * 0.8f; // Assigning a collider for player not to fall through the ground

        deathTintAnimation.Play("DeathTintTurningBlack"); // Turning the screen dark

        dead = true;
    }


    private void Update()
    {
        health = Mathf.Clamp(health, 0, 100);
        InteractingRaycast();
        LavaFloorCheck();
    }

    void LavaFloorCheck()
    {
        // Lava floor
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.2f))
        {
            if (hit.transform.tag == "Damaging Floor" && Time.time >= timeAfterCollideWithFloor)
            {
                timeAfterCollideWithFloor = Time.time + deltaTimeBetweenTakingDamageFromDamagingFloor;
                TakingDamage(10);
            }
            if (hit.transform.tag != "Damaging Floor") // to reset the variable for delta time between taking damage when stepped out of lava
            {
                timeAfterCollideWithFloor = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        WeaponToPickUp weaponToPickUp = null;
        other.TryGetComponent(out weaponToPickUp);

        if (other.GetComponent<EnemyTrigger>())
        {
            other.GetComponent<EnemyTrigger>().hasBeenEntered = true;
        }
        if (weaponToPickUp != null)
        {
            weaponToPickUp.GetPickedUp();
        }
    }
}
