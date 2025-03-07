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

    // Component references
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // State variables
    private float horizontalInput;
    private bool isGrounded;
    private bool isJumping;
    private bool isSprinting;

    // Animation parameter names
    private readonly string isRunningParam = "IsRunning";
    private readonly string isJumpingParam = "IsJumping";
    private readonly string isFallingParam = "IsFalling";
    private readonly string isSprintingParam = "IsSprinting";

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
    }

    private void Update()
    {
        // Get input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Check if player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump when the jump button is pressed and the player is grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
        }

        // Update animations
        UpdateAnimations();

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