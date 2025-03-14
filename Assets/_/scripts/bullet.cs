using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private GameObject hitEffect;

    // Optional override for volume
    [SerializeField] private float shootSoundVolumeMultiplier = 3f; // 300% volume by default
    [SerializeField] private float impactSoundVolumeMultiplier = 1f;

    private void Start()
    {
        // Play shoot sound when bullet is created
        if (AudioManager.Instance != null)
        {
            Debug.Log("Bullet: Playing shoot sound");
            AudioManager.Instance.PlaySound(SoundType.BulletShoot, transform.position, shootSoundVolumeMultiplier);
        }
        else
        {
            Debug.LogError("Bullet: AudioManager instance not found!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Bullet: Collision with {collision.gameObject.name}");

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

        // Play impact sound using AudioManager
        if (AudioManager.Instance != null)
        {
            Debug.Log("Bullet: Playing impact sound");
            AudioManager.Instance.PlaySound(SoundType.BulletImpact, transform.position, impactSoundVolumeMultiplier);
        }
        else
        {
            Debug.LogError("Bullet: AudioManager instance not found!");
        }

        // Destroy the bullet
        Destroy(gameObject);
    }
}