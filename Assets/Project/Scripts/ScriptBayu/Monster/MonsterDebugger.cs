using UnityEngine;

public class MonsterDebugger : MonoBehaviour
{
    [SerializeField] private MonsterData species;
    [SerializeField] private int startLevel = 1;

    private MonsterInstance monster;

    // 1. Klik ini untuk membuat monster
    [ContextMenu("1. Buat Monster")]
    public void CreateMonster()
    {
        if (species == null) {
            Debug.LogError("Species kosong!");
            return;
        }
        monster = new MonsterInstance(species, startLevel);
        PrintStatus();
    }

    // 2. Klik ini untuk lihat status
    [ContextMenu("2. Tampilkan Status")]
    public void PrintStatus()
    {
        if (monster == null) { Debug.Log("Belum ada monster."); return; }
        Debug.Log($"[ {monster.Data.DisplayName} Lv.{monster.Level} ] " +
                  $"HP: {monster.CurrentHP}/{monster.MaxHP} | " +
                  $"ATK: {monster.Attack} | " +
                  $"EXP: {monster.Exp}/{monster.ExpToNext}");
    }

    // 3. Klik ini untuk simulasi battle (tambah 100 EXP)
    [ContextMenu("3. Beri 100 EXP")]
    public void GiveExp()
    {
        if (monster == null) return;
        monster.GainExp(100);
        PrintStatus();
    }
    
    // 4. Klik ini untuk simulasi damage
    [ContextMenu("4. Ambil 10 Damage")]
    public void TakeDmg()
    {
        if (monster == null) return;
        monster.TakeDamage(10);
        PrintStatus();
    }
}
