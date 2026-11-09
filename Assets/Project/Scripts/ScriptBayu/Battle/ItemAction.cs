using System.Collections;
using UnityEngine;

public class ItemAction : BattleAction
{
    public override int Priority => 1; // Lebih cepat dari serangan biasa

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        // target di sini adalah user sendiri karena kita heal diri sendiri
        int healAmount = Mathf.RoundToInt(user.MaxHP * 0.4f); // Pulih 40% HP
        user.Heal(healAmount);
        
        yield return battle.Say($"{user.Data.DisplayName} memakai ramuan dan pulih {healAmount} HP!", 0.8f);
    }
}
