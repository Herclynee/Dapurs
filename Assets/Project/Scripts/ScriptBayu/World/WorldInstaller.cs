using UnityEngine;

public class WorldInstaller : SceneInstaller
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private PartnerFollower follower;
    [SerializeField] private MonsterData debugStarter;

    public override void Install(GameServices s)
    {
        if (player != null) player.Construct(s.Input);
        if (interactor != null) interactor.Construct(s.Input);
        if (follower != null) follower.Construct(s.Party, player);

        // Beri monster awal jika party kosong (Untuk testing)
        if (s.Party.Partner == null && debugStarter != null)
        {
            s.Party.SetPartner(new MonsterInstance(debugStarter, 3));
        }
    }
}