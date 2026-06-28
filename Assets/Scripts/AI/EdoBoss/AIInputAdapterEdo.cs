using UnityEngine;

// Stellt die AI Inputs zur Verfügung um einen normalen Controller/KBM Input zu simulieren
public class AIInputAdapterEdo : MonoBehaviour
{
    private AIControllerEdo coreController;

    public float VirtualHorizontal { get; set; }
    public bool VirtualTeleport { get; set; }
    public bool VirtualJump { get; set; }
    public bool VirtualPunch { get; set; }
    public bool VirtualProjectileAttack { get; set; }
    public bool VirtualSpikeAttack { get; set; }

    private void Awake()
    {
        coreController = GetComponent<AIControllerEdo>();
    }

    private void Update()
    {
        coreController.InputHorizontal = VirtualHorizontal;
        coreController.InputTeleportRequested = VirtualTeleport;
        coreController.InputJumpRequested = VirtualJump;
        coreController.InputPunchRequested = VirtualPunch;
        coreController.InputProjectileAttackRequested = VirtualProjectileAttack;
        coreController.InputSpikeAttackRequested = VirtualSpikeAttack;
    }
}
