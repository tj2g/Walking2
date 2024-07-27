using UnityEngine;

public class TooltipFacingCamera : MonoBehaviour
{
    private Camera mainCamera;
    private Transform parentTransform;

    private void Start()
    {
        mainCamera = Camera.main;
        parentTransform = transform.parent;
    }

    private void LateUpdate()
    {
        if (mainCamera != null && parentTransform != null)
        {
            // Ensure the tooltip follows the parent's position but maintains its own rotation
            transform.position = parentTransform.position + Vector3.up * 0.5f; // Adjust the position offset as needed
            transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            eulerAngles.y = -90f; // Set Y rotation
            eulerAngles.z = -90f; // Set Z rotation
            eulerAngles.x = 90f; // Set x rotation
            transform.rotation = Quaternion.Euler(eulerAngles);
        }
    }
}
