using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    private EnemyController enemyController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyController = GetComponent<EnemyController>();

        if (animator == null)
        {
            Debug.LogError("Animator component missing on enemy!");
            enabled = false;
            return;
        }

        if (enemyController == null)
        {
            Debug.LogError("EnemyController component missing on enemy!");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        // Set the enemy type in the animator
        EnemyController.EnemyType enemyType = enemyController.GetEnemyType();
        animator.SetInteger("EnemyType", (int)enemyType);
    }

    // Example of directly playing the correct walking animation based on enemy type
    public void PlayWalkAnimation()
    {
        EnemyController.EnemyType enemyType = enemyController.GetEnemyType();

        switch (enemyType)
        {
            case EnemyController.EnemyType.Dinosaur:
                animator.Play("Dinosaur_Walk");
                break;
            case EnemyController.EnemyType.Ama:
                animator.Play("Ama_Walk");
                break;
            case EnemyController.EnemyType.Lumen:
                animator.Play("Lumen_Walk");
                break;
        }
    }

    // Example of playing an attack animation based on enemy type
    public void PlayAttackAnimation()
    {
        EnemyController.EnemyType enemyType = enemyController.GetEnemyType();

        switch (enemyType)
        {
            case EnemyController.EnemyType.Dinosaur:
                animator.Play("Dinosaur_Attack");
                break;
            case EnemyController.EnemyType.Ama:
                animator.Play("Ama_Attack");
                break;
            case EnemyController.EnemyType.Lumen:
                animator.Play("Lumen_Attack");
                break;
        }
    }

    // Example of manually transitioning animations based on state
    public void UpdateAnimation(bool isMoving, bool isAttacking)
    {
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsAttacking", isAttacking);
    }
}