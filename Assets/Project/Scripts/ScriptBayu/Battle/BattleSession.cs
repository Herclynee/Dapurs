using System;
using UnityEngine;

public enum BattleResult { None, Win, Lose, Flee }

public class BattleSession
{
    public const float SafeSeconds = 2.2f;
    
    public MonsterInstance Enemy { get; private set; }
    public bool IsBoss { get; private set; }
    public bool IsSafe => Time.time < safeUntil;
    
    // PASTIKAN BARIS INI ADA:
    public BattleSystem ActiveBattle { get; set; }

    private float safeUntil;

    public void Begin(MonsterInstance enemy, bool isBoss)
    {
        Enemy = enemy;
        IsBoss = isBoss;
    }

    public void Finish(BattleResult result)
    {
        safeUntil = Time.time + SafeSeconds;
        Enemy = null;
        IsBoss = false;
        ActiveBattle = null; // Reset saat selesai
    }
}