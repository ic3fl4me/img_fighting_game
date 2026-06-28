using System.Collections;
using UnityEngine;

// Diese Klasse ist für die Angriffslogik der AI zuständig.
public class AIAttackEdo : MonoBehaviour
{
    [Header("Attack Stats")]
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackRange;

    [Header("References")]
    [SerializeField] private Transform attackPos;
    [SerializeField] private LayerMask EnemyLayer;

    [Header("Projectiles")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Spike spikePrefab;

    [SerializeField] private float groundPos = -8f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private PlayerManager playerManager;

    private void Update()
    {
        if (playerManager == null)
        {
            playerManager = PlayerManager.instance;
        }
    }

    public void Attack()
    {
        // Checkt für Kollisionen der Attackenhitbox mit dem Spieler und ruft bei Kollision seine TakeDamage auf
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, EnemyLayer);
        foreach (Collider2D hitEnemy in enemiesToDamage)
        {
            hitEnemy.GetComponent<Entity>().TakeDamage(attackDamage);
        }
    }

    public IEnumerator ProjectileAttack()
    {
        yield return new WaitForSeconds(0.75f);

        float facingDirection = spriteRenderer.flipX ? 1f : -1f;
        Vector2 direction = new Vector2(facingDirection, 0);

        Projectile projectile =
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        projectile.Fire(direction);
    }

    public void SpikeAttack()
    {
        foreach (PlayerController player in playerManager.GetAllPlayers())
        {
            Spike spike =
                Instantiate(spikePrefab, new Vector2(player.transform.position.x, groundPos), Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw attack range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

}
