using UnityEngine;

public class LanternMotion : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform lanternChild;
    [SerializeField] private PlayerController playerController;

    [Header("Rotation Range")]
    [SerializeField] private float minX = -15f;
    [SerializeField] private float maxX = 15f;
    [SerializeField] private float minZ = -10f;
    [SerializeField] private float maxZ = 10f;
    [SerializeField] private float maxMovementZ = 10f;

    [Header("Motion Settings")]
    [SerializeField] private float swaySpeed = 3f;
    [SerializeField] private float movementInfluence = 5f;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField, Range(1f, 2f)] private float strengthMultiplier = 1.25f;

    private Quaternion initialLocalRotation;
    private float currentZ;
    private float zVelocity = 0f;

    private void Start()
    {
        if (lanternChild == null)
            lanternChild = transform;

        initialLocalRotation = lanternChild.localRotation;

        if (playerController == null)
            Debug.LogWarning("Assign the PlayerController to make lantern respond to movement!");

        currentZ = initialLocalRotation.eulerAngles.z;
    }

    private void Update()
    {
        float rotX = Mathf.Lerp(minX, maxX, (Mathf.Sin(Time.time * swaySpeed) + 1f) / 2f) * strengthMultiplier;
        float targetZ = Mathf.Lerp(minZ, maxZ, (Mathf.Sin(Time.time * swaySpeed * 0.7f) + 1f) / 2f) * strengthMultiplier;

        if (playerController != null)
        {
            Vector2 moveInput = playerController.GetLookInput();
            targetZ += Mathf.Clamp(moveInput.x * movementInfluence, -maxMovementZ, maxMovementZ) * strengthMultiplier;
        }

        currentZ = Mathf.SmoothDamp(currentZ, targetZ, ref zVelocity, smoothTime);
        lanternChild.localRotation = Quaternion.Euler(rotX, initialLocalRotation.eulerAngles.y, currentZ);
    }
}