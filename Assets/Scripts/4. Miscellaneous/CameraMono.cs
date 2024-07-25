using UnityEngine;

public class CameraMono : MonoBehaviour
{
    public static UnityEngine.Camera Camera;
    public void Awake()
    {
        Camera = GetComponent<Camera>();
    }
}
