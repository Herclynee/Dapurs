using UnityEngine;

public class WorldInstaller : SceneInstaller
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private PartnerFollower follower;
    [SerializeField] private PartnerHud hud;                  // BARU
    [SerializeField] private Transform respawnPoint;          // BARU: depan Batu Simpan
    [SerializeField] private MonsterData debugStarter;        // sementara, dihapus di P6
    [SerializeField] private int debugStarterLevel = 3;       // (tambahan) naikkan ke 4 untuk uji evolusi

    private GameServices services;

    public override void Install(GameServices s)
    {
        services = s;
        player.Construct(s.Input);
        interactor.Construct(s.Input);
        follower.Construct(s.Party, player);
        hud.Construct(s.Party);

        foreach (WildMonster wild in FindInScene<WildMonster>())
            wild.Construct(s.Battle, s.Scenes, s.Party, player.transform);

        if (s.Party.Partner == null && debugStarter != null)
            s.Party.SetPartner(new MonsterInstance(debugStarter, debugStarterLevel));

        s.Battle.Ended += OnBattleEnded;
    }

    void OnDestroy()
    {
        if (services != null) services.Battle.Ended -= OnBattleEnded;
    }

    private void OnBattleEnded(BattleResult result, bool wasBoss)
    {
        if (result != BattleResult.Lose) return;

        services.Party.Partner.FullHeal();                    // partner pulih agar bisa lanjut
        if (respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
            Physics2D.SyncTransforms();
        }
    }
}