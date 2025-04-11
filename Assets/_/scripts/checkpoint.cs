using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool isStartingCheckpoint = false;
    [SerializeField] private GameObject activationEffect;

    private bool isActivated = false;
    private CheckpointManager manager;

    private void Start()
    {
        manager = FindObjectOfType<CheckpointManager>();

        if (isStartingCheckpoint && manager != null)
        {
            manager.SetCurrentCheckpoint(this);
            ActivateCheckpoint();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated && manager != null)
        {
            manager.SetCurrentCheckpoint(this);
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        isActivated = true;

        if (activationEffect != null)
        {
            activationEffect.SetActive(true);
        }
    }

    public void DeactivateCheckpoint()
    {
        isActivated = false;

        if (activationEffect != null)
        {
            activationEffect.SetActive(false);
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}