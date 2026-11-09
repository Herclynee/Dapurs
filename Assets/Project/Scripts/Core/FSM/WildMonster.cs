using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WildMonster : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private MonsterData species;
    [SerializeField] private int minLevel = 2;
    [SerializeField] private int maxLevel = 4;

    [Header("Perilaku")]
    [SerializeField] private bool isBoss;
    [SerializeField] private bool stationary;          // boss: tidak berkeliaran
    [SerializeField] private float sightRadius = 3.25f;
    [SerializeField] private float giveUpRadius = 6.25f;
    [SerializeField] private float wanderSpeed = 1.125f;
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float cooldownSeconds = 2.5f;   // setelah kalah/kabur
    [SerializeField] private float respawnSeconds = 20f;     // setelah dikalahkan

    [Header("Referensi")]
    [SerializeField] private GameObject alert;         // sprite UI_Alert (tanda !)

    // Layanan (diisi lewat Construct)
    private BattleSession session;
    private SceneFlow scenes;
    private PlayerParty party;
    private Transform target;

    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Collider2D col;
    private readonly StateMachine fsm = new StateMachine();
    private Vector2 desiredVelocity;
    private Vector2 home;
    private float cooldown;
    private float respawnAt;
    private bool inBattle;
    private bool hidden;

    // State dibuka agar WildStates bisa memakainya
    public IState Wander { get; private set; }
    public IState Chase { get; private set; }
    public IState Return { get; private set; }

    public Transform Target => target;
    public Vector2 Home => home;
    public Vector2 Position => transform.position;
    public float WanderSpeed => wanderSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float WanderRadius => wanderRadius;

    public float DistanceToTarget => Vector2.Distance(Position, target.position);
    public Vector2 DirectionToTarget => ((Vector2)target.position - Position).normalized;

    // SLIDE
    private bool Calm => cooldown > 0f || session.IsSafe;   // P6: || gate.IsLocked
    public bool CanSeeTarget => party.Partner != null && !Calm
                                && DistanceToTarget < sightRadius;
    public bool LostTarget => Calm || DistanceToTarget > giveUpRadius;

    public void SetVelocity(Vector2 v) => desiredVelocity = v;
    public void ChangeState(IState next) => fsm.ChangeTo(next);

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        home = transform.position;                    // titik Home = posisi awal

        Wander = new WanderState(this);
        Chase = new ChaseState(this);
        Return = new ReturnState(this);

        if (alert != null) alert.SetActive(false);
        fsm.ChangeTo(Wander);
    }

    public void Construct(BattleSession battle, SceneFlow sceneFlow, PlayerParty playerParty, Transform player)
    {
        session = battle;
        scenes = sceneFlow;
        party = playerParty;
        target = player;
    }

    // SLIDE
    void Update()
    {
        if (session == null) return;                  // belum di-Construct (lihat "Play dari Scn_Bootstrap")

        if (hidden)
        {
            if (Time.time >= respawnAt) Respawn();
            return;
        }

        if (cooldown > 0f) cooldown -= Time.deltaTime;
        if (stationary || inBattle || Target == null)
            desiredVelocity = Vector2.zero;
        else fsm.Tick(Time.deltaTime);
        alert.SetActive(fsm.Current == Chase && !inBattle);
    }

    void FixedUpdate() => body.linearVelocity = desiredVelocity;

    void OnCollisionEnter2D(Collision2D c) => TryBattle(c.collider);
    void OnCollisionStay2D(Collision2D c) => TryBattle(c.collider);   // sentuh terus-menerus juga memicu

    private void TryBattle(Collider2D other)
    {
        if (session == null || hidden || inBattle || Calm) return;
        if (!other.CompareTag("Player") || party.Partner == null) return;

        inBattle = true;
        desiredVelocity = Vector2.zero;
        alert.SetActive(false);

        var enemy = new MonsterInstance(species, Random.Range(minLevel, maxLevel + 1));
        session.Begin(enemy, isBoss, OnBattleFinished);   // 1. catat musuh + callback
        scenes.EnterBattle();                             // 2. kunci input, fade, muat Scn_Battle
    }

    private void OnBattleFinished(BattleResult result)
    {
        inBattle = false;
        if (result == BattleResult.Win)
        {
            Hide();
            respawnAt = Time.time + respawnSeconds;
        }
        else cooldown = cooldownSeconds;
    }

    private void Hide()
    {
        hidden = true;
        sprite.enabled = false;
        col.enabled = false;
        desiredVelocity = Vector2.zero;
        alert.SetActive(false);
    }

    private void Respawn()
    {
        transform.position = home;
        hidden = false;
        sprite.enabled = true;
        col.enabled = true;
        cooldown = cooldownSeconds;       // beri jeda agar tidak langsung menabrak pemain
        fsm.ChangeTo(Wander);
    }
}