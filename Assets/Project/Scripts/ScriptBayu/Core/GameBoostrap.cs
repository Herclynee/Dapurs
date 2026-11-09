using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string firstScene = "Scn_World";
    
    [Header("References")]
    [SerializeField] private MonsterDatabase monsterDatabase;
    [SerializeField] private ScreenFader fader;

    private GameServices services;

    void Awake()
    {
        // 1. JANGAN hancurkan GameBootstrap saat pindah scene
        DontDestroyOnLoad(gameObject);

        // 2. JANGAN hancurkan Canvas (induk dari fader) saat pindah scene
        if (fader != null)
        {
            // fader.transform.root.gameObject akan mengambil objek Canvas
            DontDestroyOnLoad(fader.transform.root.gameObject);
        }

        // 3. Buat Input Gate
        var gate = new InputGate(new InputSystemMoveInput(), new InputSystemActionInput());

        // 4. Inisialisasi GameServices
        services = new GameServices
        {
            Input = gate,
            Database = monsterDatabase,
            Party = new PlayerParty(),
            Battle = new BattleSession()
        };

        // 5. Inisialisasi SceneFlow
        services.Scenes = new SceneFlow(this, fader, services);
    }

    void Start()
    {
        // 6. Pindah ke scene pertama
        services.Scenes.GoTo(firstScene);
    }
}