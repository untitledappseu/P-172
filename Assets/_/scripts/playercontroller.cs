using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Audio")]
    [SerializeField] private float footstepRate = 0.3f;
    [SerializeField] private float sprintFootstepRateMultiplier = 1.5f;
    [SerializeField] private float minLandingVelocity = 5f;
    [SerializeField] private float hardLandingVelocity = 10f; // Threshold for hard landing sounds
    [SerializeField] private bool debugAudio = true; // Toggle for audio debugging

    // Component references
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // State variables
    private float horizontalInput;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isJumping;
    private bool isSprinting;
    private float footstepTimer;

    // Animation parameter names
    private readonly string isRunningParam = "IsRunning";
    private readonly string isJumpingParam = "IsJumping";
    private readonly string isFallingParam = "IsFalling";
    private readonly string isSprintingParam = "IsSprinting";
    private readonly string isShootingParam = "IsShooting";

    // Shooting state
    private bool isShooting = false;
    private float shootAnimationDuration = 0.5f;
    private float shootAnimationTimer = 0f;

    private void Awake()
    {
        // Get component references
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Ensure we have a ground check
        if (groundCheck == null)
        {
            Debug.LogWarning("Ground check transform not assigned. Creating one.");
            GameObject checkObj = new GameObject("GroundCheck");
            checkObj.transform.parent = transform;
            checkObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = checkObj.transform;
        }

        if (debugAudio)
        {
            Debug.Log("PlayerController initialized with audio debugging enabled");
        }
    }

    private void Start()
    {
        // Check if AudioManager exists
        if (AudioManager.Instance == null)
        {
            Debug.LogError("PlayerController: AudioManager not found! Footsteps and landing sounds won't work.");
        }
        else if (debugAudio)
        {
            Debug.Log("PlayerController: AudioManager found successfully");
        }
    }

    private void Update()
    {
        // Get input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Check for shooting input
        if (Input.GetMouseButtonDown(0))
        {
            isShooting = true;
            shootAnimationTimer = shootAnimationDuration;
        }

        // Handle shooting animation timer
        if (isShooting)
        {
            shootAnimationTimer -= Time.deltaTime;
            if (shootAnimationTimer <= 0)
            {
                isShooting = false;
            }
        }

        // Store previous ground state
        wasGrounded = isGrounded;

        // Check if player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Check for landing
        if (isGrounded && !wasGrounded)
        {
            OnLanding();
        }

        // Jump when the jump button is pressed and the player is grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
        }

        // Update animations
        UpdateAnimations();

        // Handle footstep sounds
        UpdateFootsteps();

        // Flip sprite based on movement direction
        if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        // Calculate current speed based on sprint state
        float currentSpeed = moveSpeed;
        if (isSprinting && horizontalInput != 0)
        {
            currentSpeed *= sprintMultiplier;
        }

        // Handle horizontal movement
        rb.velocity = new Vector2(horizontalInput * currentSpeed, rb.velocity.y);

        // Handle jumping
        if (isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = false;
        }

        // Better jump physics
        if (rb.velocity.y < 0)
        {
            // Falling
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Short jump
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void OnLanding()
    {
        // Play landing sound if we had significant downward velocity
        if (rb.velocity.y < -minLandingVelocity && AudioManager.Instance != null)
        {
            // Adjust volume based on landing velocity
            float landingVolume = Mathf.Clamp01(Mathf.Abs(rb.velocity.y) / 20f);

            // Use different sound type for hard landings
            SoundType landingType = SoundType.PlayerLanding;

            if (debugAudio)
            {
                Debug.Log($"PlayerController: Playing landing sound. Velocity: {rb.velocity.y}, Volume: {landingVolume}");
            }

            AudioManager.Instance.PlaySound(landingType, transform.position, landingVolume);
        }
        else if (debugAudio)
        {
            if (AudioManager.Instance == null)
            {
                Debug.LogWarning("PlayerController: Can't play landing sound - AudioManager not found");
            }
            else if (rb.velocity.y >= -minLandingVelocity)
            {
                Debug.Log($"PlayerController: Landing velocity too low for sound: {rb.velocity.y} (min: {-minLandingVelocity})");
            }
        }
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            // Set running animation
            animator.SetBool(isRunningParam, Mathf.Abs(horizontalInput) > 0.1f);

            // Set sprinting animation - commented out until animator is set up with this parameter
            // animator.SetBool(isSprintingParam, isSprinting && Mathf.Abs(horizontalInput) > 0.1f);

            // Set jumping animation
            animator.SetBool(isJumpingParam, !isGrounded && rb.velocity.y > 0);

            // Set falling animation
            animator.SetBool(isFallingParam, !isGrounded && rb.velocity.y < 0);

            // Set shooting animation
            animator.SetBool(isShootingParam, isShooting);
        }
    }

    private void UpdateFootsteps()
    {
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f;

        // Only play footsteps when moving on the ground
        if (isGrounded && isMoving && AudioManager.Instance != null)
        {
            // Calculate footstep rate based on sprint state
            float currentFootstepRate = footstepRate;
            if (isSprinting)
            {
                currentFootstepRate /= sprintFootstepRateMultiplier;
            }

            // Update footstep timer
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                // Play footstep sound
                if (debugAudio)
                {
                    Debug.Log($"PlayerController: Playing footstep sound. Moving: {isMoving}, Grounded: {isGrounded}, Sprint: {isSprinting}");
                }

                AudioManager.Instance.PlaySound(SoundType.PlayerFootstep, transform.position);
                footstepTimer = currentFootstepRate;
            }
        }
        else
        {
            // Reset timer when not moving or not on ground
            footstepTimer = 0;

            if (debugAudio && isMoving && !isGrounded)
            {
                Debug.Log("PlayerController: Not playing footsteps - player is not grounded");
            }
            else if (debugAudio && !isMoving && isGrounded)
            {
                Debug.Log("PlayerController: Not playing footsteps - player is not moving");
            }
            else if (debugAudio && AudioManager.Instance == null)
            {
                Debug.LogWarning("PlayerController: Can't play footsteps - AudioManager not found");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the ground check radius in the editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // Public getter for sprint state
    public bool IsSprinting()
    {
        return isSprinting && Mathf.Abs(horizontalInput) > 0.1f;
    }
}