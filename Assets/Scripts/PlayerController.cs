using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputActions inputActions;

    [SerializeField] private float moveSpeed;
    private float effectiveSpeed;
    [SerializeField] private float sprintMultiplier;
    private Vector2 inputVector;
    private bool isSprinting;
    [SerializeField] private float jumpForce;
    private bool jumpRequested;
    private Rigidbody2D rb;
    [SerializeField] private PlayerAttack playerAttack;
    public Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded;

    private void Awake()
    {
        inputActions = new InputActions();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        inputActions.Player.Enable();
        inputActions.Player.Sprint.performed += StartSprinting;
        inputActions.Player.Sprint.canceled += StopSprinting;
        inputActions.Player.Jump.performed += StartJumping;
        inputActions.Player.Jump.canceled += StopJumping;
        inputActions.Player.Attack.performed += AttackInput;
        //inputActions.Player.Interact.performed += Interact;
        //inputActions.Player.OpenUI.performed += OnOpenUI;
        //inputActions.UI.CloseUI.performed += OnCloseUI;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDrawGizmos()
    {
        // visualize in editor
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetFloat("Speed", inputVector.magnitude);

        PlayerInput();

        if (inputVector.x < 0)
            spriteRenderer.flipX = true;
        else if (inputVector.x > 0)
            spriteRenderer.flipX = false;
    }

    private void FixedUpdate()
    {
        HandlePlayerInputs();

        if (jumpRequested && isGrounded)
        {
            Jump();
        }
    }

    private void PlayerInput()
    {
        // sets input vector to zero if the necessary action map is disabled
        if (!inputActions.Player.enabled)
        {
            inputVector = Vector2.zero;
            return;
        }

        // get axis for wasd movement
        inputVector = inputActions.Player.Move.ReadValue<Vector2>();
    }

    private void HandlePlayerInputs()
    {
        effectiveSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        transform.Translate(effectiveSpeed * Time.deltaTime * inputVector * new Vector3(1f, 0f, 1f));
    }

    private void StartSprinting(InputAction.CallbackContext context)
    {
        isSprinting = true;
    }

    private void StopSprinting(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }

    private void StartJumping(InputAction.CallbackContext context)
    {
        jumpRequested = true;
    }
    private void StopJumping(InputAction.CallbackContext context)
    {
        jumpRequested = false;
    }

    private void Jump()
    {
        rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode2D.Impulse);
    }

    private void AttackInput(InputAction.CallbackContext context)
    {
        playerAttack.Attack();
        animator.SetTrigger("Attack");
    }

    //private IEnumerator AttackSequence()
    //{
    //    attackCollider.enabled = true;

    //    yield return new WaitForSeconds(0.1f);

    //    attackCollider.enabled = false;
    //}

    //private void OnOpenUI(InputAction.CallbackContext context)
    //{
    //    // for own menu
    //    OpenCanvas();
    //}

    //private void OnCloseUI(InputAction.CallbackContext context)
    //{
    //    CloseCanvas();
    //}
}
