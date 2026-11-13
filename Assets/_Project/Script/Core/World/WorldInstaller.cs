using UnityEngine;

public class WorldInstaller : SceneInstaller
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private MonsterData debugStarter;
    [SerializeField] private PartnerFollower follower;

    // TAMBAHKAN INI: Fallback jika Install() tidak dipanggil SceneFlow
    private void Awake()
    {
        // Cek apakah sudah di-Install oleh SceneFlow
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerMovement>();
            interactor = FindAnyObjectByType<PlayerInteractor>();
        }
    }

    public override void Install(GameServices s)
    {
        if (player != null) player.Construct(s.Input);
        if (interactor != null) interactor.Construct(s.Input);

        foreach (WildMonster wild in FindObjectsByType<WildMonster>())
        {
            wild.Construct(s.Battle, s.Scenes, s.Party, player.transform);
        }

        if (s.Party.Partner == null && debugStarter != null)
        {
            s.Party.SetPartner(new MonsterInstance(debugStarter, 3));
        }

        if (follower != null) follower.Construct(s.Party, player);
    }
}