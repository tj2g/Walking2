using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GunController : MonoBehaviour
{
    [Header("Gun Collision Settings")]
    public AudioClip dropSound;
    private AudioSource audioSource;

    [Header("Pickup Settings")]
    public PlayerController playerController;

    [Header("Tooltip Settings")]
    public GameObject tooltip;
    public float tooltipShowDistance = 3f;

    [Header("Gun Image Settings")]
    public Sprite gunImage;

    [Header("Crosshair Settings")]
    public Image crosshair;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform barrel;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    public int maxBullets = 10;
    private int currentBullets;
    public AudioClip[] shootingSounds;
    public float bulletLifetime = 2f;

    [Header("Reloading Settings")]
    public AudioClip reloadSound;
    public float reloadTime = 2f;

    [Header("Aiming Settings")]
    public AudioClip aimSound;

    [Header("Effects")]
    public GameObject muzzleFlashPrefab;
    public GameObject smokePrefab;

    private CanvasGroup tooltipCanvasGroup;
    private bool isReloading = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (crosshair != null)
        {
            crosshair.enabled = false;
        }

        currentBullets = maxBullets;

        if (tooltip != null)
        {
            tooltipCanvasGroup = tooltip.GetComponent<CanvasGroup>();
            if (tooltipCanvasGroup == null)
            {
                tooltipCanvasGroup = tooltip.gameObject.AddComponent<CanvasGroup>();
            }
            tooltipCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogWarning("Tooltip is not assigned in the inspector.");
        }
    }

    private void Update()
    {
        HandleTooltip();
        HandleCrosshair();

        if (isReloading)
            return;

        if (playerController.IsAiming() && Input.GetButton("Fire1") && Time.time >= nextFireTime && currentBullets > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }

        if (playerController.IsAiming() && Input.GetMouseButtonDown(1))
        {
            PlayAimSound();
        }
    }

    private void LateUpdate()
    {
        if (tooltip != null && tooltip.activeSelf)
        {
            UpdateTooltipOrientation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandlePickup(other);
    }

    private void HandleCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            PlayDropSound();
        }
    }

    private void PlayDropSound()
    {
        if (dropSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dropSound);
        }
        else
        {
            Debug.LogWarning("Drop sound or audio source is not assigned.");
        }
    }

    private void HandleTooltip()
    {
        if (tooltip != null && playerController != null)
        {
            float distance = Vector3.Distance(playerController.transform.position, transform.position);
            tooltipCanvasGroup.alpha = distance <= tooltipShowDistance ? Mathf.Lerp(tooltipCanvasGroup.alpha, 1f, Time.deltaTime * 5f) : Mathf.Lerp(tooltipCanvasGroup.alpha, 0f, Time.deltaTime * 5f);
        }
    }

    private void HandlePickup(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController.currentGun == null && playerController.canPickUp)
            {
                AudioSource playerAudioSource = playerController.GetImpactAudioSource();

                if (playerAudioSource != null)
                {
                    playerAudioSource.PlayOneShot(playerController.gunPickupSound);
                }
                else
                {
                    Debug.LogWarning("No audio source found on PlayerController or gunPickupSound is not assigned.");
                }

                if (tooltip != null)
                {
                    Debug.Log("Hiding tooltip on pickup");
                    tooltip.SetActive(false);
                }

                playerController.currentGun = gameObject;
                playerController.UpdateGunImage(gunImage);
                transform.SetParent(playerController.gunHolder);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                if (crosshair != null)
                {
                    crosshair.enabled = true;
                }
            }
        }
    }

    public void DropGun()
    {
        if (playerController.currentGun == gameObject)
        {
            Debug.Log("Dropping the gun");

            transform.SetParent(null);

            if (tooltip != null)
            {
                tooltip.transform.SetParent(null);
                tooltip.transform.position = transform.position + Vector3.up * 0.5f;
                tooltip.SetActive(true);
                tooltipCanvasGroup.alpha = 1f;

                Debug.Log("Reactivating the tooltip");
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(playerController.transform.forward * 2f, ForceMode.Impulse);
            }

            playerController.currentGun = null;
            playerController.UpdateGunImage(null);

            if (crosshair != null)
            {
                crosshair.enabled = false;
            }

            gameObject.SetActive(true);

            HandleTooltip();
        }
    }

    private void HandleCrosshair()
    {
        if (playerController != null && playerController.IsAiming())
        {
            if (crosshair != null)
            {
                crosshair.enabled = true;
            }
        }
        else
        {
            if (crosshair != null)
            {
                crosshair.enabled = false;
            }
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned.");
            return;
        }

        if (barrel == null)
        {
            Debug.LogError("Barrel is not assigned.");
            return;
        }

        if (currentBullets <= 0)
        {
            Debug.Log("No bullets left, reload the gun.");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, barrel.position, barrel.rotation);

        if (projectile == null)
        {
            Debug.LogError("Failed to instantiate projectile.");
            return;
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = barrel.forward * 20f;
        }
        else
        {
            Debug.LogError("Projectile does not have a Rigidbody component.");
        }

        Destroy(projectile, bulletLifetime);

        PlayRandomShootingSound();

        CreateMuzzleFlash();
        CreateSmoke(); // Add smoke effect here

        currentBullets--;

        Debug.Log("Shot fired from barrel. Bullets left: " + currentBullets);
    }

    private void PlayRandomShootingSound()
    {
        if (shootingSounds != null && shootingSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, shootingSounds.Length);
            AudioClip randomClip = shootingSounds[randomIndex];
            audioSource.PlayOneShot(randomClip);
        }
        else
        {
            Debug.LogWarning("Shooting sounds are not assigned or audio source is not available.");
        }
    }

    private IEnumerator Reload()
    {
        if (currentBullets < maxBullets)
        {
            isReloading = true;
            Debug.Log("Reloading...");

            if (reloadSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(reloadSound);
            }

            yield return new WaitForSeconds(reloadTime);

            currentBullets = maxBullets;
            isReloading = false;

            Debug.Log("Reloaded. Bullets: " + currentBullets);
        }
    }

    private void PlayAimSound()
    {
        if (aimSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(aimSound);
        }
        else
        {
            Debug.LogWarning("Aim sound is not assigned or audio source is not available.");
        }
    }

    private void CreateMuzzleFlash()
    {
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, barrel.position, barrel.rotation);
            ParticleSystem ps = muzzleFlash.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Debug.Log("Muzzle flash played");
            }
            Destroy(muzzleFlash, 0.5f); // Adjust the time as needed
        }
    }

    private void CreateSmoke()
    {
        if (smokePrefab != null)
        {
            GameObject smoke = Instantiate(smokePrefab, barrel.position, barrel.rotation);
            ParticleSystem ps = smoke.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Debug.Log("Smoke played");
            }
            Destroy(smoke, 1f); // Adjust the time as needed
        }
    }

    private void UpdateTooltipOrientation()
    {
        tooltip.transform.LookAt(Camera.main.transform);
        Vector3 eulerAngles = tooltip.transform.rotation.eulerAngles;
        eulerAngles.y = 90f;
        eulerAngles.z = 0f;
        tooltip.transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
