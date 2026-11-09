using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInstance : IDamageable
{
    public MonsterData Data { get; private set; }   // kartu spesies (bisa berganti saat evolusi)
    public int Level { get; private set; }
    public int Exp { get; private set; }
    public int CurrentHP { get; private set; }
    public event Action Changed;                    // P5: HUD mendengarkan ini

    private float Scale => 1f + (Level - 1) * 0.1f;
    public int MaxHP => Mathf.RoundToInt(Data.BaseHP * Scale) + Level * 2;
    public int Attack => Mathf.RoundToInt(Data.BaseATK * Scale);
    public int Defense => Mathf.RoundToInt(Data.BaseDEF * Scale);
    public int Speed => Mathf.RoundToInt(Data.BaseSPD * Scale);
    public int ExpToNext => Level * 10;
    public bool IsFainted => CurrentHP <= 0;

    public MonsterInstance(MonsterData data, int level)
    {
        Data = data;
        Level = Mathf.Max(1, level);
        CurrentHP = MaxHP;                          // lahir dengan HP penuh
    }

    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Changed?.Invoke();
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        Changed?.Invoke();
    }

    public List<GrowthEvent> GainExp(int amount)
    {
        var events = new List<GrowthEvent>();
        Exp += amount;

        while (Exp >= ExpToNext)
        {
            Exp -= ExpToNext;                       // pakai ExpToNext level LAMA
            int oldMax = MaxHP;
            Level++;
            CurrentHP += MaxHP - oldMax;            // HP bertambah sebesar kenaikan MaxHP
            events.Add(new GrowthEvent(GrowthKind.LevelUp,
                $"{Data.DisplayName} naik ke Lv {Level}!"));

            // Evolusi: ada target, ada syarat level, dan level sudah cukup
            if (Data.EvolvesTo != null && Data.EvolveLevel > 0 && Level >= Data.EvolveLevel)
            {
                string oldName = Data.DisplayName;
                Data = Data.EvolvesTo;
                events.Add(new GrowthEvent(GrowthKind.Evolved,
                    $"{oldName} berevolusi menjadi {Data.DisplayName}!"));
            }
        }

        Changed?.Invoke();
        return events;
    }
}