using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

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
    public float turnSpeed = 0.1f;
    private float turnSmoothVelocity;

    [Header("Jumping Variables")]
    public float jumpForce = 4f;
    public float quickJumpMultiplier = 0.5f;
    public TMP_Text coyoteText;
    public TMP_Text bufferText;
    public int numExtraJumps = 1;
    [SerializeField] int _jumps;

    // Coyote and buffer variables
    public float coyoteTimer = 0.5f;
    public float jumpBufferTime = 0.5f;
    [SerializeField] float coyoteTime;
    [SerializeField] float jumpBuffer;

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

    [Header("Attacking")]
    public bool inAction;
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

        if (!inAction)
        {
            HandleInput();
            HandleMovement();
        }

        // Set animation value
        _anim.SetBool("InAction", inAction);

        // Handle jumping value
        if (isGrounded)
        {
            _jumps = numExtraJumps;
            coyoteTime = coyoteTimer;
        }
        else
            coyoteTime -= Time.deltaTime;

        jumpBuffer -= Time.deltaTime;
        if (jumpBuffer < 0) jumpBuffer = 0;

        coyoteText.text = "Coyote Time = " + coyoteTime.ToString();
        bufferText.text = "Jump Buffer = " + jumpBuffer.ToString();
    }
    private void HandleInput()
    {
        moveInput = _inputs.Player.Move.ReadValue<Vector2>();
        if (_inputs.Player.Jump.triggered)
        {
            jumpBuffer = jumpBufferTime;
            // coyoteTime = 0;
        }
        if (_inputs.Player.Attack.triggered)
        {
            inAction = true;
            _anim.SetTrigger("Attack");
        }
        if (_inputs.Player.Jump.WasReleasedThisFrame() && playerVelocity.y > 0)
        {
            playerVelocity.y *= quickJumpMultiplier;
        }
        if (jumpBuffer > 0)
        {
            if (coyoteTime > 0)
            {
                _jumps--;
                coyoteTime = 0;
                jumpBuffer = 0;
                playerVelocity.y = Mathf.Sqrt(jumpForce * -3f * gravityForce);
                _anim.SetTrigger("Jump");
            }
            else if (_jumps > 0)
            {
                playerVelocity.y = 0;
                _jumps--;
                coyoteTime = 0;
                jumpBuffer = 0;
                playerVelocity.y = Mathf.Sqrt(jumpForce * -3f * gravityForce);
                _anim.SetTrigger("Jump");
            }
        }
    }
    private void HandleMovement()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        _anim.SetFloat("Speed", moveDirection.magnitude, moveBlend, Time.deltaTime);
        _anim.SetFloat("VSpeed", playerVelocity.y);

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
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -0.5f;
        }
        else
        {
            playerVelocity.y += gravityForce * Time.deltaTime;
        }
        _anim.SetBool("Grounded", isGrounded);
    }
    void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && coyoteTime > 0)
        {
            playerVelocity.y = Mathf.Sqrt(jumpForce * -3f * gravityForce);
            coyoteTime = 0;

            cc.Move(playerVelocity * Time.deltaTime);
        }
    }
}
