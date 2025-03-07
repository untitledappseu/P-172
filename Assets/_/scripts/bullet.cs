using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private GameObject hitEffect;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we should collide with this layer

        // Apply damage if the object has a health component
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        // Spawn hit effect rotated to match hit surface normal
        if (hitEffect != null)
        {
            Vector3 hitNormal = collision.contacts[0].normal;
            Quaternion hitRotation = Quaternion.FromToRotation(Vector3.up, hitNormal);
            Instantiate(hitEffect, transform.position, hitRotation);
        }

        // Destroy the bullet
        Destroy(gameObject);

    }


}