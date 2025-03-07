using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private float waypointReachedDistance = 0.5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Color attackRangeColor = Color.red;

    [Header("Patrol Points")]
    [SerializeField] private List<Vector2> patrolPoints = new List<Vector2>();
    [SerializeField] private Color patrolPointColor = Color.blue;
    [SerializeField] private float patrolPointSize = 0.3f;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float detectionDistance = 5f;
    [SerializeField] private float viewAngle = 75f;
    [SerializeField] private Transform eyePosition;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Debug")]
    [SerializeField] private bool showDebugRaycasts = true;

    [Header("Hit Effect")]
    [SerializeField] private float hitFlashDuration = 0.2f;
    [SerializeField] private Color hitFlashColor = Color.red;

    // Component references
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;

    // State Management
    private enum EnemyState { Patrolling, Chasing, WaitingAtWaypoint, Attacking }
    private EnemyState currentState;
    private int currentWaypointIndex = 0;
    private Transform playerTransform;
    private Vector2 moveDirection;
    private bool isGrounded;
    private float waitAtWaypointTimer;
    private const float WAYPOINT_WAIT_TIME = 1f; // How long to wait at waypoints
    private bool hasSeenPlayer = false; // New flag to track if player has been spotted
    private float nextAttackTime = 0f;

    private void Awake()
    {
        // Get component references
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError($"[{gameObject.name}] Rigidbody2D component missing!");

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError($"[{gameObject.name}] SpriteRenderer component missing!");
        else originalColor = spriteRenderer.color;

        animator = GetComponent<Animator>();
        if (animator == null) Debug.Log($"[{gameObject.name}] Animator component not found - animations will be disabled");

        // Find the player
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null) Debug.LogWarning($"[{gameObject.name}] Player not found in scene!");

        Debug.Log($"[{gameObject.name}] Initialized with {patrolPoints.Count} patrol points");

        // Set initial state
        currentState = EnemyState.Patrolling;

        // Create ground check if not assigned
        if (groundCheck == null)
        {
            GameObject checkObj = new GameObject("GroundCheck");
            checkObj.transform.parent = transform;
            checkObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = checkObj.transform;
        }

        // Create eye position if not assigned
        if (eyePosition == null)
        {
            GameObject eyeObj = new GameObject("EyePosition");
            eyeObj.transform.parent = transform;
            eyeObj.transform.localPosition = new Vector3(0.5f, 0.2f, 0);
            eyePosition = eyeObj.transform;
        }

        // Initialize patrol points if empty
        if (patrolPoints.Count < 2)
        {
            Debug.LogWarning("Enemy needs at least 2 patrol points! Adding default patrol points.");
            patrolPoints.Add(new Vector2(transform.position.x - 3f, transform.position.y));
            patrolPoints.Add(new Vector2(transform.position.x + 3f, transform.position.y));
        }
    }

    private void Update()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (!isGrounded)
        {
            Debug.LogWarning($"[{gameObject.name}] Not grounded! Check ground layer and ground check setup.");
        }

        // Check for player detection
        if (CanSeePlayer())
        {
            if (!hasSeenPlayer)
            {
                Debug.Log($"[{gameObject.name}] Player spotted! Transitioning to chase mode.");
            }
            hasSeenPlayer = true;
            if (currentState != EnemyState.Chasing && currentState != EnemyState.Attacking)
            {
                SetState(EnemyState.Chasing);
            }
        }

        // If we've seen the player before, stay in chase mode
        if (hasSeenPlayer && currentState != EnemyState.Chasing && currentState != EnemyState.Attacking)
        {
            SetState(EnemyState.Chasing);
        }

        // Check for attack range
        if (playerTransform != null && hasSeenPlayer)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange)
            {
                TryAttackPlayer();
            }
        }

        // State-specific behavior
        switch (currentState)
        {
            case EnemyState.Patrolling:
                PatrolBehavior();
                break;

            case EnemyState.Chasing:
                ChaseBehavior();
                break;

            case EnemyState.WaitingAtWaypoint:
                WaitAtWaypointBehavior();
                break;

            case EnemyState.Attacking:
                AttackBehavior();
                break;
        }

        // Update animations (if we have an animator)
        UpdateAnimations();

        // Flip sprite based on movement direction
        if (moveDirection.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveDirection.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        // Only apply movement if grounded
        if (isGrounded)
        {
            float currentSpeed = currentState == EnemyState.Chasing ? chaseSpeed : moveSpeed;
            rb.velocity = new Vector2(moveDirection.x * currentSpeed, rb.velocity.y);
        }
    }

    private void PatrolBehavior()
    {
        if (patrolPoints.Count < 2)
            return;

        Vector2 currentTarget = patrolPoints[currentWaypointIndex];
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget);

        if (distanceToTarget < waypointReachedDistance)
        {
            // Switch to waiting state when reaching waypoint
            SetState(EnemyState.WaitingAtWaypoint);
            return;
        }

        // Move towards the current patrol point
        moveDirection = new Vector2(
            Mathf.Sign(currentTarget.x - transform.position.x),
            0
        );
    }

    private void ChaseBehavior()
    {
        if (playerTransform == null)
            return;

        // Always move towards the player's position, regardless of sight
        moveDirection = new Vector2(
            Mathf.Sign(playerTransform.position.x - transform.position.x),
            0
        );
    }

    private void WaitAtWaypointBehavior()
    {
        // Wait at the waypoint for a set amount of time
        waitAtWaypointTimer += Time.deltaTime;
        moveDirection = Vector2.zero; // Don't move while waiting

        if (waitAtWaypointTimer >= WAYPOINT_WAIT_TIME)
        {
            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Count;
            SetState(EnemyState.Patrolling);
        }
    }

    private void SetState(EnemyState newState)
    {
        Debug.Log($"[{gameObject.name}] State changing from {currentState} to {newState}");

        // Exit current state
        switch (currentState)
        {
            case EnemyState.WaitingAtWaypoint:
                waitAtWaypointTimer = 0f;
                break;
        }

        // Enter new state
        switch (newState)
        {
            case EnemyState.WaitingAtWaypoint:
                waitAtWaypointTimer = 0f;
                moveDirection = Vector2.zero;
                break;
        }

        currentState = newState;
    }

    private bool CanSeePlayer()
    {
        if (playerTransform == null)
            return false;

        // Calculate direction and distance to player
        Vector2 directionToPlayer = playerTransform.position - eyePosition.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Check if player is within detection range
        if (distanceToPlayer > detectionDistance)
        {
            Debug.LogWarning($"[{gameObject.name}] Player out of detection range. Distance: {distanceToPlayer:F2}");
            return false;
        }

        // Calculate view angle
        float angle = Vector2.Angle(spriteRenderer.flipX ? Vector2.left : Vector2.right, directionToPlayer);

        // Check if player is within view angle
        if (angle > viewAngle)
        {
            Debug.LogWarning($"[{gameObject.name}] Player outside view angle. Angle: {angle:F2}");
            return false;
        }

        // Cast a ray to see if there's an obstacle blocking the view
        RaycastHit2D hit = Physics2D.Raycast(
            eyePosition.position,
            directionToPlayer.normalized,
            distanceToPlayer,
            obstacleLayer | playerLayer
        );

        // Draw debug ray if enabled
        if (showDebugRaycasts)
        {
            Debug.DrawRay(
                eyePosition.position,
                directionToPlayer.normalized * distanceToPlayer,
                hit.collider != null && hit.collider.gameObject.layer == Mathf.Log(playerLayer.value, 2) ? Color.green : Color.red
            );
        }

        // Check if we hit the player
        return hit.collider != null && hit.collider.gameObject.layer == Mathf.Log(playerLayer.value, 2);
    }

    private void TryAttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            // Try to get the Health component from the player
            Health playerHealth = playerTransform.GetComponent<Health>();
            if (playerHealth != null)
            {
                Debug.Log($"[{gameObject.name}] Attacking player for {attackDamage} damage!");
                playerHealth.TakeDamage(attackDamage);
                StartCoroutine(HitFlashEffect(playerTransform.GetComponent<SpriteRenderer>()));
                nextAttackTime = Time.time + attackCooldown;
                SetState(EnemyState.Attacking);
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] Player has no Health component!");
            }
        }
    }

    private IEnumerator HitFlashEffect(SpriteRenderer targetRenderer)
    {
        if (targetRenderer != null)
        {
            Color originalColor = targetRenderer.color;
            targetRenderer.color = hitFlashColor;

            yield return new WaitForSeconds(hitFlashDuration);

            if (targetRenderer != null) // Check again in case object was destroyed
            {
                targetRenderer.color = originalColor;
            }
        }
    }

    private void AttackBehavior()
    {
        // Stop moving while attacking
        moveDirection = Vector2.zero;

        // Return to chase mode after attack animation would finish
        // If you have an actual attack animation, you should sync this with its length
        if (Time.time >= nextAttackTime)
        {
            SetState(EnemyState.Chasing);
        }
    }

    private void UpdateAnimations()
    {
        if (animator == null)
            return;

        // Set animations based on state and movement
        animator.SetBool("IsMoving", Mathf.Abs(moveDirection.x) > 0.1f);
        animator.SetBool("IsChasing", currentState == EnemyState.Chasing);
        animator.SetBool("IsAttacking", currentState == EnemyState.Attacking);
    }

    // Adds a patrol point at the specified position
    public void AddPatrolPoint(Vector2 point)
    {
        patrolPoints.Add(point);
    }

    // Removes a patrol point at the specified index
    public void RemovePatrolPoint(int index)
    {
        if (index >= 0 && index < patrolPoints.Count)
        {
            patrolPoints.RemoveAt(index);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw patrol points
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            // Draw patrol points as spheres
            Gizmos.color = patrolPointColor;
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                // Draw point
                Gizmos.DrawSphere(patrolPoints[i], patrolPointSize);

                // Draw index number
#if UNITY_EDITOR
                Handles.Label(patrolPoints[i] + Vector2.up * patrolPointSize * 2, i.ToString());
#endif
            }

            // Draw lines connecting patrol points
            if (patrolPoints.Count >= 2)
            {
                Gizmos.color = new Color(patrolPointColor.r, patrolPointColor.g, patrolPointColor.b, 0.5f);
                for (int i = 0; i < patrolPoints.Count; i++)
                {
                    // Connect to the next point (or back to the first for the last point)
                    Gizmos.DrawLine(
                        patrolPoints[i],
                        patrolPoints[(i + 1) % patrolPoints.Count]
                    );
                }
            }
        }

        // Visualize detection range in the editor
        if (eyePosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(eyePosition.position, detectionDistance);

            // Draw view cone
            if (Application.isPlaying)
            {
                Vector3 rightDir = Quaternion.Euler(0, 0, viewAngle) * (spriteRenderer != null && spriteRenderer.flipX ? Vector3.left : Vector3.right);
                Vector3 leftDir = Quaternion.Euler(0, 0, -viewAngle) * (spriteRenderer != null && spriteRenderer.flipX ? Vector3.left : Vector3.right);
                Gizmos.DrawRay(eyePosition.position, rightDir * detectionDistance);
                Gizmos.DrawRay(eyePosition.position, leftDir * detectionDistance);
            }
            else
            {
                // Draw a generic view cone when not in play mode
                Vector3 rightDir = Quaternion.Euler(0, 0, viewAngle) * Vector3.right;
                Vector3 leftDir = Quaternion.Euler(0, 0, -viewAngle) * Vector3.right;
                Gizmos.DrawRay(eyePosition != null ? eyePosition.position : transform.position, rightDir * detectionDistance);
                Gizmos.DrawRay(eyePosition != null ? eyePosition.position : transform.position, leftDir * detectionDistance);
            }
        }

        // Visualize ground check radius
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Draw attack range
        if (Application.isPlaying && hasSeenPlayer)
        {
            Gizmos.color = attackRangeColor;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyController))]
