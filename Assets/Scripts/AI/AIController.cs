using UnityEngine;

// Implementiert sämtliche Logik und Funktionen der AI, die nicht direkt mit den Entscheidungen zu tun haben
public class AIController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpVelocity = 18f;
    [SerializeField] private float baseGravityScale = 3f;
    [SerializeField] private float fallMultiplier = 4.5f;
    [SerializeField] private float lowJumpMultiplier = 7f;
    [SerializeField] private float airControlSpeed = 8f;
    private bool jumpHeld;

    public Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AIAttack aiAttack;
    [SerializeField] private GameObject attackPos;

    private Rigidbody2D rb;
    private bool isGrounded;

    public bool IsAttacking { get; private set; }
    public bool IsHitstun { get; private set; }

    [System.NonSerialized] public float InputHorizontal;
    [System.NonSerialized] public bool InputJumpRequested;
    [System.NonSerialized] public bool InputPunchRequested;
    [System.NonSerialized] public bool InputBlockRequested;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        IsAttacking = stateInfo.IsName("Player_Attack1") || 
                      stateInfo.IsName("Player_Attack2") ||
                      stateInfo.IsName("Player_Attack3") ||
                      stateInfo.IsName("Player_Attack1_Transition") ||
                      stateInfo.IsName("Player_Attack2_Transition") ||
                      stateInfo.IsName("Player_Attack3_Transition");

        // IsHitstun = stateInfo.IsName("Hitstun");
        IsHitstun = false; // Placeholder

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        // Dreht den AI Sprite und die Attack Hitbox in die Bewegungsrichtung
        if (!IsAttacking && !IsHitstun)
        {
            if (InputHorizontal < 0)
            {
                spriteRenderer.flipX = true;
                attackPos.transform.localPosition = new Vector3(-Mathf.Abs(attackPos.transform.localPosition.x), attackPos.transform.localPosition.y, attackPos.transform.localPosition.z);
            }
            else if (InputHorizontal > 0)
            {
                spriteRenderer.flipX = false;
                attackPos.transform.localPosition = new Vector3(Mathf.Abs(attackPos.transform.localPosition.x), attackPos.transform.localPosition.y, attackPos.transform.localPosition.z);
            }
        }
    }

    private void FixedUpdate()
    {
        // Bei Attacke und Block horizontales Movement stoppen
        if (IsAttacking || IsHitstun)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // Nähert die horizontale Geschwindigkeit in der Luft langsam an die Zielgeschwindigkeit an
        if (!isGrounded)
        {
            Vector2 targetVelocity = new Vector2(InputHorizontal * airControlSpeed, rb.linearVelocity.y);
            rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, targetVelocity.x, 50f * Time.fixedDeltaTime), rb.linearVelocity.y);
        }
        
        SetMovementSpeed();
        ModifyGravity();
        
        if (InputJumpRequested && isGrounded)
        {
            ExecuteJump();
        }

        if (InputPunchRequested)
        {
            ExecuteAttack();
        }
    }

    // Setzt die horizontale Geschwindigkeit der AI basierend auf dem Input
    private void SetMovementSpeed()
    {
        rb.linearVelocity = new Vector2(
        InputHorizontal * moveSpeed,
        rb.linearVelocity.y
        );
    }

    private void ExecuteJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
    }

    // Ändert die Schwerkraft, je nachdem ob die AI fällt, springt und springen hält oder springt und nicht die Taste hält
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

    private void ExecuteAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!IsAttacking && !IsHitstun &&
            (stateInfo.IsName("Idle") ||
            stateInfo.IsName("Player_Walk") ||
            stateInfo.IsName("Player_Attack1_Transition") ||
            stateInfo.IsName("Player_Attack2_Transition")))
        {
            aiAttack.Attack();
            animator.SetTrigger("Attack");
        }
    }
}
