using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool invincibleAfterHit = true;
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private GameObject hitEffect;

    public event Action OnPlayerDeath;
    public event Action<int, int> OnHealthChanged;

    private int currentHealth;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    private void Start()
    {
        RestoreHealth();
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        if (invincibleAfterHit)
        {
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void RestoreHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void AddHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}