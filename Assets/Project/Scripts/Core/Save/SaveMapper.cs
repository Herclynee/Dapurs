using UnityEngine;

public static class SaveMapper
{
    public static SaveData Capture(GameServices s, Vector2 playerPosition)
    {
        var data = new SaveData { playerPosition = playerPosition, potions = s.Party.Potions,
                                  playtime = s.Save.Playtime, flags = s.Flags.ToList() };
        MonsterInstance p = s.Party.Partner;
        if (p != null)
        {
            data.partnerId = p.Data.Id;
            data.partnerLevel = p.Level;
            data.partnerExp = p.Exp;
            data.partnerHp = p.CurrentHP;
        }
        return data;
    }

    public static void Apply(SaveData data, GameServices s)
    {
        s.Party.Reset();
        MonsterData species = string.IsNullOrEmpty(data.partnerId) ? null : s.Database.GetById(data.partnerId);
        if (species != null)
        {
            var m = new MonsterInstance(species, data.partnerLevel);
            m.Restore(data.partnerExp, data.partnerHp);
            s.Party.SetPartner(m);
        }
        s.Party.SetPotions(data.potions);
        s.Flags.Load(data.flags);
        s.Save.SetPlaytime(data.playtime);
    }
}