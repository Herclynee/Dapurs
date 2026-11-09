using UnityEngine;

public class InputGate : IMoveInput, IActionInput
{
    private readonly IMoveInput move;        // input asli yang dibungkus
    private readonly IActionInput action;

    public bool IsLocked { get; private set; }

    public InputGate(IMoveInput moveInput, IActionInput actionInput)
    {
        move = moveInput;
        action = actionInput;
    }

    public void Lock() => IsLocked = true;
    public void Unlock() => IsLocked = false;

    // Terkunci → selalu nol/false; tidak → teruskan ke input asli
    public Vector2 Move => IsLocked ? Vector2.zero : move.Move;
    public bool IsRunning => !IsLocked && move.IsRunning;
    public bool InteractPressed => !IsLocked && action.InteractPressed;
}