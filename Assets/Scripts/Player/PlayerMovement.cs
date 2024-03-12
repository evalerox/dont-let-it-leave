using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float airMultiplier;

    public float jumpForce;
    public float jumpCooldown;
    bool readyToJump;

    public float dashForce;
    public float dashCooldown;
    [HideInInspector]
    public bool readyToDash;
    [HideInInspector]
    public bool dashDone;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;
    public Transform cameraOrientation;

    public Animator animator;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public AudioSource jumpSound;
    public AudioSource footStepsSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        readyToDash = true;

        grounded = false;
    }

    private void Update()
    {
        InputControl();
        SpeedControl();
        GroundControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void InputControl()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        bool isMoving = horizontalInput > 0.1f || horizontalInput < -0.1f || verticalInput > 0.1f || verticalInput < -0.1f;
        animator.SetBool("Running", isMoving);

        if (isMoving)
        {
            footStepsSound.pitch = Random.Range(0.8f, 1.2f);
            footStepsSound.enabled = true;
        }
        else
        {
            footStepsSound.enabled = false;
        }

        // Check when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            animator.SetTrigger("Jump");
            readyToJump = false;
            Jump();

            jumpSound.pitch = Random.Range(0.9f, 1.1f);
            jumpSound.Play();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Check Dash
        if (Input.GetKeyDown(dashKey) && readyToDash)
        {
            //animator.SetTrigger("Jump");
            dashDone = true;
            readyToDash = false;
            rb.drag = 0;
            Dash();

            jumpSound.pitch = Random.Range(0.9f, 1.1f);
            jumpSound.Play();

            rb.drag = groundDrag;
            Invoke(nameof(ResetDashDone), 0.5f);
            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            // On ground
            rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Acceleration);
        }
        else
        {
            // In air
            rb.AddForce(10f * airMultiplier * moveSpeed * moveDirection.normalized, ForceMode.Acceleration);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

        // Limit velocity if exceeds
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void GroundControl()
    {
        float maxDistance = playerHeight * 0.5f + 0.3f;

        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, maxDistance, whatIsGround);

        // Handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void Jump()
    {
        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(100f * jumpForce * transform.up, ForceMode.Impulse);
    }

    private void Dash()
    {
        // When there is no input just dash forward
        if (horizontalInput == 0 && verticalInput == 0)
        {
            Vector3 dashDirection = new(cameraOrientation.forward.x, 0, cameraOrientation.forward.z);
            rb.AddForce(10f * dashForce * dashDirection, ForceMode.VelocityChange);
        }
        else
        {
            rb.AddForce(10f * dashForce * moveDirection.normalized, ForceMode.VelocityChange);
        }
    }

    private void ResetJump() { readyToJump = true; }

    private void ResetDash() { readyToDash = true; }
    private void ResetDashDone() { dashDone = false; }
}