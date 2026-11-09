public class StateMachine
{
    public IState Current { get; private set; }

    public void ChangeTo(IState next)
    {
        if (next == Current) return;
        
        Current?.Exit();   // Keluar dari state lama
        Current = next;    // Pindah ke state baru
        Current.Enter();   // Masuk ke state baru
    }

    public void Tick(float deltaTime) => Current?.Tick(deltaTime);
}