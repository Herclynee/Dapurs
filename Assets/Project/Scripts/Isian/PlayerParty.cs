using UnityEngine;
using System;

public class PlayerParty
{
    public MonsterInstance Partner { get; private set; }
    public int Potions { get; private set; } = 3;        // 3 Ramuan
    public event Action Changed;

    public void SetPartner(MonsterInstance monster)
    {
        Partner = monster;
        Changed?.Invoke();
    }

    public bool UsePotion()
    {
        if (Potions <= 0) return false;
        Potions--;
        Changed?.Invoke();
        return true;
    }
}