using UnityEngine;
using System;

public class FootCollider : MonoBehaviour
{
    public float kickForce = 1000f; // Force applied to objects when kicked
    public AudioClip impactSound; // Sound played when impact occurs
    private AudioSource audioSource; // Reference to the player's impact audio source

    // Define delegate and event for impact action
    public event Action OnImpact;

    void Start()
    {
        // Try to find the player's impact audio source
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            audioSource = playerController.GetImpactAudioSource();
        }
        else
        {
            Debug.LogError("PlayerController not found or audio source not assigned.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kickable"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate kick direction (forward from the collider's perspective)
                Vector3 kickDirection = transform.forward;

                // Apply force to the object
                rb.AddForce(kickDirection * kickForce, ForceMode.Impulse);

                // Trigger the OnImpact event
                OnImpact?.Invoke();

                // Play impact sound
                if (impactSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(impactSound);
                }
                else
                {
                    Debug.LogError("Impact sound or audio source not assigned.");
                }
            }
        }
    }

    T FindFirstObjectByType<T>()
    {
        foreach (var obj in FindObjectsOfType(typeof(T)))
        {
            if (obj is T result)
            {
                return result;
            }
        }
        return default(T);
    }
}
