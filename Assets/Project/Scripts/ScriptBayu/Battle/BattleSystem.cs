using System;
using System.Collections;
using UnityEngine;

public class BattleSystem
{
    // --- Events (Sinyal ke UI/View) ---
    public event Action Started;
    public event Action<string> MessageShown;
    public event Action ChoiceRequested;
    public event Action<MonsterInstance> Attacked;
    public event Action Missed;
    public event Action<MonsterInstance, int, bool, Element?> Hit;
    public event Action<MonsterInstance> Fainted;
    public event Action Victory;
    public event Action<MonsterInstance> LevelUp;
    public event Action<MonsterInstance, MonsterInstance> Evolving;
    public event Action<MonsterInstance> Evolved;
    public event Action<BattleResult> Ended;

    // --- Properties ---
    public MonsterInstance Player { get; private set; }
    public MonsterInstance Enemy { get; private set; }
    public bool IsBoss { get; private set; }
    public BattleResult Result { get; private set; } = BattleResult.None;

    private BattleSession session;
    private SceneFlow scenes;
    private BattleAction chosen;
    private const float secondsPerChar = 0.018f;
    
    private MonoBehaviourHelper runner;
    private bool battleStarted = false; // Flag agar tidak double start

    // --- Constructor ---
    public BattleSystem(PlayerParty party, BattleSession session, SceneFlow scenes)
    {
        Player = party.Partner;
        Enemy = session.Enemy;
        IsBoss = session.IsBoss;
        this.session = session;
        this.scenes = scenes;
    }

    // --- Method untuk dipanggil dari UI (Praktikum 5) ---
    public void ChooseAction(BattleAction action)
    {
        chosen = action;
    }

    // --- Method yang dipanggil oleh BattleAction (Praktikum 3) ---
    public float DamageScale(MonsterInstance user)
    {
        return user == Enemy ? 0.8f : 1.0f;
    }

    public void RaiseAttacked(MonsterInstance user) => Attacked?.Invoke(user);
    public void RaiseMissed() => Missed?.Invoke();
    
    public void RaiseHit(MonsterInstance target, int damage, bool critical, Element? element)
    {
        Hit?.Invoke(target, damage, critical, element);
    }

    public void Flee()
    {
        Result = BattleResult.Flee;
    }

    // ==========================================
    // PERBAIKAN ALUR DI SINI
    // ==========================================

    // 1. Dipanggil oleh WildMonster: Simpan diri & Minta pindah scene
    public void Begin()
{
    session.ActiveBattle = this;
    scenes.GoTo("Scn_Battle");
    // HAPUS baris ini: runner.StartCoroutine(Run());
}
    public void RunBattle()
{
    if (runner == null)
    {
        runner = new GameObject("BattleRunner").AddComponent<MonoBehaviourHelper>();
    }
    runner.StartCoroutine(StartBattle());
}

