using UnityEngine;

public class InputGate : IMoveInput, IActionInput
{
    private readonly IMoveInput moveInput;
    private readonly IActionInput actionInput;
    private bool isLocked = false;

    // Constructor menerima input sistem asli (Decorator Pattern)
    public InputGate(IMoveInput move, IActionInput action)
    {
        moveInput = move;
        actionInput = action;
    }

    public bool IsLocked => isLocked;
    public void Lock() => isLocked = true;
    public void Unlock() => isLocked = false;

    // --- Implementasi IMoveInput (WAJIB Property) ---
    public Vector2 Move
    {
        get
        {
            if (isLocked) return Vector2.zero; // Jika dikunci, player diam
            return moveInput.Move;
        }
    }

    public bool IsRunning
    {
        get
        {
            if (isLocked) return false;
            return moveInput.IsRunning;
        }
    }

    // --- Implementasi IActionInput (WAJIB Method) ---
    public bool IsInteractPressed()
    {
        if (isLocked) return false; // Jika dikunci, tidak bisa interact
        return actionInput.IsInteractPressed();
    }
}