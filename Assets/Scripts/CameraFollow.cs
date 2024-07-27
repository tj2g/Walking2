using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 1f;
    public Vector3 offset;
    public Vector3 runningOffset;
    public float zoomSpeed = 5f; // Speed of the zoom transition

    private Vector3 desiredOffset; // To store the current desired offset
    private Camera cam;
    private Vector3 originalPosition; // Store the original position of the camera for shaking

    void Start()
    {
        desiredOffset = offset; // Start with the original offset
        cam = GetComponent<Camera>();
        originalPosition = transform.localPosition; // Store original local position for shaking
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + desiredOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    public void SetRunning(bool isRunning)
    {
        desiredOffset = isRunning ? runningOffset : offset;
        UpdateCameraZoom();
    }

    private void UpdateCameraZoom()
    {
        float targetFOV = desiredOffset == runningOffset ? 70f : 60f; // Adjust FOV values as needed
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }

    public void ShakeCamera(float duration = 0.2f, float magnitude = 0.05f)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;
        Vector3 originalPos = transform.localPosition;

        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-5f, 5f) * magnitude;
            float y = originalPos.y + Random.Range(-5f, 5f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
