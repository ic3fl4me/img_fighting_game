using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //private InputActions inputActions;
    public PlayerInput player1;
    public PlayerInput player2;

    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    private Vector2 inputVector;
    [SerializeField] private float jumpVelocity = 18f;
    [SerializeField] private float baseGravityScale = 3f;
    [SerializeField] private float fallMultiplier = 4.5f;
    [SerializeField] private float lowJumpMultiplier = 7f;
    [SerializeField] private float airControlSpeed = 8f;
    private bool jumpHeld;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject attackPos;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private int maxDashes = 1;
    private int dashesLeft;
    private bool isDashing;
    private bool facingRight = true;
    private float dashTimeRemaining;

    public bool IsAttacking { get; private set; }
    public bool IsDashing => isDashing;

    private void Awake()
    {
        //inputActions = new InputActions();
        rb = GetComponent<Rigidbody2D>();
        dashesLeft = maxDashes;

    }

    private void Start()
    {
        /*
        inputActions.Player.Enable();
        inputActions.Player.Sprint.performed += StartSprinting;
        inputActions.Player.Sprint.canceled += StopSprinting;
        inputActions.Player.Jump.performed += StartJumping;
        inputActions.Player.Jump.canceled += StopJumping;
        inputActions.Player.Attack.performed += AttackInput;
        */
        //inputActions.Player.Interact.performed += Interact;
        //inputActions.Player.OpenUI.performed += OnOpenUI;
        //inputActions.UI.CloseUI.performed += OnCloseUI;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //makes new player on button hit Leon
        GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        Debug.Log("Player ID: " + GetComponent<PlayerInput>().playerIndex);
        Debug.Log(Gamepad.all.Count);
    }

    private void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Setzt die verfügbaren Dashes zurück, sobald der Boden berührt wird
        if (isGrounded && !wasGrounded)
        {
            dashesLeft = maxDashes;
        }

        animator.SetFloat("Speed", inputVector.magnitude);

        //PlayerInput();

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        IsAttacking = stateInfo.IsName("Player_Attack1") ||
                      stateInfo.IsName("Player_Attack2") ||
                      stateInfo.IsName("Player_Attack3") ||
                      stateInfo.IsName("Player_Attack1_Transition") ||
                      stateInfo.IsName("Player_Attack2_Transition") ||
                      stateInfo.IsName("Player_Attack3_Transition");

        // Dreht den AI Sprite und die Attack Hitbox in die Bewegungsrichtung
        if (inputVector.x < 0)
        {
            facingRight = false;
            spriteRenderer.flipX = true;
            attackPos.transform.localPosition = new Vector3(-Mathf.Abs(attackPos.transform.localPosition.x), attackPos.transform.localPosition.y, attackPos.transform.localPosition.z);
        }
        else if (inputVector.x > 0)
        {
            facingRight = true;
            spriteRenderer.flipX = false;
            attackPos.transform.localPosition = new Vector3(Mathf.Abs(attackPos.transform.localPosition.x), attackPos.transform.localPosition.y, attackPos.transform.localPosition.z);
        }

        // Zählt die verbleibende Dash-Dauer herunter
        if (isDashing)
        {
            dashTimeRemaining -= Time.deltaTime;
            if (dashTimeRemaining <= 0f)
            {
                EndDash();
            }
        }
    }

    private void FixedUpdate()
    {
        // Während des Dashs läuft eine eigene, feste horizontale Geschwindigkeit - die Y-Velocity bleibt unberührt, damit die Schwerkraft normal weiter wirkt
        if (isDashing)
        {
            float dashDirection = facingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
            ModifyGravity();
            return;
        }

        if (IsAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // Nähert die horizontale Geschwindigkeit in der Luft langsam an die Zielgeschwindigkeit an
        if (!isGrounded)
        {
            Vector2 targetVelocity = new Vector2(inputVector.x * airControlSpeed, rb.linearVelocity.y);
            rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, targetVelocity.x, 50f * Time.fixedDeltaTime), rb.linearVelocity.y);
        }

        SetMovementSpeed();
        ModifyGravity();
    }

    void OnDrawGizmos()
    {
        // visualize in editor
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }


    /*
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
    */

    // Setzt die horizontale Geschwindigkeit des Spielers basierend auf dem Input
    private void SetMovementSpeed()
    {
        rb.linearVelocity = new Vector2(
        inputVector.x * moveSpeed,
        rb.linearVelocity.y
        );
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
    }

    private void StartDash()
    {
        if (dashesLeft <= 0 || isDashing)
        {
            return;
        }

        dashesLeft--;
        isDashing = true;
        dashTimeRemaining = dashDuration;
    }

    private void EndDash()
    {
        isDashing = false;
    }

    // Ändert die Schwerkraft, je nachdem ob der Spieler fällt, springt und springen hält oder springt und nicht die Taste hält
    void ModifyGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0 && !jumpHeld)
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = baseGravityScale;
        }
    }

    private void AttackInput(InputAction.CallbackContext context)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Idle") ||
            stateInfo.IsName("Player_Walk") ||
            stateInfo.IsName("Player_Attack1_Transition") ||
            stateInfo.IsName("Player_Attack2_Transition"))
        {
            playerAttack.Attack();
            animator.SetTrigger("Attack");
        }
    }

    //private void OnOpenUI(InputAction.CallbackContext context)
    //{
    //    OpenCanvas();
    //}

    //private void OnCloseUI(InputAction.CallbackContext context)
    //{
    //    CloseCanvas();
    //}

    //neu 2 playerinput Leon
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log($"{gameObject.name} - {context.action.name}");
        inputVector = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            Jump();
        }

        if (context.performed)
            jumpHeld = true;

        if (context.canceled)
            jumpHeld = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AttackInput(context);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartDash();
        }
    }
}