public class PlayerParty
{
    public MonsterInstance Partner { get; private set; }
    public int Ramuan { get; private set; } = 3;
    public void SetPartner(MonsterInstance m) { Partner = m; }
    public bool UseRamuan() { if (Ramuan <= 0) return false; Ramuan--; return true; }
}