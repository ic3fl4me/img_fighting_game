using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [SerializeField] private List<PlayerController> allPlayers;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public List<PlayerController> GetAllPlayers()
    {
        return allPlayers;
    }

    public PlayerController GetClosestPlayer(Vector3 referencePos)
    {
        PlayerController closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (PlayerController player in allPlayers)
        {
            if (player == null) continue;
            float distance = Vector3.SqrMagnitude(player.transform.position - referencePos);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }
}
