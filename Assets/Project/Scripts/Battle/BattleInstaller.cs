using UnityEngine;

public class BattleInstaller : SceneInstaller
{
    [SerializeField] private BattleSystem battle;
    [SerializeField] private BattleView view;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Sprite forestBackground;
    [SerializeField] private Sprite caveBackground;
    [SerializeField] private AudioClip battleMusic;            // BGM_Battle
    [SerializeField] private AudioClip bossMusic;              // BGM_Boss
    [SerializeField] private BattleSfx sfx;
    [SerializeField] private BattleFeel feel;       // field baru

    public override void Install(GameServices s)
    {
        background.sprite = s.Battle.IsBoss ? caveBackground : forestBackground;
        battle.Construct(s.Party, s.Battle, s.Scenes);
        new BattlePresenter(battle, view);
        if (s.Audio != null)
        {
            s.Audio.PlayBattleMusic(s.Battle.IsBoss ? bossMusic : battleMusic);
            new BattleAudio(battle, s.Audio, sfx);
        }
        if (feel != null) feel.Construct(battle);
        battle.Begin();                                         // P7: + BattleFeel
    }
}