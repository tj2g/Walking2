using UnityEngine;

public class TriggerSoundPlayer : MonoBehaviour
{
    public AudioClip[] voiceClips; // Array to hold the audio clips
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on the GameObject.");
        }

        if (voiceClips.Length == 0)
        {
            Debug.LogError("No audio clips assigned to the voiceClips array.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check for specific tags or other conditions if needed
        // For example, if (other.CompareTag("Player"))

        int triggerIndex = GetTriggerIndex(other.gameObject);

        if (triggerIndex >= 0 && triggerIndex < voiceClips.Length)
        {
            PlaySound(triggerIndex);
        }
    }

    private int GetTriggerIndex(GameObject triggerObject)
    {
        // Implement logic to determine which audio clip to play based on the triggerObject
        // For example, you can use the name of the trigger object
        switch (triggerObject.name)
        {
            case "TriggerZone1":
                return 0;
            case "TriggerZone2":
                return 1;
            case "TriggerZone3":
                return 2;
            case "TriggerZone4":
                return 3;
            case "TriggerZone5":
                return 4;
            case "TriggerZone6":
                return 5;
            case "TriggerZone7":
                return 6;
            default:
                return -1;
        }
    }

    private void PlaySound(int clipIndex)
    {
        if (audioSource != null && voiceClips[clipIndex] != null)
        {
            audioSource.PlayOneShot(voiceClips[clipIndex]);
        }
    }
}
