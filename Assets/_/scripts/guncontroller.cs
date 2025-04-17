using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float cooldownTime = 0.5f;
    [SerializeField] private float bulletLifetime = 2f;

    [Header("Visual Settings")]
    [SerializeField] private bool flipWithPlayer = true;
    [SerializeField] private float gunOffset = 0.5f;
    [SerializeField] private float rightGunOffset = 0.5f;
    [SerializeField] private float leftGunOffset = 0.5f;
    [SerializeField] private GameObject cooldownIndicator;
    [SerializeField] private bool followMovementDirection = true;

    // References
    private Camera mainCamera;
    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer gunSpriteRenderer;

    // State variables
    private float cooldownTimer;
    private bool canShoot = true;
    private bool facingLeft = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        gunSpriteRenderer = GetComponent<SpriteRenderer>();

        // Try to get player sprite renderer from parent
        if (transform.parent != null)
        {
            playerSpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        }

        // Create fire point if not assigned
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.parent = transform;
            firePointObj.transform.localPosition = new Vector3(0.5f, 0f, 0f);
            firePoint = firePointObj.transform;
        }
        else
        {
            // Make sure firePoint is correctly positioned
            firePoint.localPosition = new Vector3(0.5f, 0f, 0f);
        }

        // Initialize cooldown indicator if assigned
        if (cooldownIndicator != null)
        {
            cooldownIndicator.SetActive(false);
        }
    }

    private void Update()
    {
        // Get movement input
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Set gun position based on direction
        Vector3 newPosition = Vector3.zero;

        // Determine facing direction
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            // Update facing direction based on movement
            facingLeft = horizontalInput < 0;
        }
        else if (playerSpriteRenderer != null)
        {
            // If not moving, use the player's current sprite direction
            facingLeft = playerSpriteRenderer.flipX;
        }

        // Set position based on facing direction
        if (facingLeft)
        {
            // Facing left
            newPosition = new Vector3(-leftGunOffset, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else
        {
            // Facing right
            newPosition = new Vector3(rightGunOffset, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        // Apply the position
        transform.localPosition = newPosition;

        // Apply scaling based on direction
        if (flipWithPlayer && gunSpriteRenderer != null)
        {
            if (facingLeft)
            {
                transform.localScale = new Vector3(1f, -1f, 1f);
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }

        // Handle cooldown timer
        if (!canShoot)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0)
            {
                canShoot = true;
                if (cooldownIndicator != null)
                {
                    cooldownIndicator.SetActive(false);
                }
            }
        }

        // Shoot when left mouse button is clicked (not held) and not in cooldown
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Shoot();
            canShoot = false;
            cooldownTimer = cooldownTime;

            // Show cooldown indicator if assigned
            if (cooldownIndicator != null)
            {
                cooldownIndicator.SetActive(true);
            }
        }
    }

    private void Shoot()
    {
        // Create bullet at fire point
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Get bullet's rigidbody and set velocity
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = firePoint.right * bulletSpeed;
        }

        // Destroy bullet after lifetime
        Destroy(bullet, bulletLifetime);
    }

    // Public method to check if gun can shoot (useful for UI or other systems)
    public bool CanShoot()
    {
        return canShoot;
    }

    // Public method to get cooldown progress (0 to 1, where 1 is ready to shoot)
    public float GetCooldownProgress()
    {
        if (canShoot) return 1f;
        return 1f - (cooldownTimer / cooldownTime);
    }
}