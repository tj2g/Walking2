using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public Transform gunTransform; // Reference to the gun's transform
    public Transform gunBarrelEnd; // Reference to the end of the gun barrel
    private bool isAiming = false; // Boolean to check if the player is aiming
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rotationSpeed = 10f;
    public AudioClip[] footstepSounds;
    public float walkFootstepInterval = 0.5f;
    public float runFootstepInterval = 0.3f;
    public AudioSource footstepAudioSource;
    public AudioClip kickSound;
    public AudioClip impactSound;
    private AudioSource kickAudioSource;
    private AudioSource impactAudioSource;
    public GameObject footColliderPrefab;
    private Rigidbody rb;
    private Camera mainCamera;
    private float nextFootstepTime;
    private Animator animator;
    private GameObject currentFootCollider;
    public PoliceLightsController policeLightsController;
    public CrosshairController crosshairController; // Reference to the CrosshairController
    private bool isMoving;
    public CameraFollow cameraFollow;
    public float kickForce = 10f;

    public GameObject currentGun;
    public GameObject gunPrefab;
    public Transform gunHolder;
    public RectTransform UnarmedSlot;
    public RectTransform EquippedSlot;

    public float normalFOV = 60f;
    public float zoomedFOV = 65f;
    public float zoomSpeed = 5f;
    public Transform gunBone; // Assign the thumb bone or transform in the Inspector
    public Sprite defaultGunIcon;
    public Sprite revolverIcon;
    public Sprite rifleIcon;
    private Image equippedIconImage;
    private float dropCooldown = 2f;
    public bool canPickUp = true;
    private float lastDropTime;
    private bool isStrafing = false;
    public AudioClip gunPickupSound;
    public AudioClip otherPickupSound;

    public Image guiImage; // Reference to the GUI image element
    public Sprite defaultGunImage; // Default image to show when no gun is equipped

    // Add a reference to the kicking animation clip
    public AnimationClip kickingAnimationClip;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();

        // Ensure audio sources are initialized
        if (footstepAudioSource == null)
        {
            footstepAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (kickAudioSource == null)
        {
            kickAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (impactAudioSource == null)
        {
            impactAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // Initialize equipped icon image
        equippedIconImage = EquippedSlot.GetComponent<Image>();

        // Ensure both slots are active at the start
        UnarmedSlot.gameObject.SetActive(true);
        EquippedSlot.gameObject.SetActive(true);

        // Initialize UI icons
        SetEquippedGunIcon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            HandleKick();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleInteraction();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            DropGun();
        }

        // Check for right mouse button click to aim
        if (Input.GetMouseButtonDown(1) && currentGun != null)
        {
            HandleAiming();
        }

        // Check for right mouse button release to stop aiming
        if (Input.GetMouseButtonUp(1))
        {
            HandleAimingRelease();
        }

        // Check if enough time has passed since dropping the gun
        if (currentGun == null && !canPickUp && Time.time >= lastDropTime + dropCooldown)
        {
            canPickUp = true; // Allow pickup after cooldown
        }

        // Check for strafing input
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            isStrafing = true;
            animator.SetBool("IsStrafing", true);
        }
        else
        {
            isStrafing = false;
            animator.SetBool("IsStrafing", false);
        }

        // Check if the player is moving
        isMoving = IsMoving();
        crosshairController.SetCrosshairSize(isMoving);

        MovePlayer();
        RotatePlayer();
        UpdateAnimator();

        UpdateFOV();
    }

    void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        float currentFootstepInterval = isRunning ? runFootstepInterval : walkFootstepInterval;

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        rb.linearVelocity = moveDirection * currentSpeed; // Use velocity instead of linearVelocity

        if (IsMoving() && Time.time > nextFootstepTime)
        {
            PlayRandomFootstepSound();
            nextFootstepTime = Time.time + currentFootstepInterval;
        }

        // Update the Speed and Direction parameters in the Animator
        animator.SetFloat("Speed", moveDirection.magnitude * currentSpeed);
        if (isStrafing)
        {
            animator.SetFloat("StrafeSpeed", Mathf.Abs(horizontalInput));
            animator.SetFloat("Direction", horizontalInput);
        }
        else
        {
            animator.SetFloat("StrafeSpeed", 0);
            animator.SetFloat("Direction", 0);
        }
        animator.SetBool("IsMoving", IsMoving());
        animator.SetBool("IsRunning", isRunning);
    }

    void RotatePlayer()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, 0);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 worldMousePosition = ray.GetPoint(rayDistance);
            Vector3 lookDirection = worldMousePosition - transform.position;
            lookDirection.y = 0;

            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Euler(90f, rotation.eulerAngles.y, 0f);
        }
    }

    void UpdateAnimator()
    {
        if (animator != null)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            bool isAiming = IsAiming() && !isRunning; // Only aim if not running
            animator.SetBool("IsMoving", IsMoving());
            animator.SetBool("IsRunning", isRunning);
            animator.SetBool("IsAiming", isAiming);

            // Debug logs
            Debug.Log("IsMoving: " + IsMoving());
            Debug.Log("IsRunning: " + isRunning);
            Debug.Log("IsAiming: " + isAiming);
            Debug.Log("Speed: " + animator.GetFloat("Speed"));
        }
    }

    bool IsMoving()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        return Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;
    }

    void HandleKick()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Kicking"))
        {
            animator.SetTrigger("IsKicking");
            StartCoroutine(DelayedKick());
        }
    }

    IEnumerator DelayedKick()
    {
        Debug.Log("Kicking animation started.");
        yield return new WaitForSeconds(0.2f);

        InstantiateFootCollider();
        PlayKickSound();
        KickDoor();

        // Wait for the kicking animation to finish
        if (kickingAnimationClip != null)
        {
            yield return new WaitForSeconds(kickingAnimationClip.length);
        }

        animator.ResetTrigger("IsKicking");
        Debug.Log("Kicking animation finished.");
    }

    void InstantiateFootCollider()
    {
        GameObject footCollider = Instantiate(footColliderPrefab, transform.position + transform.up * 0.3f, transform.rotation);
        footCollider.transform.parent = null;

        FootCollider footColliderScript = footCollider.GetComponent<FootCollider>();

        if (footColliderScript != null)
        {
            footColliderScript.impactSound = impactSound;
            footColliderScript.kickForce = 1000f;
            footColliderScript.OnImpact += ShakeCameraOnImpact;
        }

        Destroy(footCollider, 0.5f);
    }

    void KickDoor()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.up, out hit, 2f))
        {
            InsideController door = hit.collider.GetComponent<InsideController>();

            if (door != null)
            {
                door.KickDoor(transform.up);
            }

            else
            {
                Debug.Log("No InsideController found on the hit object: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("No object in front of the player to kick.");
        }
    }

    void HandleInteraction()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.up, out hit, 1f))
        {
            InsideController door = hit.collider.GetComponent<InsideController>();

            if (door != null)
            {
                door.ToggleDoor();
            }

            else
            {
                Debug.Log("No InsideController found on the hit object: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("No object in front of the player to interact with.");
        }
    }

    void PlayKickSound()
    {
        if (kickSound != null && kickAudioSource != null)
        {
            kickAudioSource.PlayOneShot(kickSound);
        }
    }

    void PlayRandomFootstepSound()
    {
        if (footstepSounds.Length > 0 && footstepAudioSource != null)
        {
            AudioClip footstepClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            footstepAudioSource.PlayOneShot(footstepClip);
        }
    }

    void ShakeCameraOnImpact()
    {
        if (cameraFollow != null)
        {
            cameraFollow.ShakeCamera(0.2f, 0.05f);
        }
    }

    public AudioSource GetImpactAudioSource()
    {
        return impactAudioSource;
    }

    public void EndKick()
    {
        Debug.Log("EndKick function called!");
    }

    void SetEquippedGunIcon()
    {
        if (currentGun != null)
        {
            // Assuming currentGun's tag determines the icon
            if (currentGun.CompareTag("DefaultGun"))
            {
                equippedIconImage.sprite = defaultGunIcon;
            }
            else if (currentGun.CompareTag("Revolver"))
            {
                equippedIconImage.sprite = revolverIcon;
            }
            else if (currentGun.CompareTag("Rifle"))
            {
                equippedIconImage.sprite = rifleIcon;
            }
            else
            {
                equippedIconImage.sprite = defaultGunIcon; // Default case
            }
        }
    }

    void UpdateEquippedGunUI(bool isEquipped)
    {
        Transform gunModel = EquippedSlot.Find("GunModel");

        if (isEquipped)
        {
            if (gunModel == null)
            {
                gunModel = Instantiate(gunPrefab, EquippedSlot).transform;
                gunModel.name = "GunModel";
            }

            gunModel.gameObject.SetActive(true);
            gunModel.localPosition = Vector3.zero;
            gunModel.localRotation = Quaternion.identity;

            // Ensure the correct icon is set only when a gun is equipped
            SetEquippedGunIcon();
        }
        else
        {
            if (gunModel != null)
            {
                Destroy(gunModel.gameObject);
            }

            // No gun equipped, possibly set a default state for the icon if needed
            // equippedIconImage.sprite = defaultGunIcon; // This line may or may not be needed, depending on your UI setup
        }

        UnarmedSlot.gameObject.SetActive(!isEquipped);
    }

    public void DropGun()
    {
        if (currentGun != null)
        {
            Debug.Log("Dropping gun: " + currentGun.name);
            Collider[] colliders = currentGun.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }

            GunController gunController = currentGun.GetComponent<GunController>();
            if (gunController != null)
            {
                gunController.DropGun();
            }
            else
            {
                Debug.LogWarning("GunController component not found on the current gun.");
            }

            currentGun = null;
            canPickUp = false; // Disable picking up immediately
            lastDropTime = Time.time; // Record the drop time
            Invoke("ResetPickup", 2f); // Call ResetPickup after 2 seconds
            UpdateGunImage(null); // Reset the GUI image to default
        }
    }

    void ResetPickup()
    {
        canPickUp = true;
    }

    private IEnumerator PickupCooldown()
    {
        yield return new WaitForSeconds(dropCooldown);
        canPickUp = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("GunPickup"))
        {
            if (canPickUp)
            {
                PickUpGun(other.gameObject);

                if (canPickUp && Time.time >= lastDropTime + dropCooldown)
                {
                    PickUpGun(other.gameObject);
                }
            }
        }
    }

    void PickUpGun(GameObject gunObject)
    {
        if (currentGun == null && canPickUp)
        {
            // Set the current gun reference
            currentGun = gunObject;

            // Set the parent to the gun holder
            currentGun.transform.SetParent(gunHolder);

            // Ensure gun is active
            currentGun.SetActive(true);
            currentGun.transform.localRotation = Quaternion.Euler(3.1485033f, 182.671646f, 1.77343822f);
            currentGun.transform.localScale = new Vector3(0.01345291f, 0.0098518f, 0.00630685f);

            // Disable colliders on the gun
            Collider[] colliders = currentGun.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            // Set tag to avoid re-picking up
            currentGun.tag = "Untagged";

            // Update UI
            UpdateEquippedGunUI(true);
            SetEquippedGunIcon();

            GunController gunController = currentGun.GetComponent<GunController>();
            if (gunController != null && gunController.gunImage != null)
            {
                UpdateGunImage(gunController.gunImage); // Update the GUI image to the gun's image
            }
        }
    }

    public void EquipGun()
    {
        if (currentGun == null && gunPrefab != null && gunHolder != null)
        {
            // Instantiate gunPrefab and parent it to gunHolder
            currentGun = Instantiate(gunPrefab, gunHolder);
            currentGun.SetActive(true);
            currentGun.transform.localRotation = Quaternion.Euler(3.1485033f, 182.671646f, 1.77343822f);

            UpdateEquippedGunUI(true);
            SetEquippedGunIcon();

            GunController gunController = currentGun.GetComponent<GunController>();
            if (gunController != null && gunController.gunImage != null)
            {
                UpdateGunImage(gunController.gunImage); // Update the GUI image to the gun's image
            }

            // Debug logs
            Debug.Log("GunPrefab Position: " + currentGun.transform.position);
            Debug.Log("GunHolder Position: " + gunHolder.position);
            Debug.Log("Gun Local Position: " + currentGun.transform.localPosition);
            Debug.Log("Gun Local Rotation: " + currentGun.transform.localRotation.eulerAngles);
            Debug.Log("Gun Local Scale: " + currentGun.transform.localScale);
        }
    }

    void FixedUpdate()
    {
        if (currentGun != null && gunHolder != null)
        {
            // Update gun position and rotation to stick to gunHolder
            currentGun.transform.localPosition = Vector3.zero; // Adjust if necessary
            currentGun.transform.localRotation = Quaternion.identity;
        }
    }

    void HandleAiming()
    {
        if (animator != null && !animator.GetBool("IsRunning"))
        {
            crosshairController.ShowCrosshair(); // Show the crosshair
            animator.SetBool("IsAiming", true);
        }
    }

    void HandleAimingRelease()
    {
        if (animator != null)
        {
            animator.SetBool("IsAiming", false);
            crosshairController.HideCrosshair(); // Hide the crosshair
        }
    }

    void UpdateFOV()
    {
        float targetFOV = IsAiming() ? zoomedFOV : normalFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    public bool IsAiming()
    {
        return Input.GetMouseButton(1) && currentGun != null && !animator.GetBool("IsRunning");
    }

    public void UpdateGunImage(Sprite gunSprite)
    {
        if (guiImage != null)
        {
            guiImage.sprite = gunSprite != null ? gunSprite : defaultGunImage;
        }

    bool IsMoving()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        return Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;
        }
    }
}
