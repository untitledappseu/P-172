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
    [SerializeField] private GameObject cooldownIndicator;

    // References
    private Camera mainCamera;
    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer gunSpriteRenderer;

    // State variables
    private float cooldownTimer;
    private bool canShoot = true;
    private Vector3 mousePosition;
    private Vector3 aimDirection;
    private float angle;

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

        // Initialize cooldown indicator if assigned
        if (cooldownIndicator != null)
        {
            cooldownIndicator.SetActive(false);
        }
    }

    private void Update()
    {
        // Get mouse position in world space
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        // Calculate aim direction and angle
        aimDirection = (mousePosition - transform.position).normalized;
        angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Rotate gun towards mouse
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Handle gun flipping based on player direction
        if (flipWithPlayer && playerSpriteRenderer != null && gunSpriteRenderer != null)
        {
            // If player is facing left, flip the gun vertically
            if (playerSpriteRenderer.flipX)
            {
                transform.localScale = new Vector3(1f, -1f, 1f);
                transform.localPosition = new Vector3(gunOffset, -gunOffset, 0f);
            }
            else
            {
                transform.localScale = Vector3.one;
                transform.localPosition = new Vector3(gunOffset, gunOffset, 0f);
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