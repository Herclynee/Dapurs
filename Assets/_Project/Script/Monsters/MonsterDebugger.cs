using UnityEngine;

public class MonsterDebugger : MonoBehaviour
{
    [SerializeField] private MonsterData species;
    [SerializeField] private int level = 4;
    private MonsterInstance monster;

    [ContextMenu("1. Buat monster")]
    private void BuatMonster()
    {
        monster = new MonsterInstance(species, level);
        Debug.Log($"{monster.Data.DisplayName} Lv {monster.Level} | " +
                  $"MaxHP {monster.MaxHP} | ATK {monster.Attack} | " +
                  $"ExpToNext {monster.ExpToNext}");
    }

    [ContextMenu("2. Cek status")]
    private void CekStatus()
    {
        if (monster == null) { Debug.LogWarning("Buat monster dulu!"); return; }
        Debug.Log($"{monster.Data.DisplayName} Lv {monster.Level} | " +
                  $"HP {monster.CurrentHP}/{monster.MaxHP}");
    }

    [ContextMenu("3. Beri 100 EXP")]
    private void Beri100Exp()
    {
        if (monster == null) { Debug.LogWarning("Buat monster dulu!"); return; }
        monster.GainExp(100);
    }

    [ContextMenu("4. Uji TakeDamage")]
    private void UjiDamage()
    {
        if (monster == null) { Debug.LogWarning("Buat monster dulu!"); return; }
        monster.TakeDamage(10);
        Debug.Log($"HP sekarang: {monster.CurrentHP}/{monster.MaxHP}");
    }
}