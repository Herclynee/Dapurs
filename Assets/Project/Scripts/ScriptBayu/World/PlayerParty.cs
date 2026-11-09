public class PlayerParty
{
    public MonsterInstance Partner { get; private set; }

    public void SetPartner(MonsterInstance monster)
    {
        Partner = monster;
    }
}
