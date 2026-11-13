using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WildMonster : MonoBehaviour
{
    // ==================== SERIALIZED FIELD (Inspector) ====================
    [Header("Data")]
    [SerializeField] private MonsterData species;
    [SerializeField] private int level = 3;

    [Header("FSM (angka sesuai slide)")]
    [SerializeField] private float sightRadius = 3.25f;
    [SerializeField] private float giveUpRadius = 6.25f;
    [SerializeField] private float wanderSpeed = 1.125f;
    [SerializeField] private float chaseSpeed = 2.5f;

    [Header("Boss")]
    [SerializeField] private bool isBoss;
    [SerializeField] private bool stationary;

    [Header("Referensi")]
    [SerializeField] private GameObject alert;   // anak "Alert" (sprite !)
    [SerializeField] private Transform target;   // kosong → cari tag Player otomatis

    // ==================== SERVICE (diisi lewat Construct) ====================
    private BattleSession session;
    private SceneFlow scenes;
    private PlayerParty party;
    private Transform playerTransform;
    private bool inBattle;

    // ==================== FSM & STATE ====================
    public StateMachine Fsm { get; private set; }
    public WanderState Wander { get; private set; }
    public ChaseState Chase { get; private set; }
    public ReturnState Return { get; private set; }

    // ==================== PROPERTY READ-ONLY ====================
    public Vector2 Home { get; private set; }
    public Transform Target => target;
    public float WanderSpeed => wanderSpeed;
    public float ChaseSpeed => chaseSpeed;
    public MonsterData Species => species;
    public int Level => level;
    public float DistanceToTarget =>
        target != null ? Vector2.Distance(transform.position, target.position) : float.MaxValue;

    // ==================== KAPAN MONSTER TENANG (Slide 23) ====================
    private bool Calm => cooldown > 0f || (session != null && session.IsSafe);
    // P6 nanti: || gate.IsLocked

    public bool CanSeeTarget =>
        party != null && party.Partner != null && !Calm && DistanceToTarget < sightRadius;

    public bool LostTarget => Calm || DistanceToTarget > giveUpRadius;

    // ==================== INTERNAL ====================
    private Rigidbody2D rb;
    private Vector2 desiredVelocity;
    private float cooldown;

    // ==================== CONSTRUCT (dipanggil WorldInstaller) ====================
    public void Construct(BattleSession session, SceneFlow scenes,
                          PlayerParty party, Transform playerTransform)
    {
        this.session = session;
        this.scenes = scenes;
        this.party = party;
        this.playerTransform = playerTransform;
        if (target == null && playerTransform != null)
            target = playerTransform;
    }

    // ==================== LIFECYCLE ====================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Home = transform.position;

        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }

        Wander = new WanderState(this);
        Chase = new ChaseState(this);
        Return = new ReturnState(this);
        Fsm = new StateMachine();
        Fsm.ChangeTo(Wander);

        if (alert != null) alert.SetActive(false);
    }

    private void Update()
    {
        if (cooldown > 0f) cooldown -= Time.deltaTime;

        if (stationary || inBattle || Target == null)
            desiredVelocity = Vector2.zero;
        else
            Fsm.Tick(Time.deltaTime);

        rb.linearVelocity = desiredVelocity;

        if (alert != null)
            alert.SetActive(Fsm.Current == Chase && !inBattle);
    }

    // ==================== GERAK ====================
    public void MoveToward(Vector2 point, float speed, float dt)
    {
        Vector2 dir = point - (Vector2)transform.position;
        desiredVelocity = dir.sqrMagnitude < 0.01f ? Vector2.zero : dir.normalized * speed;
    }

    public void SetVelocity(Vector2 v) => desiredVelocity = v;

    // ==================== ENCOUNTER (P4 placeholder → P5 sungguhan) ====================
    private void OnCollisionEnter2D(Collision2D c) => TryEncounter(c);
    private void OnCollisionStay2D(Collision2D c) { if (isBoss) TryEncounter(c); }

    private void TryEncounter(Collision2D c)
    {
        if (!c.collider.CompareTag("Player") || Calm || inBattle) return;
        if (session == null || scenes == null) return;

        inBattle = true;
        desiredVelocity = Vector2.zero;

        var enemy = new MonsterInstance(species, level);

        session.Begin(enemy, isBoss, (result) =>
        {
            inBattle = false;
            if (!isBoss && result == BattleResult.Win)
            {
                gameObject.SetActive(false);
                Invoke(nameof(Respawn), 20f);
            }
        });

        scenes.GoToBattle("Scn_Battle");
    }

    private void Respawn()
    {
        transform.position = Home;
        gameObject.SetActive(true);
        Fsm.ChangeTo(Wander);
    }
}