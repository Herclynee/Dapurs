using System;

public class PlayerParty
{
    public MonsterInstance Partner { get; private set; }
    public int Potions { get; private set; } = 3;
    public event Action Changed;
    public event Action PotionsChanged;

    public void SetPartner(MonsterInstance monster) { Partner = monster; Changed?.Invoke(); }

    public bool UsePotion()
    {
        if (Potions <= 0) return false;
        Potions--;
        PotionsChanged?.Invoke();
        return true;
    }

    public void AddPotions(int n) { Potions += n; PotionsChanged?.Invoke(); }
    public void SetPotions(int n) { Potions = Math.Max(0, n); PotionsChanged?.Invoke(); }

    public void Reset()                       
    {
        Partner = null;
        Potions = 3;
        Changed?.Invoke();
        PotionsChanged?.Invoke();
    }
}