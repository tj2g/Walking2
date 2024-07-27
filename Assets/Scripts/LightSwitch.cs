using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public GameObject[] lightCircuits; // Array of parent objects containing the lights for different circuits
    public KeyCode toggleKey = KeyCode.E; // The key to press to toggle the light
    public float interactionDistance = 0.1f; // Maximum distance from which the switch can be interacted with
    public AudioClip switchSound; // Sound to play when toggling the lights

    private Transform player;
    private Light[][] lights;
    private AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize the lights array with the correct size
        lights = new Light[lightCircuits.Length][];

        // Iterate through each light circuit and populate the lights array
        for (int i = 0; i < lightCircuits.Length; i++)
        {
            lights[i] = lightCircuits[i].GetComponentsInChildren<Light>();
        }

        // Ensure there is an AudioSource component on the GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Check if the player is within interaction distance and presses the toggle key
        if (Vector3.Distance(player.position, transform.position) <= interactionDistance && Input.GetKeyDown(toggleKey))
        {
            ToggleLights();
        }
    }

    void ToggleLights()
    {
        // Iterate through each light circuit and toggle their lights
        for (int i = 0; i < lights.Length; i++)
        {
            foreach (Light light in lights[i])
            {
                light.enabled = !light.enabled;
            }
        }

        // Play the switch sound effect if assigned
        if (switchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(switchSound);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to visualize the interaction distance in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
