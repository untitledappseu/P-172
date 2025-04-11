using UnityEngine;

public class EnemyAnimatorSetup : MonoBehaviour
{
    /*
    This script provides guidance for setting up your Enemy Animator controller.

    Follow these steps to set up your Animator:

    1. Create an Animator Controller for your enemy
    2. Add only one parameter to the Animator:
       - EnemyType (Integer): 0 = Dinosaur, 1 = Ama, 2 = Lumen

    3. Create the following Animation States:
       - Dinosaur_Walk: Walking animation for Dinosaur type
       - Ama_Walk: Walking animation for Ama type
       - Lumen_Walk: Walking animation for Lumen type

    4. Create transitions directly from Entry to each animation state:
       - Entry → Dinosaur_Walk: EnemyType = 0
       - Entry → Ama_Walk: EnemyType = 1
       - Entry → Lumen_Walk: EnemyType = 2

    This setup allows the Animator to automatically play the correct walking animation
    based on the enemy type set in the EnemyController component.
    */

    private void Awake()
    {
        // This script doesn't need to do anything at runtime
        // It's purely for documentation purposes
        Destroy(this);
    }
}