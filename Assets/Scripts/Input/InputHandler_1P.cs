using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler_1P : InputHandler
{
    public InputActionMap AvailableActions;

    public void Start()
    {
        AvailableActions.Enable();
    }

    public void Update()
    {
        SouthActionDown = AvailableActions.FindAction("SouthAction").phase == InputActionPhase.Performed;
        WestActionDown = AvailableActions.FindAction("WestAction").phase == InputActionPhase.Performed;
        NorthActionDown = AvailableActions.FindAction("NorthAction").phase == InputActionPhase.Performed;
        EastActionDown = AvailableActions.FindAction("EastAction").phase == InputActionPhase.Performed;
        EastSpecialDown = AvailableActions.FindAction("EastSpecial").phase == InputActionPhase.Performed;
        WestSpecialDown = AvailableActions.FindAction("WestSpecial").phase == InputActionPhase.Performed;
        MoveInput = AvailableActions.FindAction("Movement").ReadValue<Vector2>();
    }
}
