using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemMoveInput : IMoveInput
{
    private readonly InputAction move;

    public InputSystemMoveInput()
    {
        move = InputSystem.actions?.FindAction("Move");
    }

    public Vector2 Move
    {
        get
        {
            if (move == null) return Vector2.zero;
            return move.ReadValue<Vector2>();
        }
    }

    public bool IsRunning
    {
        get
        {
            return Keyboard.current != null && 
                   (Keyboard.current.leftShiftKey.isPressed || 
                    Keyboard.current.rightShiftKey.isPressed);
        }
    }
}