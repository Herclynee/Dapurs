using System;
using System.Collections;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private float secondsPerChar = 0.018f;   // 55 huruf per detik
    [SerializeField] private float evolveSeconds = 2.4f;

    // ===== Event =====
    public event Action Started;
    public event Action<string> MessageShown;
    public event Action ChoiceRequested;
    public event Action<MonsterInstance> Attacked;
    public event Action<MonsterInstance, int, bool, Element?> Hit;   // target, damage, kritis, elemen jurus
    public event Action Missed;
    public event Action<MonsterInstance> Fainted;
    public event Action Victory;
    public event Action<MonsterData> LevelUp;                        // wujud yang ditampilkan
    public event Action<MonsterData, MonsterData> Evolving;
    public event Action<MonsterInstance> Evolved;
    public event Action<BattleResult> Ended;

    // ===== Layanan & data =====
    private PlayerParty party;
    private BattleSession session;
    private SceneFlow scenes;

    public MonsterInstance Player { get; private set; }
    public MonsterInstance Enemy { get; private set; }
    public bool IsBoss { get; private set; }
    public PlayerParty Party => party;
    public int SkillsLeft { get; private set; } = 3;                 // jatah Jurus pemain

    // Urutan sama dengan menu: Serang, Jurus, Ramuan, Kabur
    public BattleAction[] Actions { get; } =
        { new SerangAction(), new JurusAction(), new PotionAction(), new RunAction() };

    private BattleAction chosen;
    private BattleResult result = BattleResult.None;
    private int enemySkillUses;

    // ===== Dipanggil BattleInstaller =====
    public void Construct(PlayerParty playerParty, BattleSession battleSession, SceneFlow sceneFlow)
    {
        party = playerParty;
        session = battleSession;
        scenes = sceneFlow;
        Player = party.Partner;
        Enemy = session.Enemy;
        IsBoss = session.IsBoss;
    }

    public void Begin() => StartCoroutine(Run());
    public void Choose(BattleAction action) => chosen = action;      // dipanggil Presenter saat menu dipilih

    // ===== Dipanggil aksi =====
    public void SpendSkill(MonsterInstance user) { if (user == Player) SkillsLeft = Mathf.Max(0, SkillsLeft - 1); }
    public void SetFled() => result = BattleResult.Flee;
    public float DamageScale(MonsterInstance user) => (user == Enemy && !IsBoss) ? 0.8f : 1f;
    public void RaiseAttacked(MonsterInstance m) => Attacked?.Invoke(m);
    public void RaiseMissed() => Missed?.Invoke();
    public void RaiseHit(MonsterInstance target, int dmg, bool crit, Element? el) => Hit?.Invoke(target, dmg, crit, el);

    // ===== A. Pesan =====
    public IEnumerator Say(string text, float hold = 0.8f)
    {
        MessageShown?.Invoke(text);                                  // View mulai mengetik
        float wait = hold + text.Length * secondsPerChar;
        for (float t = 0f; t < wait; t += Time.deltaTime)
        {
            if (t > text.Length * secondsPerChar && MenuInput.Confirm) break;   // mempercepat
            yield return null;
        }
    }

    // ===== B. Loop giliran =====
    private IEnumerator Run()
    {
        Started?.Invoke();
        yield return new WaitForSeconds(0.5f);                       // tirai pembuka
        yield return Say(IsBoss ? $"{Enemy.Data.DisplayName}, penguasa Gua Petir, muncul!"
                                : $"{Enemy.Data.DisplayName} liar Lv{Enemy.Level} muncul!", 0.9f);
        yield return Say($"Maju, {Player.Data.DisplayName}!", 0.7f);

        while (result == BattleResult.None)
        {
            chosen = null;
            ChoiceRequested?.Invoke();
            yield return new WaitUntil(() => chosen != null);

            if (chosen is RunAction && IsBoss)                       // boss: giliran tidak hilang
            { yield return chosen.Execute(this, Player, Enemy); continue; }

            BattleAction enemyAction = PickEnemyAction();            // 35% Jurus, maks 2 kali
            bool playerFirst = chosen.Priority > 0 || Player.Speed > Enemy.Speed
                || (Player.Speed == Enemy.Speed && UnityEngine.Random.value < 0.5f);

            if (playerFirst) { yield return Turn(chosen, Player, Enemy); yield return Turn(enemyAction, Enemy, Player); }
            else             { yield return Turn(enemyAction, Enemy, Player); yield return Turn(chosen, Player, Enemy); }
        }

        Ended?.Invoke(result);
        yield return new WaitForSeconds(0.3f);                       // jeda sebelum fade
        scenes.EndBattle(result);
    }

    private IEnumerator Turn(BattleAction action, MonsterInstance user, MonsterInstance target)
    {
        if (result != BattleResult.None || user.IsFainted) yield break;   // sudah selesai / pelaku pingsan
        yield return action.Execute(this, user, target);
        if (result != BattleResult.None) yield break;                     // misalnya berhasil kabur

        if (Enemy.IsFainted) yield return Win();
        else if (Player.IsFainted) yield return Lose();
    }

    private BattleAction PickEnemyAction()
    {
        if (enemySkillUses < 2 && UnityEngine.Random.value < 0.35f) { enemySkillUses++; return Actions[1]; }
        return Actions[0];
    }

    // ===== C. Menang =====
    private IEnumerator Win()
    {
        result = BattleResult.Win;
        Fainted?.Invoke(Enemy);                                      // lawan tenggelam & memudar
        yield return new WaitForSeconds(0.6f);
        yield return Say(IsBoss ? $"{Enemy.Data.DisplayName} kalah!" : $"{Enemy.Data.DisplayName} liar kalah!", 0.8f);
        Victory?.Invoke();

        int exp = (10 + Enemy.Level * 8) * (IsBoss ? 2 : 1);
        yield return Say($"{Player.Data.DisplayName} mendapat {exp} EXP.", 0.9f);

        foreach (GrowthEvent growth in Player.GainExp(exp))
        {
            if (growth.Type == GrowthType.LevelUp)
            {
                LevelUp?.Invoke(growth.From);
                yield return Say(growth.Message, 0.9f);
                continue;
            }
            yield return Say($"Apa? {growth.From.DisplayName} mulai bersinar...", 0.9f);
            Evolving?.Invoke(growth.From, growth.To);                // animasi 2,4 detik
            yield return new WaitForSeconds(evolveSeconds);
            Evolved?.Invoke(Player);                                 // kilat putih + sprite baru
            yield return Say(growth.Message, 1.2f);                  // "Pyrit berevolusi menjadi Pyrago!"
        }

        if (!IsBoss)
        {
            int healed = Player.Heal(Mathf.RoundToInt(Player.MaxHP * 0.25f));
            if (healed > 0)
                yield return Say($"{Player.Data.DisplayName} beristirahat sejenak dan pulih {healed} HP.", 0.8f);
        }
    }

    // ===== C. Kalah =====
    private IEnumerator Lose()
    {
        result = BattleResult.Lose;
        Fainted?.Invoke(Player);
        yield return new WaitForSeconds(0.6f);
        yield return Say($"{Player.Data.DisplayName} pingsan!", 0.9f);
        yield return Say("Kamu bergegas kembali ke desa...", 1.0f);
    }
}