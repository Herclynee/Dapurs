using System.Collections;
using UnityEngine;

public class SkillAction : BattleAction
{
    public override int Priority => 0;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        string skillName = user.Data.SkillName;
        yield return battle.Say($"{user.Data.DisplayName} memakai {skillName}!", 0.7f);

        // Peluang meleset lebih kecil (3%)
        if (Random.value < 0.03f)
        {
            yield return battle.Say("Jurus meleset!", 0.8f);
            yield break;
        }

        // Damage lebih besar (Power 22) dan pakai multiplier elemen penuh
        float bonus = ElementChart.Multiplier(user.Data.Element, target.Data.Element);
        int damage = DamageFormula.Calculate(user, target, 22, bonus, out bool critical);
        
        target.TakeDamage(damage);
        battle.RaiseHit(target, damage, critical, user.Data.Element);
        yield return new WaitForSeconds(0.45f);

        if (critical) yield return battle.Say("Serangan kritis!", 0.7f);
        if (bonus > 1f) yield return battle.Say("Sangat efektif!", 0.7f);
        else if (bonus < 1f) yield return battle.Say("Kurang efektif...", 0.7f);
    }
}
