using UnityEngine;

// Add this script to any GameObject to test sounds with keyboard shortcuts
public class AudioTestButtons : MonoBehaviour
{
    [System.Serializable]
    public class SoundTestKey
    {
        public SoundType soundType;
        public KeyCode keyCode;
        public string description;
    }

    [SerializeField]
    private SoundTestKey[] testKeys = new SoundTestKey[]
    {
        new SoundTestKey { soundType = SoundType.BulletShoot, keyCode = KeyCode.Alpha1, description = "Bullet Shoot" },
        new SoundTestKey { soundType = SoundType.BulletImpact, keyCode = KeyCode.Alpha2, description = "Bullet Impact" },
        new SoundTestKey { soundType = SoundType.PlayerFootstep, keyCode = KeyCode.Alpha3, description = "Player Footstep" },
        new SoundTestKey { soundType = SoundType.PlayerLanding, keyCode = KeyCode.Alpha4, description = "Player Landing" },
        new SoundTestKey { soundType = SoundType.PlayerJump, keyCode = KeyCode.Alpha5, description = "Player Jump" },
        new SoundTestKey { soundType = SoundType.EnemyHit, keyCode = KeyCode.Alpha6, description = "Enemy Hit" }
    };

    private void Start()
    {
        Debug.Log("=== AUDIO TEST KEYS ===");
        foreach (var testKey in testKeys)
        {
            Debug.Log($"Press {testKey.keyCode} to test {testKey.description} sound");
        }
    }

    private void Update()
    {
        foreach (var testKey in testKeys)
        {
            if (Input.GetKeyDown(testKey.keyCode))
            {
                TestSound(testKey.soundType, testKey.description);
            }
        }
    }

    private void TestSound(SoundType soundType, string description)
    {
        Debug.Log($"Testing {description} sound...");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(soundType, transform.position);
            Debug.Log($"Played {description} sound at position {transform.position}");
        }
        else
        {
            Debug.LogError("AudioManager instance not found!");
        }
    }
}