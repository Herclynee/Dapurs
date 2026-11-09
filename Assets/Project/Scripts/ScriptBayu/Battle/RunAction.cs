using System.Collections;
using UnityEngine;

public class RunAction : BattleAction
{
    public override int Priority => 2; // Paling cepat

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        // Boss tidak bisa dikaburi
        if (battle.IsBoss)
        {
            yield return battle.Say("Tidak bisa kabur dari Boss!", 0.8f);
            yield break;
        }

        // Peluang kabur 70%
        if (Random.value < 0.7f)
        {
            yield return battle.Say("Berhasil kabur!", 0.5f);
            battle.Flee(); // Method ini akan kita buat di BattleSystem nanti
        }
        else
        {
            yield return battle.Say("Gagal kabur!", 0.8f);
        }
    }
}
