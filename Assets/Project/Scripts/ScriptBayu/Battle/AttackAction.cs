using System.Collections;
using UnityEngine;

public class AttackAction : BattleAction
{
    public override int Priority => 0;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        string who = user.Data.DisplayName;
        yield return battle.Say($"{who} menyerang!", 0.7f);

        // Peluang meleset 6%
        if (Random.value < 0.06f)
        {
            yield return battle.Say("Serangan meleset!", 0.8f);
            yield break;
        }

        // Hitung damage
        float bonus = ElementChart.Multiplier(user.Data.Element, target.Data.Element);
        int damage = DamageFormula.Calculate(user, target, 14, bonus, out bool critical);
        
        target.TakeDamage(damage);
        battle.RaiseHit(target, damage, critical, null);
        yield return new WaitForSeconds(0.45f);

        if (critical) yield return battle.Say("Serangan kritis!", 0.7f);
        if (bonus > 1f) yield return battle.Say("Sangat efektif!", 0.7f);
        else if (bonus < 1f) yield return battle.Say("Kurang efektif...", 0.7f);
    }
}
