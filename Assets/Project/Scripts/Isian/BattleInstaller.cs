
using System.Collections;
using UnityEngine;

public class BattleInstaller : SceneInstaller
{
    [SerializeField] private SpriteRenderer enemyView;
    [SerializeField] private float autoWinSeconds = 2f;

    public override void Install(GameServices s)
    {
        if (s == null || s.Battle == null)
        {
            Debug.LogError("BattleInstaller: GameServices atau Battle belum tersedia!");
            return;
        }

        MonsterInstance enemy = s.Battle.Enemy;

        if (enemy != null && enemy.Data != null)
        {
            if (enemy.Data.Frames != null &&
                enemy.Data.Frames.Length > 0 &&
                enemy.Data.Frames[0] != null)
            {
                if (enemyView != null)
                {
                    enemyView.sprite = enemy.Data.Frames[0];
                    enemyView.enabled = true;
                }
            }
            else
            {
                Debug.LogWarning("BattleInstaller: Frames monster kosong atau belum memiliki sprite!");
                
                if (enemyView != null)
                    enemyView.enabled = false;
            }

            Debug.Log($"Battle: {enemy.Data.DisplayName} Lv {enemy.Level}");
        }
        else
        {
            Debug.LogWarning("BattleInstaller: Data enemy tidak ditemukan!");

            if (enemyView != null)
                enemyView.enabled = false;
        }

        StartCoroutine(AutoWin(s));
    }

    private IEnumerator AutoWin(GameServices s)
    {
        yield return new WaitForSeconds(autoWinSeconds);

        if (s != null && s.Scenes != null)
        {
            s.Scenes.ExitBattle(BattleResult.Win);
        }
    }
}