    // 2. Dipanggil oleh BattleInstaller SETELAH scene Scn_Battle dimuat
    public IEnumerator StartBattle()
    {
        if (battleStarted) yield break; // Cegah double start
        battleStarted = true;

        // Buat runner di scene yang baru (Scn_Battle)
        runner = new GameObject("BattleRunner").AddComponent<MonoBehaviourHelper>();

        Started?.Invoke();
        yield return new WaitForSeconds(0.5f);

        // Intro
        string intro = IsBoss 
            ? $"{Enemy.Data.DisplayName}, penguasa area ini, muncul!" 
            : $"{Enemy.Data.DisplayName} liar Lv{Enemy.Level} muncul!";
        yield return Say(intro, 0.9f);
        yield return Say($"Maju, {Player.Data.DisplayName}!", 0.7f);

        // Loop Battle
        while (Result == BattleResult.None)
        {
            chosen = null;
            ChoiceRequested?.Invoke(); // Minta UI menampilkan menu
            
            // Tunggu pemain memilih
            yield return new WaitUntil(() => chosen != null);

            // Cek kabur dari boss
            if (chosen is RunAction && IsBoss)
            {
                yield return Say("Tidak bisa kabur dari Boss!", 0.8f);
                continue;
            }

            // Aksi Musuh (Sederhana: 35% Jurus, 65% Serang)
            BattleAction enemyAction = UnityEngine.Random.value < 0.35f 
                ? (BattleAction)new SkillAction() 
                : new AttackAction();

            // Urutan giliran berdasarkan Speed
            bool playerFirst = chosen.Priority > enemyAction.Priority 
                || (chosen.Priority == enemyAction.Priority && Player.Speed >= Enemy.Speed);

            if (playerFirst)
            {
                yield return ExecuteTurn(chosen, Player, Enemy);
                if (Result != BattleResult.None) break;
                yield return ExecuteTurn(enemyAction, Enemy, Player);
            }
            else
            {
                yield return ExecuteTurn(enemyAction, Enemy, Player);
                if (Result != BattleResult.None) break;
                yield return ExecuteTurn(chosen, Player, Enemy);
            }

            if (Result != BattleResult.None) break;
        }

        yield return new WaitForSeconds(0.3f);
        Ended?.Invoke(Result);
        
        // Selesai battle, kembali ke dunia
        session.Finish(Result);
        scenes.GoTo("Scn_World");
        
        // Hancurkan helper
        if (runner != null)
        {
            UnityEngine.Object.Destroy(runner.gameObject);
        }
    }

    private IEnumerator ExecuteTurn(BattleAction action, MonsterInstance user, MonsterInstance target)
    {
        yield return action.Execute(this, user, target);

        if (target.CurrentHP <= 0)
        {
            Fainted?.Invoke(target);
            yield return new WaitForSeconds(0.6f);
            
            if (target == Enemy) HandleWin();
            else if (target == Player) HandleLose();
        }
    }

    private void HandleWin()
    {
        Result = BattleResult.Win;
        Victory?.Invoke();
    
        int exp = (10 + Enemy.Level * 8) * (IsBoss ? 2 : 1);
        runner.StartCoroutine(ProcessWin(exp));
    }

    private IEnumerator ProcessWin(int exp)
    {
        yield return Say($"{Player.Data.DisplayName} mendapat {exp} EXP.", 0.9f);

        foreach (GrowthEvent growth in Player.GainExp(exp))
        {
            if (growth.Type == GrowthType.LevelUp)
            {
                LevelUp?.Invoke(growth.From);
                yield return Say(growth.Message, 0.9f);
            }
            else if (growth.Type == GrowthType.Evolve)
            {
                Evolving?.Invoke(growth.From, growth.To);
                yield return new WaitForSeconds(2.4f); // Durasi animasi evolusi
                Evolved?.Invoke(Player);
                yield return Say(growth.Message, 1.2f);
            }
        }

        // Heal 25% setelah battle (kecuali boss)
        if (!IsBoss)
        {
            int healed = Player.Heal(Mathf.RoundToInt(Player.MaxHP * 0.25f));
            if (healed > 0) yield return Say($"{Player.Data.DisplayName} beristirahat dan pulih {healed} HP.", 0.8f);
        }
    }

    private void HandleLose()
    {
        Result = BattleResult.Lose;
    }

    // --- System Pesan (Say) ---
    public IEnumerator Say(string text, float hold = 0.8f)
    {
        MessageShown?.Invoke(text);
        float wait = hold + text.Length * secondsPerChar;
        
        for (float t = 0f; t < wait; t += Time.deltaTime)
        {
            // Skip pesan jika tekan Confirm
            if (t > text.Length * secondsPerChar && MenuInput.Confirm) break;
            yield return null;
        }
    }
}

// Helper class agar BattleSystem bisa menjalankan Coroutine tanpa MonoBehaviour
public class MonoBehaviourHelper : MonoBehaviour { }