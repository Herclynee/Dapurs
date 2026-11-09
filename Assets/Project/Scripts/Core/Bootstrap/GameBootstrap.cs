using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private string firstScene = "Scn_Title";
    [SerializeField] private MonsterDatabase monsterDatabase;
    [SerializeField] private ScreenFader fader;
    [SerializeField] private DialogueRunner dialogue;
    [SerializeField] private ToastView toast;
    [SerializeField] private AudioService audioService;
    [SerializeField] private PauseMenu pauseMenu;

    private GameServices services;

    void Awake()
    {
        var gate = new InputGate(new InputSystemMoveInput(), new InputSystemActionInput());
        services = new GameServices
        {
            Input = gate,
            Database = monsterDatabase,
            Party = new PlayerParty(),
            Battle = new BattleSession(),
            Flags = new StoryFlags(),
            Save = new SaveService(0),
            Dialogue = dialogue,
            Toast = toast,
            Audio = audioService,
        };
        services.Scenes = new SceneFlow(this, fader, services);

        dialogue.Construct(gate, audioService);
        pauseMenu.Construct(services);
        TextMenu.Sfx = audioService;
    }

    void Start() => services.Scenes.GoTo(firstScene);

    void Update()
    {
        // Battle dimuat DI ATAS dunia, jadi scene utama tetap Scn_World selama battle
        if (services != null && services.Scenes.CurrentSceneName == SceneFlow.WorldScene)
            services.Party.Playtime += Time.unscaledDeltaTime;
    }
}