using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public SpriteRenderer crosshair; // Reference to the crosshair SpriteRenderer

    public float maxCrosshairSize = 2.0f; // Maximum size of the crosshair
    public float minCrosshairSize = 1.0f; // Minimum size of the crosshair
    public float resizeSpeed = 5.0f; // Speed at which the crosshair resizes

    private Vector3 targetScale; // Target scale of the crosshair

    private void Start()
    {
        crosshair.enabled = false; // Hide crosshair initially
        crosshair.transform.localScale = Vector3.one * maxCrosshairSize; // Start with the maximum size
        targetScale = crosshair.transform.localScale;
        Debug.Log("Crosshair initialized and hidden.");
    }

    private void Update()
    {
        if (crosshair.enabled)
        {
            // Smoothly resize the crosshair to the target scale
            crosshair.transform.localScale = Vector3.Lerp(crosshair.transform.localScale, targetScale, Time.deltaTime * resizeSpeed);
        }
    }

    public void ShowCrosshair()
    {
        crosshair.enabled = true; // Show crosshair
        Debug.Log("Crosshair shown.");
    }

    public void HideCrosshair()
    {
        crosshair.enabled = false; // Hide crosshair
        Debug.Log("Crosshair hidden.");
    }

    public void SetCrosshairSize(bool isMoving)
    {
        if (isMoving)
        {
            targetScale = Vector3.one * maxCrosshairSize; // Set target scale to maximum size when moving
        }
        else
        {
            targetScale = Vector3.one * minCrosshairSize; // Set target scale to minimum size when not moving
        }
    }
}
