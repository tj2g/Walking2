using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the projectile after its lifetime expires
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // Move the projectile forward
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision logic here (e.g., damage to target)
        Destroy(gameObject); // Destroy the projectile on collision
    }
}
