using UnityEngine;

public static class DamageFormula
{
    public const float CriticalChance = 0.08f; // 8% peluang kritis

    public static int Calculate(MonsterInstance user, MonsterInstance target, 
                                int power, float multiplier, out bool critical)
    {
        // 1. Cek Critical Hit
        critical = Random.value < CriticalChance;
        
        // 2. Rumus dasar damage
        float raw = user.Attack * power / (target.Defense * 0.5f + 10f);
        
        // 3. Kalikan dengan multiplier elemen, critical (1.5x), dan variasi acak (0.9 - 1.1)
        raw *= multiplier * (critical ? 1.5f : 1f) * Random.Range(0.9f, 1.1f);
        
        // 4. Minimal damage adalah 1
        return Mathf.Max(1, Mathf.FloorToInt(raw));
    }
}