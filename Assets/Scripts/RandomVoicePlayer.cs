using UnityEngine;
using System.Collections.Generic;

public class RandomVoicePlayer : MonoBehaviour
{
    public AudioClip[] voiceClips; // Array to hold the audio clips
    private AudioSource audioSource;

    private List<int> clipIndices; // List to track unplayed clips
    private int totalClips; // Total number of clips
    private float playInterval = 20f; // Time interval to play a clip
    private float timer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on the GameObject.");
            return;
        }

        if (voiceClips.Length == 0)
        {
            Debug.LogError("No audio clips assigned to the voiceClips array.");
            return;
        }

        totalClips = voiceClips.Length;
        ResetClipIndices();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= playInterval)
        {
            PlayRandomClip();
            timer = 0f;
        }
    }

    private void ResetClipIndices()
    {
        clipIndices = new List<int>();
        for (int i = 0; i < totalClips; i++)
        {
            clipIndices.Add(i);
        }
    }

    private void PlayRandomClip()
    {
        if (clipIndices.Count == 0)
        {
            ResetClipIndices();
        }

        int randomIndex = Random.Range(0, clipIndices.Count);
        int clipIndex = clipIndices[randomIndex];
        clipIndices.RemoveAt(randomIndex);

        if (audioSource != null && voiceClips[clipIndex] != null)
        {
            audioSource.PlayOneShot(voiceClips[clipIndex]);
        }
    }
}
