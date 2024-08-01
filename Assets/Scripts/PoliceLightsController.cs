using UnityEngine;
using System.Collections;

public class PoliceLightsController : MonoBehaviour
{
    public Light[] spotLights; // Array to hold the spotlights
    public ParticleSystem[] particleEffects; // Array to hold the particle systems
    public Color color1 = Color.red; // First color (e.g., red)
    public Color color2 = Color.blue; // Second color (e.g., blue)
    public float switchInterval = 0.5f; // Time interval between color switches
    public float flashDuration = 0.1f; // Duration for each flash

    void Start()
    {
        if (spotLights.Length == 0)
        {
            Debug.LogError("No spotlights assigned to PoliceLightsController.");
        }
        if (particleEffects.Length == 0)
        {
            Debug.LogWarning("No particle effects assigned to PoliceLightsController.");
        }
        StartCoroutine(FlashLights());
    }

    IEnumerator FlashLights()
    {
        while (true)
        {
            Flash(color1);
            yield return new WaitForSeconds(flashDuration);

            DisableLightsAndParticles();
            yield return new WaitForSeconds(flashDuration);

            Flash(color2);
            yield return new WaitForSeconds(flashDuration);

            DisableLightsAndParticles();
            yield return new WaitForSeconds(switchInterval - (flashDuration * 2));
        }
    }

    void Flash(Color color)
    {
        foreach (Light light in spotLights)
        {
            light.color = color;
            light.enabled = true; // Ensure the light is on
        }
        foreach (ParticleSystem particleSystem in particleEffects)
        {
            var main = particleSystem.main;
            main.startColor = color;
            particleSystem.Play(); // Start the particle effect
        }
    }

    void DisableLightsAndParticles()
    {
        foreach (Light light in spotLights)
        {
            light.enabled = false; // Turn off the light
        }
        foreach (ParticleSystem particleSystem in particleEffects)
        {
            particleSystem.Stop(); // Stop the particle effect
        }
    }
}
