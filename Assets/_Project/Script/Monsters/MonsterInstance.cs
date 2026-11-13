using System;
using UnityEngine;

public class MonsterInstance : IDamageable
{
    public MonsterData Data { get; private set; }
    public int Level { get; private set; }
    public int CurrentHP { get; private set; }
    public event Action Changed;               // P5: HUD

    private int exp;

    private float Scale => 1f + (Level - 1) * 0.1f;
    public int MaxHP => Mathf.RoundToInt(Data.BaseHP * Scale) + Level * 2;
    public int Attack => Mathf.RoundToInt(Data.BaseATK * Scale);
    public int Defense => Mathf.RoundToInt(Data.BaseDEF * Scale);
    public int Speed => Mathf.RoundToInt(Data.BaseSPD * Scale);
    public int ExpToNext => Level * 10;

    public MonsterInstance(MonsterData data, int level)
    {
        Data = data;
        Level = Mathf.Max(1, level);
        CurrentHP = MaxHP;
    }

    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Changed?.Invoke();
    }

    public void GainExp(int amount)
    {
        exp += amount;
        while (exp >= ExpToNext)
        {
            exp -= ExpToNext;
            Level++;
            Debug.Log($"{Data.DisplayName} naik ke Lv {Level}!");
            CurrentHP = MaxHP;   // pulih saat naik level

            if (Data.EvolvesTo != null && Data.EvolveLevel > 0
                && Level >= Data.EvolveLevel)
            {
                string lama = Data.DisplayName;
                Data = Data.EvolvesTo;
                Debug.Log($"{lama} berevolusi menjadi {Data.DisplayName}!");
            }
            Changed?.Invoke();
        }
    }
}