public class EnemyControllerEditor : Editor
{
    private EnemyController enemyController;
    private bool isAddingPoints = false;
    private int selectedPointIndex = -1;
    private SerializedProperty patrolPointsProp;
    private SerializedProperty patrolPointSizeProp;

    private void OnEnable()
    {
        enemyController = (EnemyController)target;
        patrolPointsProp = serializedObject.FindProperty("patrolPoints");
        patrolPointSizeProp = serializedObject.FindProperty("patrolPointSize");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Patrol Point Editor", EditorStyles.boldLabel);

        // Toggle for adding points mode
        isAddingPoints = GUILayout.Toggle(isAddingPoints, "Add Points Mode", "Button");

        // Display instructions based on mode
        EditorGUILayout.HelpBox(
            isAddingPoints
                ? "Click in the Scene view to add patrol points."
                : "Click on points in the Scene view to select them. Press Delete to remove selected points.",
            MessageType.Info);

        // Display current selection
        if (selectedPointIndex != -1)
        {
            EditorGUILayout.LabelField($"Selected Point: {selectedPointIndex}");

            if (GUILayout.Button("Delete Selected Point"))
            {
                enemyController.RemovePatrolPoint(selectedPointIndex);
                selectedPointIndex = -1;
                SceneView.RepaintAll();
            }
        }
    }

