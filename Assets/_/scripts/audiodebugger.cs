using UnityEngine;
using System;

// Add this script to any GameObject to debug audio issues
public class AudioDebugger : MonoBehaviour
{
    [SerializeField] private KeyCode debugKey = KeyCode.F1;

    private void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            DebugAudioManager();
        }
    }

    private void DebugAudioManager()
    {
        Debug.Log("=== AUDIO MANAGER DEBUG ===");

        // Check if AudioManager exists
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager instance not found! Create an AudioManager GameObject.");
            return;
        }

        Debug.Log("AudioManager found ✓");

        // Get the sound groups using reflection
        var soundGroupsField = typeof(AudioManager).GetField("soundGroups",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        if (soundGroupsField == null)
        {
            Debug.LogError("Could not access soundGroups field via reflection");
            return;
        }

        var soundGroups = soundGroupsField.GetValue(AudioManager.Instance) as System.Collections.Generic.List<AudioManager.SoundGroup>;

        if (soundGroups == null)
        {
            Debug.LogError("Sound groups list is null");
            return;
        }

        Debug.Log($"Sound groups configured: {soundGroups.Count}");

        // Check each sound type in the enum
        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            bool found = false;
            AudioManager.SoundGroup foundGroup = null;

            foreach (var group in soundGroups)
            {
                if (group.type == type)
                {
                    found = true;
                    foundGroup = group;
                    break;
                }
            }

            if (found && foundGroup != null)
            {
                int clipCount = foundGroup.clips != null ? foundGroup.clips.Length : 0;
                bool hasClips = clipCount > 0;

                if (hasClips)
                {
                    Debug.Log($"✓ {type}: Configured with {clipCount} clips, Volume: {foundGroup.volume}");
                }
                else
                {
                    Debug.LogWarning($"⚠ {type}: Configured but has NO CLIPS assigned");
                }
            }
            else
            {
                Debug.LogError($"✗ {type}: NOT CONFIGURED in AudioManager");
            }
        }

        // Check if PlayerController is using AudioManager correctly
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        if (players.Length == 0)
        {
            Debug.LogWarning("No PlayerController found in scene");
        }
        else
        {
            Debug.Log($"Found {players.Length} PlayerController(s) ✓");
        }

        Debug.Log("=== DEBUG COMPLETE ===");
    }

    // Test all sound types
    [ContextMenu("Test All Sound Types")]
    public void TestAllSoundTypes()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager instance not found!");
            return;
        }

        Debug.Log("Testing all sound types...");

        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            Debug.Log($"Testing sound: {type}");
            AudioManager.Instance.PlaySound(type, transform.position);
            // Add a small delay between sounds
        }
    }
}