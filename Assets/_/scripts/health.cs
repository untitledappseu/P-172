using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool destroyOnDeath = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onDamage;
    [SerializeField] private UnityEvent onDeath;


    private SpriteRenderer _rend;
    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        _rend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        // Invoke damage event
        onDamage?.Invoke();

        

        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Damage(damageAmount));
        }
    }


    public IEnumerator Damage(float dam)
    {
        _rend.color = Color.red;

        yield return new WaitForSeconds(0.3f);
        _rend.color = Color.white;

        yield return null;
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
    }

    private void Die()
    {
        // Invoke death event
        onDeath?.Invoke();

        // Destroy the game object if set to do so
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }

    // Getters for health values
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
}