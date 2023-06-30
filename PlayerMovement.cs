using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
// some basic X-Z movement stuff
    [HideInInspector] public float velocity;
    float startDefaultSpeed;
    public float startVelocity { get; private set; }
    float speedAdjustingForPowerSlide;
    Vector3 movement;

    // movement values
    [SerializeField] float defaultSpeed = 18;
    [SerializeField] float startSpeedAdjustingForPowerSlide = 40; // start speed when sliding; also used as delta values to take from sliding speed * Time.deltatime
    float startSpeedAdjustingForPowerSlideReducing = 30f; // the greater the value, the faster the startSpeedAdjustingForPowerSlide will reduce
    float timeForNextJump, jumpDeltaTime = 0.5f;

    // number consts
    [SerializeField] float explosionForce = 18;
    [SerializeField] float jumpForce = 13f;
    [SerializeField] float gravity = 20f;
    public float verticalExplosionForceMultiplier { get; set; } = 0.5f;

    float startGravity;

    bool wasMovingWhileCrouched;

    // Axis
    float horizontal, vertical;

    [SerializeField] Transform groundCheck;
    [SerializeField] Transform ceilingCheck;

    CharacterController character;
    [SerializeField] LayerMask groundMask;

    Animator animator;
    
    private void Start()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        velocity = defaultSpeed;
        startVelocity = velocity;
        startDefaultSpeed = defaultSpeed;
        startGravity = gravity;
    }

    /// <summary>
    /// All the movement management happens here
    /// </summary>
    void Movement()
    {
        // this variable represents the speed of the player atm (its cant be == 0, cuz the speed == 0 when the Input.GetAxis("Vertical") == 0)
        // this is the speed only when the wlking button is pressed
        velocity = defaultSpeed + speedAdjustingForPowerSlide;

        if (CeilingCheck() && movement.y > 0)
        {
            // Making it so the player will stop jumping when touch ceilings
            forceVector.y = -0.4f;
        }

        movement = transform.right * horizontal + transform.forward * vertical + forceVector;

        Gravity();
        Crouching();
        Jumping();

        character.Move(movement * Time.deltaTime);
    }

    /// <summary>
    /// Applying gravity forces to a player
    /// </summary>
    void Gravity()
    {
        if (character.isGrounded && movement.y <= -0.1f)
        {
            gravity = 0;
            movement.y = -1f;
        }
        else
        {
            gravity = startGravity;
        }

        forceVector.y -= gravity * Time.deltaTime;

        forceVector = Vector3.MoveTowards(forceVector, new Vector3(0, forceVector.y, 0), Time.deltaTime * 15);
    }

    bool CeilingCheck()
    {
        return Physics.CheckSphere(ceilingCheck.position, 0.25f, groundMask);
    }
    float lerpTime = 10;
    float lerpResult;


    void Crouching()
    {
        character.height = Mathf.MoveTowards(character.height, lerpResult, lerpTime * Time.deltaTime);

        if (character.height != 1 && character.height != 2)
        {
            gravity = 0;
        }

        defaultSpeed = lerpResult == 1 ? startDefaultSpeed / 2.3f : startDefaultSpeed; // if you crouch, the default Speed will be divided by X once

        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetAxisRaw("Vertical") != 0) // if he was moving when crouched
        {
            wasMovingWhileCrouched = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl)) // reseting the bool when uncrouches
        {
            wasMovingWhileCrouched = false;
        }

        if (wasMovingWhileCrouched)
        {
            speedAdjustingForPowerSlide = Input.GetKeyDown(KeyCode.LeftControl) ? startSpeedAdjustingForPowerSlide : speedAdjustingForPowerSlide;
        }

        // assigning value to the variable for the height of the player making sure he doesnt stand up inside the wall when the ceiling is too low 
        float maxCheckerRayDistance = 0.35f;
        if (Physics.Raycast(ceilingCheck.position, Vector3.up, maxCheckerRayDistance, LayerMask.GetMask("Default")))
        {
            lerpResult = 1;
        }
        else
        {
            lerpResult = Input.GetKey(KeyCode.LeftControl) ? 1 : 2;
        }

        speedAdjustingForPowerSlide = lerpResult == 2 ? 0 : speedAdjustingForPowerSlide;

        // Decreasing speedAdjustingForPowerSlide every frame to smoothly decrease player's velocity while sliding
        speedAdjustingForPowerSlide -= Mathf.Clamp(speedAdjustingForPowerSlide, -1, 1) * Time.deltaTime * startSpeedAdjustingForPowerSlideReducing;
    }

    Vector3 forceVector;
    /// <summary>
    /// Adding force to a player (used in to add jump force, explosion force, etc.)
    /// </summary>
    void AddForce(Vector3 forceVector_1)
    {
        forceVector = new Vector3(forceVector.x, 0.075f, forceVector.z);

        forceVector += forceVector_1;
    }

    /// <summary>
    /// Jump from the ground
    /// </summary>
    void Jumping()
    {
        //Jump
        if (Input.GetKey(KeyCode.Space) && Time.time >= timeForNextJump)
        {
            if (character.isGrounded)
            {
                animator.SetBool("toSlide", false);

                AddForce(Vector3.up * jumpForce);

                timeForNextJump = Time.time + jumpDeltaTime;
            }
        }
    }

    /// <summary>
    /// Adding force to a player of explosion
    /// </summary>
    public void ExplosionJump(Vector3 forceVector_1)
    {
        AddForce(forceVector_1 * explosionForce);
    }

    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal") * velocity;
        vertical = Input.GetAxis("Vertical") * velocity;

        // This line shakes camera by aniamtion; its being played if the velocity fo played is greater then 0
        animator.SetFloat("toWalk", Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")));

        Movement();
    }
}
