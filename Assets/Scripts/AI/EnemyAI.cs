using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
    Neutral,
    Attacking,
    Blocking,
    Hitstun
}

// Diese Klasse funktioniert wie das Gehirn der AI. Sie entscheidet in welchem State die AI ist und über die Übergänge zwischen den States. Also sie lenkt die Entscheidungen der AI.
public class EnemyAi : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private AIController aiController;
    [SerializeField] private AIInputAdapter aiInput;

    [Header("Player References")]
    [SerializeField] private List<Transform> playerTransforms;
    [SerializeField] private PlayerController targetedPlayer;

    [Header("Attack Stats")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float humanReactionDelay = 0.25f;

    private AIState currentState = AIState.Neutral;
    private float reactionTimer = 0f;

    public float VirtualHorizontal { get; private set; }
    public bool VirtualPunch { get; private set; }
    public bool VirtualBlock { get; private set; }
    public bool VirtualJump { get; private set; }

    void Start()
    {
        
    }

    void Update()
    {
        // Spieler der dem Boss näher ist finden und als Target setzen
        float minSqrDistanceToPlayer = Mathf.Infinity;
        for (int i = 0; i < playerTransforms.Count; i++)
        {
            float sqrDistToPlayer = (transform.position - playerTransforms[i].position).sqrMagnitude;
            if (sqrDistToPlayer < minSqrDistanceToPlayer)
            {
                minSqrDistanceToPlayer = sqrDistToPlayer;
                targetedPlayer = playerTransforms[i].GetComponentInParent<PlayerController>();
            }
        }

        // Falls der Boss Stuns kriegt, kann muss er sofort in den State wechseln, damit er nichts mehr machen kann solange er stunned ist
        if (aiController.IsHitstun)
        {
            TransitionToState(AIState.Hitstun);
        }

        // State Machine der AI
        switch (currentState)
        {
            case AIState.Neutral:
                HandleNeutralState(minSqrDistanceToPlayer);
                break;

            case AIState.Attacking:
                HandleAttackingState(minSqrDistanceToPlayer);
                break;

            case AIState.Blocking:
                HandleBlockingState();
                break;

            case AIState.Hitstun:
                HandleHitstunState(); 
                break;
        }
    }

    // Default State
    private void HandleNeutralState(float sqrDistance)
    {
        ResetInputs();

        // Wenn der Target Spieler in Reichweite ist und angreift, dann soll die AI anfangen zu blocken
        if (targetedPlayer.IsAttacking && sqrDistance <= attackRange * attackRange)
        {
            reactionTimer += Time.deltaTime;
            if (reactionTimer >= humanReactionDelay)
            {
                TransitionToState(AIState.Blocking);
                return;
            }
        }
        else
        {
            reactionTimer = 0f;
        }

        // Wenn Spieler nicht in Reichweite ist, zum Spieler navigieren
        if (sqrDistance > attackRange * attackRange)
        {
            aiInput.VirtualHorizontal = (targetedPlayer.transform.position.x > transform.position.x) ? 1f : -1f;
        }
        else
        {
            // Sobald Spieler in Reichweite ist, probieren zu attackieren
            aiInput.VirtualHorizontal = 0f;
            if (!targetedPlayer.IsAttacking)
            {
                TransitionToState(AIState.Attacking);
            }
        }

        // Wenn Spieler nahe ist aber auf y-Achse höher ist, dann soll die AI springen damit sie den Spieler erreicht
        if (targetedPlayer.transform.position.y > transform.position.y + 0.5f && Mathf.Abs(targetedPlayer.transform.position.x - transform.position.x) < 3f)
        {
            aiInput.VirtualJump = true;
        }
    }

    // Angreifen wenn Spieler in Reichweite, sonst -> neutral state
    private void HandleAttackingState(float sqrDistance)
    {
        if (sqrDistance > attackRange * attackRange)
        {
            TransitionToState(AIState.Neutral);
            return;
        }

        aiInput.VirtualPunch = true;

        TransitionToState(AIState.Neutral);
    }

    // Blocken wenn Spieler noch angreift, sonst -> neutral state
    private void HandleBlockingState()
    {
        ResetInputs();

        aiInput.VirtualBlock = true;

        if (!targetedPlayer.IsAttacking)
        {
            TransitionToState(AIState.Neutral);
        }
    }

    // Verhindert AI Input bei Stun
    private void HandleHitstunState()
    {
        ResetInputs();

        if (!aiController.IsHitstun)
        {
            TransitionToState(AIState.Neutral);
        }
    }

    // State-Übergang Methode
    private void TransitionToState(AIState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        reactionTimer = 0f;
    }

    private void ResetInputs()
    {
        aiInput.VirtualHorizontal = 0f;
        aiInput.VirtualPunch = false;
        aiInput.VirtualBlock = false;
        aiInput.VirtualJump = false;
    }
}
