using System.Collections;
using UnityEngine;

public abstract class BattleAction
{
    public virtual int Priority => 0;
    public abstract IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target);
}

public abstract class AttackAction : BattleAction
{
    protected abstract int Power { get; }
    protected virtual string Move(MonsterInstance user) => null;                              // null = serang biasa
    protected virtual float ElementBonus(MonsterInstance user, MonsterInstance target) => 1f;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        string who = (user == battle.Enemy && !battle.IsBoss ? "Lawan " : "") + user.Data.DisplayName;
        string move = Move(user);

        yield return battle.Say(move == null ? $"{who} menyerang!" : $"{who} memakai {move}!", 0.7f);
        battle.RaiseAttacked(user);                                  // animasi menerjang
        yield return new WaitForSeconds(0.2f);

        if (Random.value < 0.06f)                                    // meleset 6%
        {
            battle.RaiseMissed();
            yield return battle.Say("Serangan meleset!", 0.8f);
            yield break;
        }

        float bonus = ElementBonus(user, target);
        int damage = DamageFormula.Calculate(user, target, Power,
                         bonus * battle.DamageScale(user), out bool critical);
        target.TakeDamage(damage);
        battle.RaiseHit(target, damage, critical, move != null ? user.Data.Element : (Element?)null);
        yield return new WaitForSeconds(0.45f);

        if (critical) yield return battle.Say("Serangan kritis!", 0.7f);
        if (bonus > 1f) yield return battle.Say("Sangat efektif!", 0.7f);
        else if (bonus < 1f) yield return battle.Say("Kurang efektif...", 0.7f);
    }
}

public class SerangAction : AttackAction
{
    protected override int Power => 14;
}

public class JurusAction : AttackAction
{
    protected override int Power => 22;
    protected override string Move(MonsterInstance user) => user.Data.SkillName;
    protected override float ElementBonus(MonsterInstance user, MonsterInstance target)
        => ElementChart.Multiplier(user.Data.Element, target.Data.Element);

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        battle.SpendSkill(user);                                     // kurangi jatah 3×
        return base.Execute(battle, user, target);
    }
}

public class PotionAction : BattleAction
{
    public override int Priority => 1;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        if (!battle.Party.UsePotion())
        {
            yield return battle.Say("Ramuan sudah habis!", 0.8f);
            yield break;
        }
        int healed = user.Heal(Mathf.RoundToInt(user.MaxHP * 0.4f));
        yield return battle.Say($"{user.Data.DisplayName} meminum Ramuan dan pulih {healed} HP.", 0.9f);
    }
}

public class RunAction : BattleAction
{
    public override int Priority => 2;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        if (battle.IsBoss)
        {
            yield return battle.Say("Tidak bisa kabur dari boss!", 0.9f);
            yield break;
        }
        yield return battle.Say($"{user.Data.DisplayName} mencoba kabur...", 0.7f);
        if (Random.value < 0.7f)
        {
            battle.SetFled();
            yield return battle.Say("Berhasil kabur!", 0.8f);
        }
        else yield return battle.Say("Gagal kabur!", 0.8f);
    }
}