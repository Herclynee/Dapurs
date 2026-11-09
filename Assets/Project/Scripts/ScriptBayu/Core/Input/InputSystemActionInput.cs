using UnityEngine.InputSystem;

public class InputSystemActionInput : IActionInput
{
    private readonly InputAction interact;

    public InputSystemActionInput()
    {
        interact = InputSystem.actions.FindAction("Interact");
    }

    public bool IsInteractPressed()
    {
        return interact != null && interact.WasPressedThisFrame();
    }
}