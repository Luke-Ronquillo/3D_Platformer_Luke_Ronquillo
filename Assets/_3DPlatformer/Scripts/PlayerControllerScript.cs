using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerScript : MonoBehaviour
{
    IA_PlatformerInputs _inputs;
    [SerializeField] CharacterController cc;
    [SerializeField] Animator _anim;
    [SerializeField] Transform camPosition;

    [Header("Movement Variables")]
    [SerializeField] Vector2 moveInput;
    [SerializeField] Vector3 moveDirection;
    public float moveSpeed = 4f;
    public float runSpeed  = 8f;
    public float jumpForce = 4f;
    public float turnSpeed = 0.1f;
    private float turnSmoothVelocity;

    [Header("Player Physics Variables")]
    public float gravityForce = -8f;
    [SerializeField] Vector3 playerVelocity;
    public Transform groundCheck;
    public LayerMask groundLayer;
    [SerializeField] Collider[] groundCollider;

    [Header("Animation Blending")]
    public float moveBlend;

    [Header("Boolean Variables")]
    public bool isGrounded;
    private void Awake()
    {
        if (_inputs == null)
            _inputs = new IA_PlatformerInputs();
        if (cc == null)
            cc = GetComponent<CharacterController>();
        if (_anim == null)
            _anim = GetComponentInChildren<Animator>();
        if (camPosition == null)
            camPosition = Camera.main.transform;
    }
    private void OnEnable()
    {
        _inputs.Enable();
        _inputs.Player.Jump.performed += OnJump;
    }
    private void OnDisable()
    {
        _inputs.Player.Jump.performed -= OnJump;
        _inputs.Disable();
    }
    private void Update()
    {
        HandlePhysics();
        HandleInput();
        HandleMovement();
    }
    private void HandleInput()
    {
        moveInput = _inputs.Player.Move.ReadValue<Vector2>();
    }
    private void HandleMovement()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        _anim.SetFloat("Speed", moveDirection.magnitude, moveBlend, Time.deltaTime);

        if (moveDirection.magnitude > 0.1f)
        {
            float targetAngle = MathF.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + camPosition.eulerAngles.y;
            float _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSpeed);
            transform.rotation = Quaternion.Euler(0, _angle, 0);
            Vector3 newDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            cc.Move(newDirection * moveSpeed * Time.deltaTime);
        }
        cc.Move(playerVelocity * Time.deltaTime);
    }

    private void HandlePhysics()
    {
        groundCollider = Physics.OverlapSphere(groundCheck.position, 0.1f, groundLayer);
        isGrounded = (groundCollider.Length > 0);
        if (isGrounded)
        {
            playerVelocity.y = -0.5f;
        }
        else
        {
            playerVelocity.y += gravityForce * Time.deltaTime;
        }
    }
    void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpForce * -3f * gravityForce);
            cc.Move(playerVelocity * Time.deltaTime);
        }
    }
}
