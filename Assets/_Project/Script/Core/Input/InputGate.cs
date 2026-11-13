using UnityEngine;

public class InputGate : IMoveInput, IActionInput
{
    private readonly IMoveInput move;
    private readonly IActionInput action;
    public bool IsLocked { get; private set; }

    public InputGate(IMoveInput move, IActionInput action)
    { this.move = move; this.action = action; }

    public void Lock()   { IsLocked = true; }
    public void Unlock() { IsLocked = false; }

    public Vector2 Move      => IsLocked ? Vector2.zero : move.Move;
    public bool IsRunning    => !IsLocked && move.IsRunning;
    public bool InteractPressed => !IsLocked && action.InteractPressed;
}