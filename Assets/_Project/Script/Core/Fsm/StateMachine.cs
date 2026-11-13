public class StateMachine
{
    public IState Current { get; private set; }

    public void ChangeTo(IState next)
    {
        if (next == Current) return;   // hanya satu state aktif
        Current?.Exit();               // Exit lama dulu…
        Current = next;
        Current.Enter();               // …baru Enter baru
    }

    public void Tick(float deltaTime) => Current?.Tick(deltaTime);
}