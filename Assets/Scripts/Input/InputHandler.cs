using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Vector2 MoveInput;
    public bool SouthActionDown;
    public bool WestActionDown;
    public bool NorthActionDown;
    public bool EastActionDown;
    public bool EastSpecialDown;
    public bool WestSpecialDown;

    public void ConnectInputsToGO(IInputConnectable _agent)
    {
        _agent.ConnectInput(this);
    }

    public void DisconnectInputsFromGO(IInputConnectable _agent)
    {
        _agent.DisconnectInput();
    }
}
