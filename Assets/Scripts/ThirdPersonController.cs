using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    private InputSystem_Actions inputs;
    private CharacterController controller;
    public CinemachineCamera characterCamera;

    [SerializeField] private Vector2 moveInput;

    public float moveSpeed = 20f;
    public float runMultiplier = 4f;
    public float rotationSpeed = 200f;

    public float gravity = -9.81f;
    public float verticalVelocity = 0f;
    public float jumpForce = 10f;
    public float pushForce = 4f;

    private void Awake()
    {
        inputs = new();
        controller = GetComponent<CharacterController>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private bool IsDashing;
    public float dashTimer = 0.2f;
    public float dashDuration = 0.2f;
    public float dashForce = 40;

    private bool isRunning;

    private void OnEnable()
    {
        inputs.Enable();

        inputs.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputs.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputs.Player.Jump.performed += OnJump;
        inputs.Player.Dash.performed += OnDash;

        inputs.Player.Sprint.started += ctx => isRunning = true;
        inputs.Player.Sprint.canceled += ctx => isRunning = false;
    }

    void Update()
    {
        OnMove();
    }

    public void OnMove()
    {
        transform.Rotate(Vector3.up * moveInput.x * rotationSpeed * Time.deltaTime);

        Vector3 cameraForwardDir = characterCamera.transform.forward;
        cameraForwardDir.y = 0;
        cameraForwardDir.Normalize();

        float currentSpeed = isRunning ? moveSpeed * runMultiplier : moveSpeed;
        Vector3 moveDir = (transform.forward * moveInput.y) * currentSpeed;

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        moveDir.y = verticalVelocity;

        if (IsDashing)
        {
            moveDir = transform.forward * dashForce * (dashTimer / dashDuration);
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                IsDashing = false;
            }
        }
        controller.Move(moveDir * Time.deltaTime);

    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (this == null || controller == null) return;

        if (controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        IsDashing = true;
        dashTimer = dashDuration;
    }

}
