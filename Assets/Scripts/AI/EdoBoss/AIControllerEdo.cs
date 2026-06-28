using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Implementiert sämtliche Logik und Funktionen der AI, die nicht direkt mit den Entscheidungen zu tun haben
public class AIControllerEdo : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpVelocity = 18f;
    [SerializeField] private float baseGravityScale = 3f;
    [SerializeField] private float fallMultiplier = 4.5f;
    [SerializeField] private float lowJumpMultiplier = 7f;
    [SerializeField] private float airControlSpeed = 8f;
    private bool jumpHeld;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AIAttackEdo aiAttack;
    [SerializeField] private GameObject attackPos;
    [SerializeField] private GameObject firePoint;

    private Rigidbody2D rb;
    private PlayerManager playerManager;
    [SerializeField] private GameObject teleportNodes;
    private bool isGrounded;
    private float flipTimer;
    public int teleportCounter = 0;
    private float teleportTimer = 0f;

    public bool IsAttacking { get; private set; }
    public bool IsHitstun { get; private set; }

    [System.NonSerialized] public float InputHorizontal;
    [System.NonSerialized] public bool InputTeleportRequested;
    [System.NonSerialized] public bool InputJumpRequested;
    [System.NonSerialized] public bool InputPunchRequested;
    [System.NonSerialized] public bool InputProjectileAttackRequested;
    [System.NonSerialized] public bool InputSpikeAttackRequested;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (playerManager == null)
        {
            playerManager = PlayerManager.instance;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        IsAttacking = stateInfo.IsName("Edoboss_Attack") || 
                      stateInfo.IsName("Edoboss_ProjectileAttack");

        // IsHitstun = stateInfo.IsName("Hitstun");
        IsHitstun = false; // Placeholder

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        teleportTimer -= Time.deltaTime;
        flipTimer += Time.deltaTime;
        // Dreht den AI Sprite und die Attack Hitbox in die Bewegungsrichtung
        if (!IsAttacking && !IsHitstun && flipTimer > 1f)
        {
            flipTimer = 0f;
            if (InputHorizontal > 0)
            {
                spriteRenderer.flipX = true;
                attackPos.transform.localPosition = FlipLocalX(attackPos, true);
                firePoint.transform.localPosition = FlipLocalX(firePoint, true);
            }
            else if (InputHorizontal < 0)
            {
                spriteRenderer.flipX = false;
                attackPos.transform.localPosition = FlipLocalX(attackPos, false);
                firePoint.transform.localPosition = FlipLocalX(firePoint, false);
            }
        }

        if (InputTeleportRequested && teleportTimer <= 0)
        {
            ExecuteTeleport();
        }

        if (InputPunchRequested)
        {
            ExecuteAttack();
        }

        if (InputProjectileAttackRequested)
        {
            ExecuteProjectileAttack();
        }

        if (InputSpikeAttackRequested)
        {
            ExecuteSpikeAttack();
        }
    }

    private void FixedUpdate()
    {
        // Bei Attacke und wenn stunned dann horizontales Movement stoppen
        if (IsAttacking || IsHitstun)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // Nähert die horizontale Geschwindigkeit in der Luft langsam an die Zielgeschwindigkeit an
        /*
        if (!isGrounded)
        {
            Vector2 targetVelocity = new Vector2(InputHorizontal * airControlSpeed, rb.linearVelocity.y);
            rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, targetVelocity.x, 50f * Time.fixedDeltaTime), rb.linearVelocity.y);
        }*/
        
        //SetMovementSpeed();
        //ModifyGravity();
        
        /*if (InputJumpRequested && isGrounded)
        {
            ExecuteJump();
        }*/
    }

    // Setzt die horizontale Geschwindigkeit der AI basierend auf dem Input
    private void SetMovementSpeed()
    {
        rb.linearVelocity = new Vector2(
        InputHorizontal * moveSpeed,
        rb.linearVelocity.y
        );
    }
    
    /*
    private void ExecuteJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
    }*/

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

    private void ExecuteTeleport()
    {
        Transform firstNode = teleportNodes.transform.GetChild(0);

        if (teleportCounter != 0 && teleportCounter % 4 == 0)
        {
            teleportCounter = 0;
            transform.position = firstNode.position;
        }
        else
        {
            transform.position = GetFurthestTeleportPointFromPlayers().position;
            teleportCounter++;
        }

        teleportTimer = 2f;
    }

    private Transform GetFurthestTeleportPointFromPlayers()
    {
        Transform furthestPosition = null;
        List<PlayerController> players = playerManager.GetAllPlayers();
        float furthestDistance = 0f;

        for (int i = 1; i < teleportNodes.transform.childCount; i++)
        {
            Transform node = teleportNodes.transform.GetChild(i);
            float score = float.MaxValue;

            foreach (PlayerController player in players)
            {
                float distance = Vector2.Distance(node.position, player.transform.position);
                score = Mathf.Min(score, distance);
            }

            if (score > furthestDistance)
            {
                furthestDistance = score;
                furthestPosition = node;
            }
        }
        
        return furthestPosition;
    }

    private void ExecuteAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!IsAttacking && !IsHitstun && stateInfo.IsName("Idle"))
        {
            aiAttack.Attack();
            animator.SetTrigger("Attack");
        }
    }

    private void ExecuteProjectileAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!IsAttacking && !IsHitstun && stateInfo.IsName("Idle"))
        {
            StartCoroutine(aiAttack.ProjectileAttack());
            animator.SetTrigger("ProjectileAttack");
        }
    }

    private void ExecuteSpikeAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!IsAttacking && !IsHitstun && stateInfo.IsName("Idle"))
        {
            aiAttack.SpikeAttack();
            animator.SetTrigger("Attack");
        }
    }

    private Vector3 FlipLocalX(GameObject obj, bool isPositive)
    {
        if (isPositive)
        {
            return new Vector3(-Mathf.Abs(obj.transform.localPosition.x), obj.transform.localPosition.y, obj.transform.localPosition.z);
        } else
        {
            return new Vector3(Mathf.Abs(obj.transform.localPosition.x), obj.transform.localPosition.y, obj.transform.localPosition.z);
        }
        
    }
}
