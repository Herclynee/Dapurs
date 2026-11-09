using UnityEngine;

public class StateMachine
{
    public IState Current { get; private set; }

    public void ChangeTo(IState next)
    {
        if (next == Current) return;   // sudah di state itu, abaikan
        Current?.Exit();               // 1. keluar dari yang lama
        Current = next;                // 2. ganti
        Current.Enter();               // 3. masuk yang baru
    }

    public void Tick(float deltaTime) => Current?.Tick(deltaTime);
}