    private void OnSceneGUI()
    {
        Event e = Event.current;

        // Handle mouse events in scene view
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (isAddingPoints)
            {
                // Add a new point at mouse position
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Vector3 mousePosition = ray.origin;
                Vector2 point = new Vector2(mousePosition.x, mousePosition.y);

                Undo.RecordObject(enemyController, "Add Patrol Point");
                enemyController.AddPatrolPoint(point);

                e.Use();
                SceneView.RepaintAll();
            }
            else
            {
                // Select a point
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Vector3 mousePosition = ray.origin;
                float selectRadius = patrolPointSizeProp.floatValue * 2;

                selectedPointIndex = -1;
                for (int i = 0; i < patrolPointsProp.arraySize; i++)
                {
                    SerializedProperty pointProp = patrolPointsProp.GetArrayElementAtIndex(i);
                    Vector2 point = pointProp.vector2Value;

                    if (Vector2.Distance(new Vector2(mousePosition.x, mousePosition.y), point) < selectRadius)
                    {
                        selectedPointIndex = i;
                        break;
                    }
                }

                SceneView.RepaintAll();
            }
        }

        // Handle key events for deleting points
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete && selectedPointIndex != -1)
        {
            Undo.RecordObject(enemyController, "Delete Patrol Point");
            enemyController.RemovePatrolPoint(selectedPointIndex);
            selectedPointIndex = -1;
            e.Use();
            SceneView.RepaintAll();
        }

        // Draw handles for point manipulation
        for (int i = 0; i < patrolPointsProp.arraySize; i++)
        {
            SerializedProperty pointProp = patrolPointsProp.GetArrayElementAtIndex(i);
            Vector2 point = pointProp.vector2Value;

            // Draw a movable handle for the point
            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = Handles.PositionHandle(new Vector3(point.x, point.y, 0), Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(enemyController, "Move Patrol Point");
                pointProp.vector2Value = new Vector2(newPosition.x, newPosition.y);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif