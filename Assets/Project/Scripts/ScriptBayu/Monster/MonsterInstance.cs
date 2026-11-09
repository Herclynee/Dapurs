using System.Collections.Generic;
using UnityEngine;

public enum GrowthType { LevelUp, Evolve }

public class GrowthEvent
{
    public GrowthType Type;
    public MonsterInstance From; // Data lama
    public MonsterInstance To;   // Data baru (jika evolusi)
    public string Message;
}

public class MonsterInstance
{
    public MonsterData Data { get; private set; }
    public int Level { get; private set; }
    public int CurrentHP { get; private set; }
    public int Exp { get; private set; }

    private float Scale => 1f + (Level - 1) * 0.1f;

    public int MaxHP => Mathf.RoundToInt(Data.BaseHP * Scale) + Level * 2;
    public int Attack => Mathf.RoundToInt(Data.BaseATK * Scale);
    public int Defense => Mathf.RoundToInt(Data.BaseDEF * Scale);
    public int Speed => Mathf.RoundToInt(Data.BaseSPD * Scale);
    public int ExpToNext => Level * 10;

    public MonsterInstance(MonsterData data, int level)
    {
        Data = data;
        Level = level;
        CurrentHP = MaxHP;
        Exp = 0;
    }

    public void TakeDamage(int amount) => CurrentHP = Mathf.Max(0, CurrentHP - amount);

    public int Heal(int amount)
    {
        int before = CurrentHP;
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        return CurrentHP - before;
    }

    // Method baru: Mengembalikan list peristiwa (Level Up / Evolusi)
    public List<GrowthEvent> GainExp(int amount)
    {
        List<GrowthEvent> events = new List<GrowthEvent>();
        Exp += amount;

        while (Exp >= ExpToNext && Level < 9)
        {
            Exp -= ExpToNext;
            Level++;
            CurrentHP = MaxHP; // Full heal saat level up

            GrowthEvent levelUp = new GrowthEvent
            {
                Type = GrowthType.LevelUp,
                From = new MonsterInstance(Data, Level - 1),
                Message = $"{Data.DisplayName} naik ke Level {Level}!"
            };
            events.Add(levelUp);

            // Cek evolusi
            if (Data.EvolvesTo != null && Level >= Data.EvolveLevel)
            {
                MonsterInstance oldForm = new MonsterInstance(Data, Level);
                Data = Data.EvolvesTo; // Ganti data jadi bentuk evolusi
                CurrentHP = MaxHP;     // Refresh HP dengan stat baru

                GrowthEvent evolve = new GrowthEvent
                {
                    Type = GrowthType.Evolve,
                    From = oldForm,
                    To = new MonsterInstance(Data, Level),
                    Message = $"{oldForm.Data.DisplayName} berevolusi menjadi {Data.DisplayName}!"
                };
                events.Add(evolve);
            }
        }
        return events;
    }
}