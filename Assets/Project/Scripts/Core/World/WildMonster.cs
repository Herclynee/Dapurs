using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class WildMonster : MonoBehaviour
{
    // ===== A. DATA DARI INSPECTOR =====
    [Header("Data")]
    [SerializeField] private MonsterData species;     // spesies lawan di battle
    [SerializeField] private int minLevel = 2;
    [SerializeField] private int maxLevel = 4;

    [Header("Perilaku")]
    [SerializeField] private bool isBoss;
    [SerializeField] private bool stationary;         // true = tidak bergerak (boss)
    [SerializeField] private float sightRadius = 3.25f;
    [SerializeField] private float giveUpRadius = 6.25f;
    [SerializeField] private float wanderSpeed = 1.125f;
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float cooldownSeconds = 2.5f;
    [SerializeField] private float respawnSeconds = 20f;

    [Header("Referensi")]
    [SerializeField] private GameObject alert;        // anak objek bertanda !

    [Header("Debug (tambahan)")]
    [SerializeField] private bool logStates;

    // ===== B. LAYANAN (diisi oleh WorldInstaller lewat Construct) =====
    private BattleSession session;
    private SceneFlow scenes;
    private PlayerParty party;
    private Transform target;                          // Player
    private InputGate gate;
    private DialogueRunner dialogue;

    // ===== C. KEADAAN INTERNAL =====
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Collider2D col;
    private readonly StateMachine fsm = new StateMachine();
    private Vector2 desiredVelocity;                   // kecepatan yang DIINGINKAN state
    private Vector2 home;
    private float cooldown;
    private float respawnAt;
    private bool inBattle;
    private bool hidden;

    // ===== D. YANG DIBACA OLEH STATE =====
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

    // "Tenang" = monster sedang tidak boleh agresif
    private bool Calm => cooldown > 0f || session.IsSafe || gate.IsLocked;      // diam saat dialog/jeda
    public bool CanSeeTarget => party.Partner != null && !Calm && DistanceToTarget < sightRadius;
    public bool LostTarget => Calm || DistanceToTarget > giveUpRadius;

    public void SetVelocity(Vector2 v) => desiredVelocity = v;

    public void ChangeState(IState next)
    {
        if (logStates && next != fsm.Current)
            Debug.Log($"{name}: {fsm.Current?.GetType().Name} -> {next.GetType().Name}");
        fsm.ChangeTo(next);
    }

    // ===== E. SIKLUS HIDUP =====
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        home = transform.position;                 // posisi awal = Home

        Wander = new WanderState(this);
        Chase = new ChaseState(this);
        Return = new ReturnState(this);

        SetAlert(false);
        fsm.ChangeTo(Wander);                      // state awal
    }

    // Dipanggil WorldInstaller: "ini peralatanmu"
    public void Construct(BattleSession battle, SceneFlow sceneFlow, PlayerParty playerParty,
                      Transform player, InputGate inputGate, DialogueRunner dialogueRunner)
    {
        session = battle; scenes = sceneFlow; party = playerParty; target = player;
        gate = inputGate; dialogue = dialogueRunner;
    }

    void Update()
    {
        if (session == null) return;               // belum diberi layanan → diam saja

        if (hidden)                                // sedang "mati", tunggu respawn
        {
            if (Time.time >= respawnAt) Respawn();
            return;
        }

        if (cooldown > 0f) cooldown -= Time.deltaTime;

        if (stationary || inBattle || target == null)
            desiredVelocity = Vector2.zero;        // diam
        else
            fsm.Tick(Time.deltaTime);              // biarkan state aktif bekerja

        SetAlert(fsm.Current == Chase && !inBattle);   // tanda ! hanya saat Chase
    }

    void FixedUpdate() => body.linearVelocity = desiredVelocity;   // fisika menerapkan gerakan

    // ===== F. SENTUHAN → BATTLE =====
    void OnCollisionEnter2D(Collision2D c) => TryBattle(c.collider);
    void OnCollisionStay2D(Collision2D c) => TryBattle(c.collider);   // bersentuhan terus pun memicu

    private void TryBattle(Collider2D other)
    {
        if (session == null || hidden || inBattle || Calm) return;
        if (!other.CompareTag("Player") || party.Partner == null) return;

        inBattle = true;
        desiredVelocity = Vector2.zero;
        SetAlert(false);

        if (isBoss) StartCoroutine(BossIntro());
        else StartBattle();
    }

    private IEnumerator BossIntro()                          // auman dulu, baru battle
    {
        dialogue.Run(Roar());
        yield return null;
        while (dialogue.IsRunning) yield return null;
        StartBattle();
    }

    private IEnumerator Roar()
    {
        yield return dialogue.Say("", $"GRRRAAAH!! {species.DisplayName} menghalangi jalan, percikan listrik memenuhi gua!");
    }

    private void StartBattle()
    {
        var enemy = new MonsterInstance(species, Random.Range(minLevel, maxLevel + 1));
        session.Begin(enemy, isBoss, OnBattleFinished);
        scenes.EnterBattle();
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

    // ===== G. HILANG & MUNCUL LAGI =====
    private void Hide()
    {
        hidden = true;
        sprite.enabled = false;
        col.enabled = false;
        desiredVelocity = Vector2.zero;
        SetAlert(false);
    }

    private void Respawn()
    {
        transform.position = home;
        hidden = false;
        sprite.enabled = true;
        col.enabled = true;
        cooldown = cooldownSeconds;
        fsm.ChangeTo(Wander);
    }

    private void SetAlert(bool on)
    {
        if (alert != null) alert.SetActive(on);
    }

    [ContextMenu("DEBUG: status")]
    private void DebugStatus()
    {
        Debug.Log(
            $"{name} | session={(session != null)} partner={(party != null && party.Partner != null)} target={(target != null)}\n" +
            $"stationary={stationary} inBattle={inBattle} hidden={hidden} cooldown={cooldown:F1} calm={(session != null && Calm)}\n" +
            $"state={fsm.Current?.GetType().Name} desiredVel={desiredVelocity} " +
            $"rbType={body.bodyType} rbSimulated={body.simulated} rbVel={body.linearVelocity} constraints={body.constraints}\n" +
            (target != null ? $"jarakKePlayer={DistanceToTarget:F2}" : "target=NULL"));
    }

    // ===== H. GIZMO (tambahan): lingkaran di Scene view saat objek dipilih =====
    void OnDrawGizmosSelected()
    {
        Vector3 homePos = Application.isPlaying ? (Vector3)home : transform.position;
        Gizmos.color = Color.green;  Gizmos.DrawWireSphere(homePos, wanderRadius);           // tali ke Home
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, sightRadius); // mulai mengejar
        Gizmos.color = Color.red;    Gizmos.DrawWireSphere(transform.position, giveUpRadius);// menyerah
    }
}