using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    private float attackTimer;
    public float attackCooldown;

    public Transform attackPos;
    public float attackRange;
    public LayerMask EnemyLayer;
    public int attackDamage;


    //test PowerBar
    public Image PowerBar;
    //PowerAmount gibt den Wert womit die Bar verändert wird
    public float PowerAmount = 100f;


    private void Update()
    {
        
        if(attackTimer <= 0)
        {
            attackTimer = attackCooldown;
        } else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, EnemyLayer);
        foreach (Collider2D hitEnemy in enemiesToDamage)
        {
            hitEnemy.GetComponent<Entity>().TakeDamage(attackDamage);

            //für hit auf gegner steigt die PowerBar
            PowerAmount += attackDamage;
            //Cap das PowerBar bis 100 nur geht
            PowerAmount = Mathf.Clamp(PowerAmount, 0, 100);
            //FillAmount lässt die Anizeige nach Rechts steigen
            PowerBar.fillAmount = PowerAmount / 100f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
