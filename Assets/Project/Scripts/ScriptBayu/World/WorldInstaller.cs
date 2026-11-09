using UnityEngine;

public class WorldInstaller : SceneInstaller
{
    [Header("Player")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private PartnerFollower follower;
    [SerializeField] private MonsterData debugStarter;

    [Header("Optional: HUD Partner (Praktikum 6)")]
    [SerializeField] private PartnerHud partnerHud; // Boleh kosong jika belum setup

    public override void Install(GameServices s)
    {
        Debug.Log("[WorldInstaller] Install() dipanggil!");

        // 1. Setup Player & Interactor
        if (player != null) player.Construct(s.Input);
        if (interactor != null) interactor.Construct(s.Input);
        if (follower != null) follower.Construct(s.Party, player);

        // 2. Setup HUD Partner (opsional - Praktikum 6)
        if (partnerHud != null)
        {
            partnerHud.Construct(s.Party);
            Debug.Log("[WorldInstaller] PartnerHud di-setup.");
        }
        else
        {
            Debug.Log("[WorldInstaller] PartnerHud tidak ada (normal jika belum Praktikum 6).");
        }

        // 3. Beri monster awal jika party kosong
        if (s.Party.Partner == null && debugStarter != null)
        {
            s.Party.SetPartner(new MonsterInstance(debugStarter, 3));
            Debug.Log($"[WorldInstaller] Partner awal dibuat: {debugStarter.DisplayName}");
        }

        // 4. Construct semua WildMonster di scene (PENTING!)
        // API Unity 6 terbaru: tanpa parameter FindObjectsSortMode
        var wildMonsters = Object.FindObjectsByType<WildMonster>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Debug.Log($"[WorldInstaller] Ditemukan {wildMonsters.Length} WildMonster di scene.");

        foreach (var wm in wildMonsters)
        {
            wm.Construct(s.Battle, s.Scenes, s.Party, player != null ? player.transform : null);
            Debug.Log($"[WorldInstaller] WildMonster '{wm.gameObject.name}' sudah di-Construct.");
        }

        Debug.Log("[WorldInstaller] Install selesai!");
    }
}