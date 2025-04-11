using UnityEngine;

public class PlayerAnimatorSetup : MonoBehaviour
{
    /*
    This script provides guidance for setting up your Player Animator controller.

    Follow these steps to set up your Player Animator:

    1. Create an Animator Controller for your player

    2. Add the following parameters to the Animator:
       - IsWalking (Boolean): True when the player is moving
       - IsShooting (Boolean): True when the player is shooting

    3. Create the following Animation States:
       - Idle: Default idle animation for player
       - Walking: Walking animation for player
       - Shooting: Shooting animation for player

    4. Set up transitions between states:
       - Idle → Walking: When IsWalking = true
       - Walking → Idle: When IsWalking = false
       - Idle → Shooting: When IsShooting = true
       - Walking → Shooting: When IsShooting = true
       - Shooting → Idle: When IsShooting = false AND IsWalking = false
       - Shooting → Walking: When IsShooting = false AND IsWalking = true

    Make sure to set appropriate transition times and exit times for smooth animation transitions.
    */

    private void Awake()
    {
        // This script doesn't need to do anything at runtime
        // It's purely for documentation purposes
        Destroy(this);
    }
}