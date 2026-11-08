using System;
using UnityEngine;

[Serializable]
public class BattleSfx
{
    public AudioClip hit, critical, miss, faint, victory, levelUp, evolve;
}

public class BattleAudio
{
    public BattleAudio(BattleSystem b, AudioService a, BattleSfx c)
    {
        b.Hit      += (target, dmg, crit, el) => a.PlaySfx(crit ? c.critical : c.hit);
        b.Missed   += () => a.PlaySfx(c.miss);
        b.Fainted  += m => a.PlaySfx(c.faint, 0f);
        b.Victory  += () => a.PlaySfx(c.victory, 0f);
        b.LevelUp  += d => a.PlaySfx(c.levelUp, 0f);
        b.Evolving += (from, to) => a.PlaySfx(c.evolve, 0f);
    }
}