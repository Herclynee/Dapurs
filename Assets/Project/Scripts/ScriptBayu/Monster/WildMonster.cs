using UnityEngine;

public class WildMonster : MonoBehaviour
{
    [Header("Monster Data")]
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private int level = 3;

    [Header("Visuals")]
    [SerializeField] private GameObject alertSign;
    [SerializeField] private bool isBoss = false;
    [SerializeField] private bool stationary = false;

    [Header("Settings")]
    [SerializeField] private float sightRadius = 3.25f;
    [SerializeField] private float giveUpRadius = 6.25f;
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float wanderSpeed = 1.125f;
    [SerializeField] private float chaseSpeed = 2.5f;

    public MonsterInstance Instance { get; private set; }
    public bool IsBoss => isBoss;
    public float WanderSpeed => wanderSpeed;
    public float ChaseSpeed => chaseSpeed;

    [System.NonSerialized] public StateMachine fsm;
    
    private Vector2 homePosition;
    private Transform playerTransform;
    private Rigidbody2D rb;

    private bool inBattle = false;

    private BattleSession session;
    private SceneFlow sceneFlow;
    private PlayerParty party;  

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        homePosition = transform.position;
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (monsterData != null)
        {
            Instance = new MonsterInstance(monsterData, level);
        }
        else
        {
            Debug.LogError($"[WildMonster] Data monster pada {gameObject.name} KOSONG!");
        }
        
        SetupFSM();
    }

    void Update()
    {
        if (stationary || inBattle || playerTransform == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (fsm != null)
        {
            fsm.Tick(Time.deltaTime);
        }

        if (alertSign != null && fsm != null)
        {
            alertSign.SetActive(fsm.Current is ChaseState);
        }
    }

    private void SetupFSM()
    {
        fsm = new StateMachine();
        var wander = new WanderState(this);
        var chase = new ChaseState(this);
        var ret = new ReturnState(this);

        wander.SetTransitions(chase, null);
        chase.SetTransitions(null, ret);
        ret.SetTransitions(wander, null);

        fsm.ChangeTo(wander);
    }

    public void MoveTowards(Vector2 target, float speed)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * speed;
    }

    public void Stop() => rb.linearVelocity = Vector2.zero;

    public Vector2 Home => homePosition;
    public Transform Player => playerTransform;
    public float SightRadius => sightRadius;
    public float GiveUpRadius => giveUpRadius;
    public float WanderRadius => wanderRadius;

    public void Construct(BattleSession session, SceneFlow sceneFlow, PlayerParty party, Transform player)
    {
        this.session = session;
        this.sceneFlow = sceneFlow;
        this.party = party;
        if (player != null) this.playerTransform = player;
    }

   private void OnCollisionEnter2D(Collision2D collision)
{
    Debug.Log($"[WildMonster] 🎯 Tabrakan terdeteksi! Objek: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
    
    if (collision.gameObject.CompareTag("Player") && !inBattle)
    {
        Debug.Log("[WildMonster] ✅ Player tertabrak! Cek session...");
        
        if (session == null)
        {
            Debug.LogError("[WildMonster] ❌ Session NULL!");
            return;
        }
        
        if (session.IsSafe)
        {
            Debug.Log("[WildMonster] ⏳ Session masih aman.");
            return;
        }
        
        Debug.Log("[WildMonster] 🚀 Mulai battle...");
        StartBattle();
    }
}

private void StartBattle()
{
    Debug.Log($"[WildMonster] 🎯 StartBattle()! Instance: {(Instance != null ? Instance.Data.DisplayName : "NULL")}");
    
    if (Instance == null)
    {
        Debug.LogError("[WildMonster] ❌ Instance NULL! Isi 'Monster Data' di Inspector.");
        return;
    }

    inBattle = true;
    session.Begin(Instance, IsBoss);
    
    BattleSystem battle = new BattleSystem(party, session, sceneFlow);
    Debug.Log($"[WildMonster] ⚔️ BattleSystem dibuat. Musuh: {battle.Enemy.Data.DisplayName}");
    
    battle.Begin();
    Debug.Log("[WildMonster] 🚀 battle.Begin() dipanggil!");
}
} // <--- INI MENUTUP CLASS WILDMONSTER

// ==========================================
// CLASS STATE (BERADA DI LUAR CLASS WILDMONSTER)
// ==========================================

public class WanderState : IState
{
    private WildMonster ctx;
    private IState toChase, toNext;
    private Vector2 targetPoint;
    private float timer;

    public WanderState(WildMonster context) { ctx = context; }
    public void SetTransitions(IState chase, IState next) { toChase = chase; toNext = next; }

    public void Enter()
    {
        PickNewPoint();
        timer = Random.Range(1f, 3f);
    }

    public void Tick(float dt)
    {
        if (ctx.Player == null) return;

        float dist = Vector2.Distance(ctx.transform.position, ctx.Player.position);
        if (dist < ctx.SightRadius && toChase != null)
        {
            ctx.fsm.ChangeTo(toChase);
            return;
        }

        ctx.MoveTowards(targetPoint, ctx.WanderSpeed);
        timer -= dt;

        if (Vector2.Distance(ctx.transform.position, targetPoint) < 0.1f || timer <= 0)
        {
            PickNewPoint();
            timer = Random.Range(1f, 3f);
        }
    }

    public void Exit() => ctx.Stop();

    private void PickNewPoint()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        targetPoint = ctx.Home + randomDir * ctx.WanderRadius;
    }
}

public class ChaseState : IState
{
    private WildMonster ctx;
    private IState toReturn;

    public ChaseState(WildMonster context) { ctx = context; }
    public void SetTransitions(IState next, IState ret) { toReturn = ret; }

    public void Enter() { }

    public void Tick(float dt)
    {
        if (ctx.Player == null) return;

        float dist = Vector2.Distance(ctx.transform.position, ctx.Player.position);
        
        if (dist > ctx.GiveUpRadius && toReturn != null)
        {
            ctx.fsm.ChangeTo(toReturn);
            return;
        }

        ctx.MoveTowards(ctx.Player.position, ctx.ChaseSpeed);
    }

    public void Exit() => ctx.Stop();
}

public class ReturnState : IState
{
    private WildMonster ctx;
    private IState toWander;

    public ReturnState(WildMonster context) { ctx = context; }
    public void SetTransitions(IState wander, IState next) { toWander = wander; }

    public void Enter() { }

    public void Tick(float dt)
    {
        float dist = Vector2.Distance(ctx.transform.position, ctx.Home);
        
        if (dist < 0.5f && toWander != null)
        {
            ctx.fsm.ChangeTo(toWander);
            return;
        }

        ctx.MoveTowards(ctx.Home, ctx.ChaseSpeed);
    }

    public void Exit() => ctx.Stop();
}