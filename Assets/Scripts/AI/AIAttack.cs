using UnityEngine;

// Diese Klasse ist für die Angriffslogik der AI zuständig.
public class AIAttack : MonoBehaviour
{
    [Header("Attack Stats")]
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;

    [Header("References")]
    [SerializeField] private Transform attackPos;
    [SerializeField] private LayerMask EnemyLayer;

    private float attackTimer = 0;

    private void Update()
    {
        // Cooldown timer for attack
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        // Checkt ob Attack auf Cooldown ist, wenn nicht, dann wird Cooldown gesetzt und Attacke ausgeführt
        if (attackTimer > 0) return;

        attackTimer = attackCooldown;

        // Checkt für Kollisionen der Attackenhitbox mit dem Spieler und ruft bei Kollision seine TakeDamage auf
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, EnemyLayer);
        foreach (Collider2D hitEnemy in enemiesToDamage)
        {
            hitEnemy.GetComponent<Entity>().TakeDamage(attackDamage);
        }

    }

    private void OnDrawGizmosSelected()
    {
        // Draw attack range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
