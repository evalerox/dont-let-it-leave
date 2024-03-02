using UnityEngine;

public class PlayerMovementTutorial : MonoBehaviour
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
    bool readyToDash;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;
    public Transform cameraOrientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

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

        // Check when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Check Dash
        if (Input.GetKeyDown(dashKey) && readyToDash)
        {
            readyToDash = false;
            rb.drag = 0;
            Dash();
            rb.drag = groundDrag;
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
        // TODO: Review if I need this
        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // No me acaba de convencer ninguno de los dos, lo dejo para mas adelante
        //Vector3 jumpDirection = transform.up / 2 + cameraOrientation.forward;
        //Vector3 jumpDirection = new(cameraOrientation.forward.x, 1f, cameraOrientation.forward.z);
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

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void ResetDash()
    {
        readyToDash = true;
    }
}