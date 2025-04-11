using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimationTester : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Text stateText;

    // Animation parameter names (match those in PlayerController)
    private readonly string isRunningParam = "IsRunning";
    private readonly string isShootingParam = "IsShooting";

    // State variables
    private bool isWalking = false;
    private bool isShooting = false;
    private float shootingTimer = 0f;

    private void Start()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
            if (playerAnimator == null)
            {
                Debug.LogError("Animator not found on this GameObject or not set in inspector.");
                enabled = false;
                return;
            }
        }

        UpdateStateText();
    }

    private void Update()
    {
        // Test walking with WASD keys
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            isWalking = true;
            UpdateStateText();
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                isWalking = false;
                UpdateStateText();
            }
        }

        // Test shooting with mouse click
        if (Input.GetMouseButtonDown(0) && !isShooting)
        {
            isShooting = true;
            shootingTimer = 0.5f; // Duration of shooting animation
            UpdateStateText();
        }

        // Handle shooting timer
        if (isShooting)
        {
            shootingTimer -= Time.deltaTime;
            if (shootingTimer <= 0)
            {
                isShooting = false;
                UpdateStateText();
            }
        }

        // Update animation parameters
        playerAnimator.SetBool(isRunningParam, isWalking);
        playerAnimator.SetBool(isShootingParam, isShooting);
    }

    // Manual control methods for use with UI buttons
    public void ToggleWalking()
    {
        isWalking = !isWalking;
        UpdateStateText();
    }

    public void TriggerShooting()
    {
        if (!isShooting)
        {
            isShooting = true;
            shootingTimer = 0.5f;
            UpdateStateText();
        }
    }

    private void UpdateStateText()
    {
        if (stateText != null)
        {
            string stateName = "Idle";

            if (isWalking && isShooting)
                stateName = "Walking + Shooting";
            else if (isWalking)
                stateName = "Walking";
            else if (isShooting)
                stateName = "Shooting";

            stateText.text = "Current State: " + stateName;
        }
    }

    // Helper method to demonstrate setting up the animator in code
    private void SetupAnimatorExample()
    {
        // This is just a demonstration and should not be called in runtime
        // It shows how you might programmatically set up the animator

        // Create an AnimatorController
        /*
        AnimatorController controller = new AnimatorController();

        // Create parameters
        controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsShooting", AnimatorControllerParameterType.Bool);

        // Create states
        AnimatorState idleState = controller.CreateState("Idle");
        AnimatorState walkingState = controller.CreateState("Walking");
        AnimatorState shootingState = controller.CreateState("Shooting");

        // Set up transitions
        // Idle -> Walking
        AnimatorStateTransition idleToWalk = idleState.AddTransition(walkingState);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");

        // Walking -> Idle
        AnimatorStateTransition walkToIdle = walkingState.AddTransition(idleState);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");

        // Idle -> Shooting
        AnimatorStateTransition idleToShoot = idleState.AddTransition(shootingState);
        idleToShoot.AddCondition(AnimatorConditionMode.If, 0, "IsShooting");

        // Walking -> Shooting
        AnimatorStateTransition walkToShoot = walkingState.AddTransition(shootingState);
        walkToShoot.AddCondition(AnimatorConditionMode.If, 0, "IsShooting");

        // Shooting -> Idle
        AnimatorStateTransition shootToIdle = shootingState.AddTransition(idleState);
        shootToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsShooting");
        shootToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");

        // Shooting -> Walking
        AnimatorStateTransition shootToWalk = shootingState.AddTransition(walkingState);
        shootToWalk.AddCondition(AnimatorConditionMode.IfNot, 0, "IsShooting");
        shootToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
        */
    }
}