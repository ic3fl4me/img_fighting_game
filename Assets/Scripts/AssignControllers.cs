using UnityEngine;
using UnityEngine.InputSystem;

public class AssignControllers : MonoBehaviour
{
    public PlayerInput player1;
    public PlayerInput player2;

    void Start()
    {
        if (Gamepad.all.Count < 2)
        {
            Debug.LogError("Nicht genug Controller verbunden!");
            return;
        }

        // Player 1 bekommt Controller 1
        player1.SwitchCurrentControlScheme(Gamepad.all[0]);

        // Player 2 bekommt Controller 2
        player2.SwitchCurrentControlScheme(Gamepad.all[1]);

        Debug.Log(Gamepad.all.Count);
    }
}