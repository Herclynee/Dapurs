using UnityEngine;

public class WanderState : IState
{
    private readonly WildMonster m;
    private float idleTimer;
    private Vector2 target;

    public WanderState(WildMonster m) { this.m = m; }

    public void Enter()
    {
        idleTimer = Random.Range(0.3f, 1.5f);
        target = m.Home + Random.insideUnitCircle * 3f;  // ≤ 3 unit dari Home
    }

    public void Tick(float dt)
    {
        if (m.CanSeeTarget) { m.Fsm.ChangeTo(m.Chase); return; }

        if (idleTimer > 0f) { idleTimer -= dt; m.SetVelocity(Vector2.zero); return; }

        m.MoveToward(target, m.WanderSpeed, dt);
        if (((Vector2)m.transform.position - target).sqrMagnitude < 0.04f)
        {
            idleTimer = Random.Range(0.3f, 1.5f);
            target = m.Home + Random.insideUnitCircle * 3f;
        }
    }

    public void Exit() { }
}

public class ChaseState : IState
{
    private readonly WildMonster m;
    public ChaseState(WildMonster m) { this.m = m; }

    public void Enter() { }

    public void Tick(float dt)
    {
        if (m.LostTarget) { m.Fsm.ChangeTo(m.Return); return; }
        if (m.Target != null) m.MoveToward(m.Target.position, m.ChaseSpeed, dt);
    }

    public void Exit() { }
}

public class ReturnState : IState
{
    private readonly WildMonster m;
    public ReturnState(WildMonster m) { this.m = m; }

    public void Enter() { }

    public void Tick(float dt)
    {
        if (m.CanSeeTarget) { m.Fsm.ChangeTo(m.Chase); return; }
        m.MoveToward(m.Home, m.WanderSpeed * 1.4f, dt);
        if (((Vector2)m.transform.position - m.Home).sqrMagnitude < 0.05f)
            m.Fsm.ChangeTo(m.Wander);   // sampai → Wander
    }

    public void Exit() { }
}