using UnityEngine;

public class WorldInstaller : SceneInstaller
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private PartnerFollower follower;
    [SerializeField] private MonsterData debugStarter;   // sementara, dihapus di P6

    public override void Install(GameServices s)
    {
        player.Construct(s.Input);          // InputGate dipakai sebagai IMoveInput
        interactor.Construct(s.Input);      // dan sebagai IActionInput
        follower.Construct(s.Party, player);

        foreach (WildMonster wild in FindInScene<WildMonster>())
            wild.Construct(s.Battle, s.Scenes, s.Party, player.transform);

        // Belum punya partner? Beri Pyrit Lv 3 agar bisa menguji monster liar
        if (s.Party.Partner == null && debugStarter != null)
            s.Party.SetPartner(new MonsterInstance(debugStarter, 3));
    }
}