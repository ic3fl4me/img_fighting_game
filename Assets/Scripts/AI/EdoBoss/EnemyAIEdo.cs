using System.Collections.Generic;
using UnityEngine;

public enum AIStateEdo
{
    Neutral,
    Attacking,
    Hitstun
}

// Diese Klasse funktioniert wie das Gehirn der AI. Sie entscheidet in welchem State die AI ist und über die Übergänge zwischen den States. Also sie lenkt die Entscheidungen der AI.
public class EnemyAiEdo : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private AIControllerEdo aiController;
    [SerializeField] private AIInputAdapterEdo aiInput;
    [SerializeField] private GameObject attackPos;

    [Header("Player References")]
    [SerializeField] private List<Transform> playerTransforms;
    
    public PlayerController targetedPlayer;

    [Header("Attack Stats")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float humanReactionDelay = 0.25f;
    [SerializeField] private float attackCooldown = 3f;

    private AIStateEdo currentState = AIStateEdo.Neutral;
    private float startDelay = 1f;
    private float attackTimer = 0;

    public float VirtualHorizontal { get; private set; }
    public bool VirtualPunch { get; private set; }
    public bool VirtualProjectileAttack { get; private set; }
    public bool VirtualSpikeAttack { get; private set; }
    public bool VirtualJump { get; private set; }

    private void Start()
    {
        
    }

    private void Update()
    {
        startDelay -= Time.deltaTime;
        if (startDelay > 0f) return;

        // Spieler der dem Boss näher ist finden und als Target setzen
        float minSqrDistanceToPlayer = Mathf.Infinity;
        for (int i = 0; i < playerTransforms.Count; i++)
        {
            float sqrDistToPlayer = (attackPos.transform.position - playerTransforms[i].position).sqrMagnitude;
            if (sqrDistToPlayer < minSqrDistanceToPlayer)
            {
                minSqrDistanceToPlayer = sqrDistToPlayer;
                targetedPlayer = playerTransforms[i].GetComponentInParent<PlayerController>();
            }
        }

        attackTimer -= Time.deltaTime;

        // Falls der Boss Stuns kriegt, kann muss er sofort in den State wechseln, damit er nichts mehr machen kann solange er stunned ist
        if (aiController.IsHitstun)
        {
            TransitionToState(AIStateEdo.Hitstun);
        }

        // State Machine der AI
        switch (currentState)
        {
            case AIStateEdo.Neutral:
                HandleNeutralState(minSqrDistanceToPlayer);
                break;

            case AIStateEdo.Attacking:
                HandleAttackingState(minSqrDistanceToPlayer);
                break;

            case AIStateEdo.Hitstun:
                HandleHitstunState(); 
                break;
        }
    }

    // Default State
    private void HandleNeutralState(float sqrDistanceToTarget)
    {
        ResetInputs();

        aiInput.VirtualHorizontal = (targetedPlayer.transform.position.x > transform.position.x) ? 1f : -1f;
        
        if (attackTimer <= 0f)
        {
            aiInput.VirtualTeleport = true;
            TransitionToState(AIStateEdo.Attacking);
        }
    }

    // Angreifen wenn Spieler in Reichweite, sonst -> neutral state
    private void HandleAttackingState(float sqrDistanceToTarget)
    {
        ResetInputs();

        Debug.Log(sqrDistanceToTarget + " " + attackRange * attackRange);

        if (transform.position.y > 5)
        {
            aiInput.VirtualSpikeAttack = true;
        } else if (sqrDistanceToTarget < attackRange * attackRange && Random.value < 0.5f)
        {
            aiInput.VirtualPunch = true;
        } else
        {
            aiInput.VirtualProjectileAttack = true;
        }

        attackTimer = attackCooldown;
        TransitionToState(AIStateEdo.Neutral);
    }

    // Verhindert AI Input bei Stun
    private void HandleHitstunState()
    {
        ResetInputs();

        if (!aiController.IsHitstun)
        {
            TransitionToState(AIStateEdo.Neutral);
        }
    }

    // State-Übergang Methode
    private void TransitionToState(AIStateEdo newState)
    {
        if (currentState == newState) return;

        currentState = newState;
    }

    private void ResetInputs()
    {
        aiInput.VirtualHorizontal = 0f;
        aiInput.VirtualTeleport = false;
        aiInput.VirtualPunch = false;
        aiInput.VirtualJump = false;
        aiInput.VirtualProjectileAttack = false;
        aiInput.VirtualSpikeAttack = false;
    }
}
