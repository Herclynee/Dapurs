using UnityEngine;

public class BattleInstaller : SceneInstaller
{
    [SerializeField] private SpriteRenderer enemySprite;
    private GameServices s;

    public override void Install(GameServices services)
    {
        s = services;
        if (s.Battle.Enemy != null)
        {
            var m = s.Battle.Enemy;
            if (enemySprite != null) enemySprite.sprite = m.Data.Frames[0];
            
            // P4 Placeholder: Menang otomatis setelah 2 detik
            Invoke(nameof(AutoWin), 2f);
        }
    }

    private void AutoWin()
    {
        s.Battle.Finish(BattleResult.Win);
        s.Scenes.ReturnFromBattle("Scn_Battle");
    }
}