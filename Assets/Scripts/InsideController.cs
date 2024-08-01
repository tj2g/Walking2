using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InsideController : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject door; // Assign the door GameObject in the Inspector
    public float kickForce = 1000f;
    public float kickTorque = 300f;
    public AudioClip kickSound;
    public AudioClip closeSound;
    public AudioClip openSound;
    private AudioSource doorAudioSource;
    private Rigidbody doorRigidbody;
    private HingeJoint hinge;
    private bool isDoorOpen = false;
    private Vector3 closedDoorPosition;
    private Quaternion closedDoorRotation;

    [Header("Fog of War Settings")]
    public GameObject fogOfWar;
    private MeshRenderer fogRenderer;
    private bool isFogRevealed = false;

    [Header("Police Knocking and Voices Settings")]
    public AudioClip[] knockingSounds;
    public AudioClip[] policeVoices;
    private List<int> availableVoiceClips;
    private int totalVoiceClips;
    public float voiceIntervalMin = 15f;
    public float voiceIntervalMax = 30f;
    public float knockingInterval = 10f;

    private AudioSource policeAudioSource;

    private bool isInteracting = false;
    private float interactionCooldown = 1.0f;

    private void Start()
    {
        // Door initialization
        if (door != null)
        {
            doorRigidbody = door.GetComponent<Rigidbody>();
            hinge = door.GetComponent<HingeJoint>();
            doorAudioSource = door.GetComponent<AudioSource>();

            if (doorRigidbody != null && hinge != null)
            {
                doorRigidbody.isKinematic = true;
                hinge.useSpring = false;
                closedDoorPosition = door.transform.position;
                closedDoorRotation = door.transform.rotation;
            }
            else
            {
                Debug.LogError("Door components missing. Make sure the door has a Rigidbody and HingeJoint.");
            }
        }

        // Fog of war initialization
        if (fogOfWar != null)
        {
            fogRenderer = fogOfWar.GetComponent<MeshRenderer>();
            if (fogRenderer != null)
            {
                fogRenderer.enabled = true;
            }
            else
            {
                Debug.LogError("Fog of War MeshRenderer component missing.");
            }
        }

        // Police knocking and voices initialization
        policeAudioSource = GetComponent<AudioSource>();
        if (policeAudioSource == null)
        {
            policeAudioSource = gameObject.AddComponent<AudioSource>();
        }

        totalVoiceClips = policeVoices.Length;
        ResetVoiceClipIndices();
        InvokeRepeating(nameof(PlayKnockingSound), 0f, knockingInterval);
        Invoke(nameof(PlayRandomVoice), Random.Range(voiceIntervalMin, voiceIntervalMax));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isInteracting && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Door collision detected with player.");
            ToggleDoor();
        }
    }

    public void ToggleDoor()
    {
        if (!isInteracting)
        {
            isInteracting = true;
            if (isDoorOpen)
            {
                Debug.Log("Closing the door.");
                CloseDoor();
            }
            else
            {
                Debug.Log("Opening the door.");
                KickDoor(transform.forward);
            }
            StartCoroutine(ResetInteractionCooldown());
        }
    }

    private IEnumerator ResetInteractionCooldown()
    {
        yield return new WaitForSeconds(interactionCooldown);
        isInteracting = false;
    }

    public void KickDoor(Vector3 kickDirection)
    {
        if (!isDoorOpen && doorRigidbody != null)
        {
            isDoorOpen = true;
            doorRigidbody.isKinematic = false;
            doorRigidbody.AddForce(kickDirection * kickForce, ForceMode.Impulse);
            doorRigidbody.AddTorque(Vector3.up * kickTorque, ForceMode.Impulse);
            PlaySound(doorAudioSource, kickSound);
            PlaySound(doorAudioSource, openSound);
            if (fogOfWar != null)
            {
                RevealFog();
            }
            Debug.Log("Door kicked open.");
        }
        else
        {
            Debug.Log("Door is already open or Rigidbody is missing.");
        }
    }

    private void CloseDoor()
    {
        if (isDoorOpen)
        {
            isDoorOpen = false;
            StartCoroutine(CloseDoorCoroutine());
        }
    }

    private IEnumerator CloseDoorCoroutine()
    {
        float elapsedTime = 0f;
        float duration = 1f;
        Vector3 startPosition = door.transform.position;
        Quaternion startRotation = door.transform.rotation;

        while (elapsedTime < duration)
        {
            door.transform.position = Vector3.Lerp(startPosition, closedDoorPosition, elapsedTime / duration);
            door.transform.rotation = Quaternion.Lerp(startRotation, closedDoorRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        door.transform.position = closedDoorPosition;
        door.transform.rotation = closedDoorRotation;
        doorRigidbody.isKinematic = true;
        PlaySound(doorAudioSource, closeSound);
        Debug.Log("Door closed.");
    }

    private void PlaySound(AudioSource source, AudioClip clip)
    {
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip);
        }
    }

    // Fog of war methods
    public void RevealFog()
    {
        if (!isFogRevealed && fogRenderer != null)
        {
            fogRenderer.enabled = false;
            isFogRevealed = true;
        }
    }

    public void HideFog()
    {
        if (isFogRevealed && fogRenderer != null)
        {
            fogRenderer.enabled = true;
            isFogRevealed = false;
        }
    }

    // Police knocking and voices methods
    private void ResetVoiceClipIndices()
    {
        availableVoiceClips = new List<int>();
        for (int i = 0; i < totalVoiceClips; i++)
        {
            availableVoiceClips.Add(i);
        }
    }

    private void PlayKnockingSound()
    {
        int randomIndex = Random.Range(0, knockingSounds.Length);
        PlaySound(policeAudioSource, knockingSounds[randomIndex]);
    }

    private void PlayRandomVoice()
    {
        if (availableVoiceClips.Count == 0)
        {
            ResetVoiceClipIndices();
        }

        int randomIndex = Random.Range(0, availableVoiceClips.Count);
        int clipIndex = availableVoiceClips[randomIndex];
        availableVoiceClips.RemoveAt(randomIndex);

        PlaySound(policeAudioSource, policeVoices[clipIndex]);

        Invoke(nameof(PlayRandomVoice), Random.Range(voiceIntervalMin, voiceIntervalMax));
    }
}
