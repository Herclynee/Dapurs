using UnityEngine;

public class WildMonster : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject alertSign; // Objek tanda "!"
    [SerializeField] private bool isBoss = false;
    [SerializeField] private bool stationary = false; // Untuk Boss

    [Header("Settings")]
    [SerializeField] private float sightRadius = 3.25f;
    [SerializeField] private float giveUpRadius = 6.25f;
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float wanderSpeed = 1.125f;
    [SerializeField] private float chaseSpeed = 2.5f;
    // Property public agar bisa diakses oleh State
    public float WanderSpeed => wanderSpeed;
    public float ChaseSpeed => chaseSpeed;

    public StateMachine fsm;
    private Vector2 homePosition;
    private Transform playerTransform;
    private Rigidbody2D rb;

    // Placeholder untuk Praktikum 5
    private bool inBattle = false;
    private float cooldown = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        homePosition = transform.position;
        // Cari Player berdasarkan Tag "Player"
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        SetupFSM();
    }

    void Update()
    {
        if (cooldown > 0f) cooldown -= Time.deltaTime;

        // Jika Boss diam, atau sedang battle, atau player tidak ada -> Diam
        if (stationary || inBattle || playerTransform == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        fsm.Tick(Time.deltaTime);

        // Tampilkan tanda "!" hanya saat state Chase
        if (alertSign != null)
        {
            alertSign.SetActive(fsm.Current is ChaseState);
        }
    }

    private void SetupFSM()
    {
        fsm = new StateMachine();
        
        // Buat 3 State
        var wander = new WanderState(this);
        var chase = new ChaseState(this);
        var ret = new ReturnState(this);

        // Atur Transisi
        wander.SetTransitions(chase, null);
        chase.SetTransitions(null, ret);
        ret.SetTransitions(wander, null);

        // Mulai dari Wander
        fsm.ChangeTo(wander);
    }

    // --- Helper Methods untuk State ---
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

    private BattleSession session;
    private SceneFlow sceneFlow;
    private PlayerParty party;  

    public void Construct(BattleSession session, SceneFlow sceneFlow, PlayerParty party, Transform player)
    {
        this.session = session;
        this.sceneFlow = sceneFlow;
        this.party = party;
        this.playerTransform = player;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Jika menabrak player, waktu aman habis, dan belum battle
        if (collision.gameObject.CompareTag("Player") && !session.IsSafe && !inBattle)
        {
            StartBattle();
        }
    }

    private void StartBattle()
    {
        inBattle = true;
        Stop(); // Monster berhenti bergerak
        
        // Kunci input player
        // services.Input.Lock(); (Akan dihandle di SceneFlow/BattleSession nanti)

        // Pindah ke scene battle
        // sceneFlow.GoTo("Scn_Battle");
        Debug.Log("Battle dimulai!");
    }
}

// ==========================================
// CLASS STATE (Bisa ditaruh di file yang sama)
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
        // Cek apakah player dekat? -> Kejar!
        float dist = Vector2.Distance(ctx.transform.position, ctx.Player.position);
        if (dist < ctx.SightRadius && toChase != null)
        {
            ctx.fsm.ChangeTo(toChase); // Perlu akses fsm, kita buat public di WildMonster
            return;
        }

        // Jalan ke titik acak
        ctx.MoveTowards(targetPoint, ctx.WanderSpeed);
        timer -= dt;

        // Jika sampai atau waktu habis, cari titik baru
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
        
        // Jika sudah sampai di rumah -> Wander lagi
        if (dist < 0.5f && toWander != null)
        {
            ctx.fsm.ChangeTo(toWander);
            return;
        }

        ctx.MoveTowards(ctx.Home, ctx.ChaseSpeed);
    }

    public void Exit() => ctx.Stop();
}