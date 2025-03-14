using UnityEngine;

// Add this script to any GameObject to control volume with keyboard shortcuts
public class VolumeControl : MonoBehaviour
{
    [Header("Volume Control Keys")]
    [SerializeField] private KeyCode masterVolumeUpKey = KeyCode.PageUp;
    [SerializeField] private KeyCode masterVolumeDownKey = KeyCode.PageDown;
    [SerializeField] private KeyCode sfxVolumeUpKey = KeyCode.Home;
    [SerializeField] private KeyCode sfxVolumeDownKey = KeyCode.End;

    [Header("Sound Type Volume Controls")]
    [SerializeField] private KeyCode bulletShootVolumeUpKey = KeyCode.F5;
    [SerializeField] private KeyCode bulletShootVolumeDownKey = KeyCode.F6;
    [SerializeField] private KeyCode footstepVolumeUpKey = KeyCode.F7;
    [SerializeField] private KeyCode footstepVolumeDownKey = KeyCode.F8;

    [Header("Settings")]
    [SerializeField] private float volumeStep = 0.25f;
    [SerializeField] private float maxVolume = 5f;

    private void Start()
    {
        Debug.Log("=== VOLUME CONTROL KEYS ===");
        Debug.Log($"Master Volume: {masterVolumeUpKey} (up) / {masterVolumeDownKey} (down)");
        Debug.Log($"SFX Volume: {sfxVolumeUpKey} (up) / {sfxVolumeDownKey} (down)");
        Debug.Log($"Bullet Shoot Volume: {bulletShootVolumeUpKey} (up) / {bulletShootVolumeDownKey} (down)");
        Debug.Log($"Footstep Volume: {footstepVolumeUpKey} (up) / {footstepVolumeDownKey} (down)");
    }

    private void Update()
    {
        if (AudioManager.Instance == null)
            return;

        // Master volume controls
        if (Input.GetKeyDown(masterVolumeUpKey))
        {
            AdjustMasterVolume(volumeStep);
        }
        else if (Input.GetKeyDown(masterVolumeDownKey))
        {
            AdjustMasterVolume(-volumeStep);
        }

        // SFX volume controls
        if (Input.GetKeyDown(sfxVolumeUpKey))
        {
            AdjustSfxVolume(volumeStep);
        }
        else if (Input.GetKeyDown(sfxVolumeDownKey))
        {
            AdjustSfxVolume(-volumeStep);
        }

        // Bullet shoot volume controls
        if (Input.GetKeyDown(bulletShootVolumeUpKey))
        {
            AdjustSoundTypeVolume(SoundType.BulletShoot, volumeStep);
        }
        else if (Input.GetKeyDown(bulletShootVolumeDownKey))
        {
            AdjustSoundTypeVolume(SoundType.BulletShoot, -volumeStep);
        }

        // Footstep volume controls
        if (Input.GetKeyDown(footstepVolumeUpKey))
        {
            AdjustSoundTypeVolume(SoundType.PlayerFootstep, volumeStep);
        }
        else if (Input.GetKeyDown(footstepVolumeDownKey))
        {
            AdjustSoundTypeVolume(SoundType.PlayerFootstep, -volumeStep);
        }
    }

    private void AdjustMasterVolume(float amount)
    {
        float newVolume = Mathf.Clamp(AudioManager.Instance.masterVolume + amount, 0f, maxVolume);
        AudioManager.Instance.SetMasterVolume(newVolume);
        Debug.Log($"Master Volume: {newVolume}");
    }

    private void AdjustSfxVolume(float amount)
    {
        float newVolume = Mathf.Clamp(AudioManager.Instance.sfxVolume + amount, 0f, maxVolume);
        AudioManager.Instance.SetSfxVolume(newVolume);
        Debug.Log($"SFX Volume: {newVolume}");
    }

    private void AdjustSoundTypeVolume(SoundType type, float amount)
    {
        // Get current volume using reflection
        var soundGroupsField = typeof(AudioManager).GetField("soundGroups",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        if (soundGroupsField == null)
            return;

        var soundGroups = soundGroupsField.GetValue(AudioManager.Instance) as System.Collections.Generic.List<AudioManager.SoundGroup>;

        if (soundGroups == null)
            return;

        foreach (var group in soundGroups)
        {
            if (group.type == type)
            {
                float newVolume = Mathf.Clamp(group.volume + amount, 0f, maxVolume);
                AudioManager.Instance.SetSoundTypeVolume(type, newVolume);
                Debug.Log($"{type} Volume: {newVolume}");
                break;
            }
        }
    }
}