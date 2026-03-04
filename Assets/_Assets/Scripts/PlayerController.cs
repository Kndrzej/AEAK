using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private const string INTERACTABLE_TAG = "Interactable";

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private Transform cameraHolder;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 3f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private float yVelocity;
    private float xRotation;

    private Outline currentOutline; // currently highlighted outline

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        Ray ray = new Ray(cameraHolder.position, cameraHolder.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            // Try to get SkullPuzzle from hit object or its parents
            SkullPuzzle puzzle = hit.collider.GetComponentInParent<SkullPuzzle>();
            if (puzzle != null)
            {
                puzzle.TryRotateFromRaycast(hit.collider.transform);
            }
        }
    }

    private void Update()
    {
        Move();
        Look();
        CheckInteractable();
    }

    private void Move()
    {
        Vector3 move = transform.right * moveInput.x +
                       transform.forward * moveInput.y;

        if (controller.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
        }

        yVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * moveSpeed;
        velocity.y = yVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public Vector2 GetLookInput()
    {
        return lookInput;
    }

    private void CheckInteractable()
    {
        Ray ray = new Ray(cameraHolder.position, cameraHolder.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag(INTERACTABLE_TAG))
            {
                Outline outline = hit.collider.GetComponent<Outline>();
                if (outline != null && outline != currentOutline)
                {
                    if (currentOutline != null)
                        currentOutline.enabled = false;

                    currentOutline = outline;
                    currentOutline.enabled = true;
                }
                return;
            }
        }

        // Disable outline if nothing is hit
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }
}