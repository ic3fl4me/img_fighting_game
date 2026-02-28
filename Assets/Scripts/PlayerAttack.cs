using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private float attackTimer;
    public float attackCooldown;

    public Transform attackPos;
    public float attackRange;
    public LayerMask EnemyLayer;
    public int attackDamage;

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
        Debug.Log("Attacking");
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, EnemyLayer);
        Debug.Log(enemiesToDamage.Length + ": " + enemiesToDamage.ToString());
        foreach (Collider2D hitEnemy in enemiesToDamage)
        {
            Debug.Log("Enemy Hit");
            hitEnemy.GetComponent<Entity>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
