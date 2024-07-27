using UnityEngine;
using UnityEngine.SceneManagement; // Import SceneManager to manage scenes
using TMPro;

public class InteractiveController : MonoBehaviour
{
    [Header("Tooltip Controller Settings")]
    public Transform player; // Reference to the player's transform
    public Transform eInteractable; // Reference to the E interactable object
    public Transform qInteractable; // Reference to the Q interactable object
    public float showDistance = 3f; // Distance at which the tooltip starts fading in
    public float fadeSpeed = 5f; // Speed at which the tooltip fades in/out
    public float eInteractionCooldown = 1f; // Cooldown duration after interaction with E
    public float qInteractionCooldown = 1f; // Cooldown duration after interaction with Q

    [Header("Item Tooltip Settings")]
    public float itemShowDistance = 3f; // Distance at which the item tooltip starts fading in

    [Header("Message Display Settings")]
    public TMP_Text messageText; // Reference to the TextMeshPro Text component for messages
    public float messageFadeSpeed = 2f; // Speed at which the message fades in/out
    public float messageDisplayTime = 3f; // Duration for which the message is displayed
    public string initialMessage = "Welcome to the game!"; // Initial message to display

    [Header("Exit Manager Settings")]
    // Exit Manager functionality (No fields needed for this section)

    private TMP_Text eTooltipText; // Reference to the TextMeshPro Text component of the E tooltip
    private TMP_Text qTooltipText; // Reference to the TextMeshPro Text component of the Q tooltip
    private CanvasGroup eCanvasGroup; // Reference to the CanvasGroup component for E tooltip
    private CanvasGroup qCanvasGroup; // Reference to the CanvasGroup component for Q tooltip
    private bool eInteracted; // Flag to track if 'E' interactable was interacted with
    private bool qInteracted; // Flag to track if 'Q' interactable was interacted with
    private float eLastInteractionTime; // Time when last interaction with E occurred
    private float qLastInteractionTime; // Time when last interaction with Q occurred

    private CanvasGroup messageCanvasGroup; // Reference to the CanvasGroup component for message display
    private bool isMessageDisplaying; // Flag to track if a message is being displayed
    private float messageDisplayStartTime; // Time when the message display started

    private void Start()
    {
        // Initialize Tooltip Controller components
        if (eInteractable != null)
        {
            eTooltipText = eInteractable.GetComponentInChildren<TMP_Text>();
            eCanvasGroup = eInteractable.GetComponent<CanvasGroup>();
        }

        if (qInteractable != null)
        {
            qTooltipText = qInteractable.GetComponentInChildren<TMP_Text>();
            qCanvasGroup = qInteractable.GetComponent<CanvasGroup>();
        }

        // Initialize tooltips as invisible
        if (eCanvasGroup != null)
        {
            eCanvasGroup.alpha = 0f;
        }

        if (qCanvasGroup != null)
        {
            qCanvasGroup.alpha = 0f;
        }

        // Initialize Message Display components
        if (messageText != null)
        {
            messageCanvasGroup = messageText.GetComponent<CanvasGroup>();
            if (messageCanvasGroup == null)
            {
                messageCanvasGroup = messageText.gameObject.AddComponent<CanvasGroup>();
            }

            // Display the initial message at start
            DisplayMessage(initialMessage);
        }
    }

    private void Update()
    {
        // Calculate distance between player and interactables
        if (eInteractable != null)
        {
            float eDistance = Vector3.Distance(player.position, eInteractable.position);
            FadeTooltip(eCanvasGroup, eTooltipText, eDistance);

            if (Input.GetKeyDown(KeyCode.E) && eDistance <= showDistance && !eInteracted && Time.time >= eLastInteractionTime + eInteractionCooldown)
            {
                eInteracted = true;
                HideTooltip(eCanvasGroup);
                eLastInteractionTime = Time.time;
            }

            if (Time.time >= eLastInteractionTime + eInteractionCooldown)
            {
                eInteracted = false;
            }
        }

        if (qInteractable != null)
        {
            float qDistance = Vector3.Distance(player.position, qInteractable.position);
            FadeTooltip(qCanvasGroup, qTooltipText, qDistance);

            if (Input.GetKeyDown(KeyCode.Q) && qDistance <= showDistance && !qInteracted && Time.time >= qLastInteractionTime + qInteractionCooldown)
            {
                qInteracted = true;
                HideTooltip(qCanvasGroup);
                qLastInteractionTime = Time.time;
            }

            if (Time.time >= qLastInteractionTime + qInteractionCooldown)
            {
                qInteracted = false;
            }
        }

        // Handle message display fade-out
        if (isMessageDisplaying)
        {
            if (Time.time >= messageDisplayStartTime + messageDisplayTime)
            {
                messageCanvasGroup.alpha = Mathf.Lerp(messageCanvasGroup.alpha, 0f, Time.deltaTime * messageFadeSpeed);
                if (messageCanvasGroup.alpha <= 0.01f)
                {
                    isMessageDisplaying = false;
                    messageCanvasGroup.alpha = 0f; // Ensure it's fully hidden
                }
            }
        }

        // Display message when pressing 'I'
        if (Input.GetKeyDown(KeyCode.I))
        {
            DisplayMessage("\"E\" to interact, \"Q\" to Kick, \"T\" to Drop Gun, \"Right MB\" to Aim, \"I\" for Help!");
        }

        // Handle item tooltips
        HandleItemTooltips();
    }

    // Method to handle item tooltips
    private void HandleItemTooltips()
    {
        foreach (var item in FindObjectsOfType<Transform>()) // Using Transform as the generic type to find all items
        {
            var itemTooltip = item.GetComponentInChildren<CanvasGroup>();
            if (itemTooltip != null && item.CompareTag("Item")) // Ensure the item has the tag "Item"
            {
                float distance = Vector3.Distance(player.position, item.position);
                itemTooltip.alpha = distance <= itemShowDistance ? Mathf.Lerp(itemTooltip.alpha, 1f, Time.deltaTime * fadeSpeed) : Mathf.Lerp(itemTooltip.alpha, 0f, Time.deltaTime * fadeSpeed);
            }
        }
    }

    // Method to display a message
    public void DisplayMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageCanvasGroup.alpha = 1f;
            isMessageDisplaying = true;
            messageDisplayStartTime = Time.time;
        }
    }

    // Method to fade in/out tooltip based on distance
    private void FadeTooltip(CanvasGroup canvasGroup, TMP_Text tooltipText, float distance)
    {
        if (canvasGroup != null && tooltipText != null)
        {
            if (distance <= showDistance && !(eInteracted && canvasGroup == eCanvasGroup) && !(qInteracted && canvasGroup == qCanvasGroup))
            {
                // Show tooltip
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, Time.deltaTime * fadeSpeed);
            }
            else
            {
                // Hide tooltip
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
            }
        }
    }

    // Method to hide the tooltip
    private void HideTooltip(CanvasGroup canvasGroup)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f; // Hide tooltip
        }
    }

    // Exit Manager functionality
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit(); // This will quit the application when in build, but not in the editor
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // This line reloads the current scene, effectively restarting the game
    }
}
