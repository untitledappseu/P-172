using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private bool destroyOnContact = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);

                if (destroyOnContact)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}