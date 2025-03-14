using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sound types for the game
public enum SoundType
{
    BulletShoot,
    BulletImpact,
    PlayerFootstep,
    PlayerLanding,
    PlayerJump,
    EnemyHit
}

public class AudioManager : MonoBehaviour
{
    // Singleton instance
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();

                if (_instance == null)
                {
                    Debug.LogWarning("No AudioManager found in scene. Creating one...");
                    GameObject obj = new GameObject("AudioManager");
                    _instance = obj.AddComponent<AudioManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }

    // Sound group class - holds a set of audio clips for a specific sound type
    [System.Serializable]
    public class SoundGroup
    {
        public SoundType type;
        public AudioClip[] clips;
        [Range(0f, 5f)] public float volume = 3f; // Default to 300% volume for all sounds
        [Range(0f, 0.5f)] public float pitchVariation = 0.1f;
    }

    // Make this public so it can be configured in the inspector
    [Header("Sound Configuration")]
    public List<SoundGroup> soundGroups = new List<SoundGroup>();

    [Header("Global Volume Settings")]
    [Range(0f, 5f)] public float masterVolume = 1f;
    [Range(0f, 5f)] public float sfxVolume = 1f;

    // Dictionary for quick lookup of sound groups
    private Dictionary<SoundType, SoundGroup> soundDictionary = new Dictionary<SoundType, SoundGroup>();

    // Audio source pool
    private List<AudioSource> audioSources = new List<AudioSource>();
    [SerializeField] private int initialPoolSize = 10;

    private void Awake()
    {
        // Singleton pattern setup
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AudioManager initialized as singleton");

            // Initialize the sound dictionary
            InitializeSoundDictionary();
        }
        else if (_instance != this)
        {
            Debug.Log("Destroying duplicate AudioManager");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize the audio source pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateAudioSource();
        }

        Debug.Log($"AudioManager started with {initialPoolSize} audio sources");
        Debug.Log($"Sound groups configured: {soundGroups.Count}");

        // Log the configured sound groups
        foreach (var group in soundGroups)
        {
            int clipCount = group.clips != null ? group.clips.Length : 0;
            Debug.Log($"Sound group: {group.type}, Clips: {clipCount}, Volume: {group.volume}");
        }
    }

    // Initialize or refresh the sound dictionary
    public void InitializeSoundDictionary()
    {
        soundDictionary.Clear();

        foreach (SoundGroup group in soundGroups)
        {
            if (group.clips != null && group.clips.Length > 0)
            {
                if (!soundDictionary.ContainsKey(group.type))
                {
                    soundDictionary.Add(group.type, group);
                    Debug.Log($"Added sound group: {group.type} with {group.clips.Length} clips");
                }
                else
                {
                    Debug.LogWarning($"Duplicate sound type: {group.type}");
                }
            }
            else
            {
                Debug.LogWarning($"Sound group {group.type} has no clips assigned");
            }
        }

        Debug.Log($"Sound dictionary initialized with {soundDictionary.Count} sound types");
    }

    private AudioSource CreateAudioSource()
    {
        GameObject audioObj = new GameObject("AudioSource_" + audioSources.Count);
        audioObj.transform.parent = transform;
        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
        audioSources.Add(audioSource);
        return audioSource;
    }

    private AudioSource GetAvailableAudioSource()
    {
        // Find an audio source that's not playing
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // If all are in use, create a new one
        Debug.Log("Creating additional audio source because all are in use");
        return CreateAudioSource();
    }

    // Play a sound by type
    public void PlaySound(SoundType type, Vector3 position, float volumeMultiplier = 1f)
    {
        if (soundDictionary.TryGetValue(type, out SoundGroup group))
        {
            if (group.clips != null && group.clips.Length > 0)
            {
                // Select a random clip
                AudioClip clip = group.clips[Random.Range(0, group.clips.Length)];

                // Calculate final volume (group volume * multiplier * master volume * sfx volume)
                float finalVolume = group.volume * volumeMultiplier * masterVolume * sfxVolume;

                // Calculate pitch with variation
                float pitch = 1f + Random.Range(-group.pitchVariation, group.pitchVariation);

                // Get an audio source and play
                AudioSource source = GetAvailableAudioSource();
                source.clip = clip;
                source.volume = finalVolume;
                source.pitch = pitch;
                source.transform.position = position;
                source.Play();

                Debug.Log($"Playing sound: {type}, Clip: {clip.name}, Volume: {finalVolume} (base: {group.volume}, multiplier: {volumeMultiplier}, master: {masterVolume}, sfx: {sfxVolume})");
            }
            else
            {
                Debug.LogWarning($"No clips found for sound type: {type}");
            }
        }
        else
        {
            Debug.LogWarning($"Sound type not found in dictionary: {type}");
        }
    }

    // Play a specific sound clip
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("Attempted to play null audio clip");
            return;
        }

        // Apply master and sfx volume
        float finalVolume = volume * masterVolume * sfxVolume;

        AudioSource source = GetAvailableAudioSource();
        source.clip = clip;
        source.volume = finalVolume;
        source.pitch = pitch;
        source.transform.position = position;
        source.Play();

        Debug.Log($"Playing direct clip: {clip.name}, Volume: {finalVolume} (base: {volume}, master: {masterVolume}, sfx: {sfxVolume})");
    }

    // Method to set volume for a specific sound type
    public void SetSoundTypeVolume(SoundType type, float volume)
    {
        if (soundDictionary.TryGetValue(type, out SoundGroup group))
        {
            group.volume = volume;
            Debug.Log($"Set volume for {type} to {volume}");
        }
        else
        {
            Debug.LogWarning($"Cannot set volume - sound type not found: {type}");
        }
    }

    // Method to set master volume
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        Debug.Log($"Master volume set to {volume}");
    }

    // Method to set SFX volume
    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        Debug.Log($"SFX volume set to {volume}");
    }
}