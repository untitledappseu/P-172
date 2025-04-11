using UnityEngine;
using System.Collections;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 1f;

    private Checkpoint currentCheckpoint;
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath += RespawnPlayer;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath -= RespawnPlayer;
        }
    }

    public void SetCurrentCheckpoint(Checkpoint checkpoint)
    {
        if (currentCheckpoint != null)
        {
            currentCheckpoint.DeactivateCheckpoint();
        }

        currentCheckpoint = checkpoint;
        SaveCheckpoint();
    }

    private void SaveCheckpoint()
    {
        PlayerPrefs.SetFloat("CheckpointX", currentCheckpoint.GetPosition().x);
        PlayerPrefs.SetFloat("CheckpointY", currentCheckpoint.GetPosition().y);
        PlayerPrefs.SetFloat("CheckpointZ", currentCheckpoint.GetPosition().z);
        PlayerPrefs.Save();
    }

    private Vector3 GetSavedCheckpointPosition()
    {
        float x = PlayerPrefs.GetFloat("CheckpointX", 0f);
        float y = PlayerPrefs.GetFloat("CheckpointY", 0f);
        float z = PlayerPrefs.GetFloat("CheckpointZ", 0f);

        return new Vector3(x, y, z);
    }

    private void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.SetActive(false);

            yield return new WaitForSeconds(respawnDelay);

            if (currentCheckpoint != null)
            {
                player.transform.position = currentCheckpoint.GetPosition();
            }
            else
            {
                player.transform.position = GetSavedCheckpointPosition();
            }

            player.SetActive(true);

            if (playerHealth != null)
            {
                playerHealth.RestoreHealth();
            }
        }
    }
}