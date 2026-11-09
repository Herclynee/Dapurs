using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class WorldInstaller : SceneInstaller
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private PartnerFollower follower;
    [SerializeField] private PartnerHud hud;
    [SerializeField] private Transform respawnPoint;          // depan Batu Simpan
    [SerializeField] private SaveStone saveStone;

    private GameServices services;

    public override void Install(GameServices s)
    {
        services = s;
        s.Player = player.transform;

        player.Construct(s.Input);
        interactor.Construct(s.Input);
        follower.Construct(s.Party, player);
        hud.Construct(s.Party);

        foreach (WildMonster wild in FindInScene<WildMonster>())
            wild.Construct(s.Battle, s.Scenes, s.Party, player.transform, s.Input, s.Dialogue);
        foreach (Npc npc in FindInScene<Npc>()) npc.Construct(s);
        foreach (FlagToggle t in FindInScene<FlagToggle>()) t.Construct(s.Flags);
        saveStone.Construct(s, player.transform);

        s.Battle.Ended += OnBattleEnded;

        if (s.Save.PendingLoad != null)                       // dari "Lanjutkan"
        {
            WarpPlayer(s.Save.PendingLoad.playerPosition, Vector2.down);
            s.Save.PendingLoad = null;
            s.Toast.Show("Perjalanan dilanjutkan");
        }
        else if (!s.Flags.Has(StoryFlags.IntroSeen))          // permainan baru
        {
            s.Scenes.HoldFade = true;                         // tahan fade-in
            StartCoroutine(Intro());
        }
    }

    void OnDestroy()
    {
        if (services != null) services.Battle.Ended -= OnBattleEnded;
    }

    private IEnumerator Intro()
    {
        services.Dialogue.Run(IntroLines());
        yield return null;
        while (services.Dialogue.IsRunning) yield return null;
        services.Flags.Set(StoryFlags.IntroSeen);
        yield return services.Scenes.ReleaseFade();           // fade-in
        services.Toast.Show("Temui Tetua Rahmat (kanan jalan desa)");
    }

    // Teks persis demo tidak ada di slide: ini kalimat usulan saya. Ganti sesuai modul/versi Anda.
    private IEnumerator IntroLines()
    {
        yield return services.Dialogue.Narrate("Di ujung Pulau Batam ada desa kecil yang dikelilingi hutan dan danau.");
        yield return services.Dialogue.Narrate("Hari ini, petualanganmu bersama monster dimulai...");
    }

    private void OnBattleEnded(BattleResult result, bool wasBoss)
    {
        if (result == BattleResult.Win && wasBoss)
        {
            services.Flags.Set(StoryFlags.BossDefeated);
            StartCoroutine(Ending());
            return;
        }
        if (result == BattleResult.Lose)
        {
            services.Party.Partner.FullHeal();
            if (respawnPoint != null) WarpPlayer(respawnPoint.position, Vector2.up);   // kalah: ke (21,5; 6)
        }
    }

    private IEnumerator Ending()
    {
        while (services.Scenes.Busy) yield return null;            // tunggu fade selesai
        services.Dialogue.Run(EndingLines());
        yield return null;
        while (services.Dialogue.IsRunning) yield return null;

        services.Save.Save(SaveMapper.Capture(services, player.transform.position));   // simpan otomatis
        services.Scenes.GoTo(SceneFlow.EndScene);
    }

    // Baris pertama dari modul; baris kedua (tambahan) usulan saya
    private IEnumerator EndingLines()
    {
        yield return services.Dialogue.Narrate("Raijugar menunduk dan membiarkanmu lewat...");
        yield return services.Dialogue.Narrate("Petir di gua mereda. Perjalananmu di pulau ini baru saja dimulai.");
    }

    // Pindahkan pemain dan beri tahu Cinemachine agar kamera ikut langsung, bukan meluncur
    private void WarpPlayer(Vector2 pos, Vector2 facing)
    {
        Vector3 old = player.transform.position;
        player.Teleport(pos, facing);
        CinemachineCamera cm = FindAnyObjectByType<CinemachineCamera>();
        if (cm != null) cm.OnTargetObjectWarped(player.transform, (Vector3)pos - old);
    }
}