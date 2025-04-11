using UnityEngine;

public class EnemyTypeTester : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;

    public void SetToDinosaur()
    {
        if (enemyController != null)
        {
            enemyController.SetEnemyType(EnemyController.EnemyType.Dinosaur);
        }
    }

    public void SetToAma()
    {
        if (enemyController != null)
        {
            enemyController.SetEnemyType(EnemyController.EnemyType.Ama);
        }
    }

    public void SetToLumen()
    {
        if (enemyController != null)
        {
            enemyController.SetEnemyType(EnemyController.EnemyType.Lumen);
        }
    }

    // Testing with keyboard input
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetToDinosaur();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetToAma();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetToLumen();
        }
    }
}