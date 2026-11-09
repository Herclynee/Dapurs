using UnityEngine;

public class WanderState : IState
{
    private readonly WildMonster m;
    private Vector2 dir;
    private float timer;

    public WanderState(WildMonster monster) { m = monster; }

    public void Enter() => PickNew();

    public void Tick(float dt)
    {
        if (m.CanSeeTarget) { m.ChangeState(m.Chase); return; }

        Vector2 toHome = m.Home - m.Position;
        if (toHome.magnitude > m.WanderRadius)      // terlalu jauh dari Home: balik arah
        {
            dir = toHome.normalized;
            timer = 1f;
        }
        else
        {
            timer -= dt;
            if (timer <= 0f) PickNew();
        }
        m.SetVelocity(dir * m.WanderSpeed);
    }

    public void Exit() => m.SetVelocity(Vector2.zero);

    private void PickNew()
    {
        // 50% diam, 50% jalan ke arah acak, selama 1-3 detik
        dir = Random.value < 0.5f ? Vector2.zero : Random.insideUnitCircle.normalized;
        timer = Random.Range(1f, 3f);
    }
}

public class ChaseState : IState
{
    private readonly WildMonster m;
    public ChaseState(WildMonster monster) { m = monster; }

    public void Enter() { }

    public void Tick(float dt)
    {
        if (m.LostTarget) { m.ChangeState(m.Return); return; }
        m.SetVelocity(m.DirectionToTarget * m.ChaseSpeed);
    }

    public void Exit() => m.SetVelocity(Vector2.zero);
}

public class ReturnState : IState
{
    private readonly WildMonster m;
    public ReturnState(WildMonster monster) { m = monster; }

    public void Enter() { }

    public void Tick(float dt)
    {
        Vector2 toHome = m.Home - m.Position;
        if (toHome.magnitude < 0.1f) { m.ChangeState(m.Wander); return; }
        m.SetVelocity(toHome.normalized * m.WanderSpeed);
    }

    public void Exit() => m.SetVelocity(Vector2.zero);
}