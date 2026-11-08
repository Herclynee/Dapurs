using UnityEngine;

public class MonsterDebugger : MonoBehaviour
{
    [SerializeField] private MonsterData species;
    [SerializeField] private int level = 4;

    private MonsterInstance monster;

    [ContextMenu("1. Buat monster")]
    private void Create()
    {
        monster = new MonsterInstance(species, level);
        Debug.Log($"{monster.Data.DisplayName} Lv {monster.Level} | HP {monster.CurrentHP}/{monster.MaxHP} " +
                  $"| ATK {monster.Attack} | ExpToNext {monster.ExpToNext}");
    }

    [ContextMenu("2. Beri 10 damage")]
    private void Hit()
    {
        if (monster == null) { Debug.LogWarning("Jalankan menu 1 dulu"); return; }
        monster.TakeDamage(10);
        Debug.Log($"HP {monster.CurrentHP}/{monster.MaxHP}");
    }

    [ContextMenu("3. Beri 100 EXP")]
    private void GiveExp()
    {
        if (monster == null) { Debug.LogWarning("Jalankan menu 1 dulu"); return; }
        foreach (GrowthEvent e in monster.GainExp(100)) Debug.Log(e.Message);
    }

    [ContextMenu("4. Tes ElementChart")]
    private void TestChart()
    {
        Debug.Log($"Pyro vs Sprout = {ElementChart.Multiplier(Element.Pyro, Element.Sprout)} (harus 1.5)");
        Debug.Log($"Sprout vs Pyro = {ElementChart.Multiplier(Element.Sprout, Element.Pyro)} (harus 0.75)");
        Debug.Log($"Pyro vs Volt = {ElementChart.Multiplier(Element.Pyro, Element.Volt)} (harus 1)");
    }
}