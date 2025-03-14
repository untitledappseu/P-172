using UnityEngine;

// Add this script to any GameObject to test audio
public class AudioTester : MonoBehaviour
{
    [SerializeField] private AudioClip testSound;
    [SerializeField] private SoundType soundTypeToTest = SoundType.BulletShoot;
    [SerializeField] private KeyCode testDirectPlayKey = KeyCode.T;
    [SerializeField] private KeyCode testSoundTypeKey = KeyCode.Y;

    private void Update()
    {
        // Test direct sound play
        if (Input.GetKeyDown(testDirectPlayKey) && testSound != null)
        {
            Debug.Log("Testing direct sound play...");
            if (AudioManager.Instance != null)
            {
                Debug.Log("AudioManager instance found, playing sound directly");
                AudioManager.Instance.PlaySound(testSound, transform.position, 1.0f);
            }
            else
            {
                Debug.LogError("AudioManager instance not found!");
            }
        }

        // Test sound type play
        if (Input.GetKeyDown(testSoundTypeKey))
        {
            Debug.Log("Testing sound type play: " + soundTypeToTest);
            if (AudioManager.Instance != null)
            {
                Debug.Log("AudioManager instance found, playing sound type");
                AudioManager.Instance.PlaySound(soundTypeToTest, transform.position);
            }
            else
            {
                Debug.LogError("AudioManager instance not found!");
            }
        }
    }
}