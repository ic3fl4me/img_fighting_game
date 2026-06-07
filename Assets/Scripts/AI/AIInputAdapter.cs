using UnityEngine;

// Stellt die AI Inputs zur Verfügung um einen normalen Controller/KBM Input zu simulieren
[RequireComponent(typeof(AIController))]
public class AIInputAdapter : MonoBehaviour
{
    private AIController coreController;

    public float VirtualHorizontal { get; set; }
    public bool VirtualJump { get; set; }
    public bool VirtualPunch { get; set; }
    public bool VirtualBlock { get; set; }

    private void Awake()
    {
        coreController = GetComponent<AIController>();
    }

    private void Update()
    {
        coreController.InputHorizontal = VirtualHorizontal;
        coreController.InputJumpRequested = VirtualJump;
        coreController.InputPunchRequested = VirtualPunch;
        coreController.InputBlockRequested = VirtualBlock;
    }
}
