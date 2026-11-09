using UnityEngine;

public class BattleInstaller : SceneInstaller
{
    [Header("References")]
    [SerializeField] private BattleView battleView;

    public override void Install(GameServices s)
    {
        Debug.Log("🔧 [BattleInstaller] Install() DIPANGGIL!");

        var session = s.Battle;
        var sceneFlow = s.Scenes;
        var party = s.Party;

        if (session == null)
        {
            Debug.LogError("[BattleInstaller] ❌ Session NULL!");
            return;
        }

        var battle = session.ActiveBattle;

        if (battle == null)
        {
            Debug.LogError("[BattleInstaller] ❌ ActiveBattle NULL!");
            sceneFlow.GoTo("Scn_World");
            return;
        }

        Debug.Log($"✅ [BattleInstaller] Battle ditemukan: vs {battle.Enemy.Data.DisplayName}");

        if (battleView == null)
        {
            Debug.LogError("[BattleInstaller] ❌ BattleView KOSONG!");
            return;
        }

        // Buat Presenter
        new BattlePresenter(battle, battleView);
        Debug.Log("🎭 [BattleInstaller] Presenter dibuat.");

        // PERBAIKAN: Jalankan StartBattle sebagai Coroutine
        battle.RunBattle();
        Debug.Log("🚀 [BattleInstaller] Battle dimulai sebagai Coroutine!");
    }
}