public class BattlePresenter
{
    private readonly BattleSystem battle;
    private readonly BattleView view;

    public BattlePresenter(BattleSystem battleSystem, BattleView battleView)
    {
        battle = battleSystem;
        view = battleView;

        battle.Started          += OnStarted;
        battle.MessageShown     += view.ShowMessage;
        battle.ChoiceRequested  += OnChoiceRequested;
        battle.Attacked         += m => PanelOf(m).PlayLunge();
        battle.Hit              += OnHit;
        battle.Fainted          += m => PanelOf(m).PlayFaint();
        battle.LevelUp          += shown => view.PlayerPanel.Show(battle.Player, false, shown);
        battle.Evolving         += (from, to) => { view.PlayerPanel.PlayEvolve(from, to, 2.4f); view.PlayEvolveRings(2.4f); };
        battle.Evolved          += m => { view.PlayFlash(); view.PlayerPanel.Show(m); };
        view.ActionClicked      += OnActionClicked;
    }

    private MonsterPanel PanelOf(MonsterInstance m) => m == battle.Player ? view.PlayerPanel : view.EnemyPanel;

    private void OnStarted()
    {
        view.EnemyPanel.Boss = battle.IsBoss;
        view.EnemyPanel.Show(battle.Enemy);
        view.PlayerPanel.Show(battle.Player);
        view.PlayIntro();
    }

    private void OnChoiceRequested()
    {
        MonsterInstance p = battle.Player;
        string[] labels = { "Serang", $"Jurus ({battle.SkillsLeft})", $"Ramuan ({battle.Party.Potions})", "Kabur" };
        bool[] usable = { true, battle.SkillsLeft > 0, battle.Party.Potions > 0, true };
        view.ShowMenu(labels, usable,
            $"Apa yang akan {p.Data.DisplayName} lakukan?",
            $"Jurus elemen: {p.Data.SkillName}");
    }

    private void OnActionClicked(int index) => battle.Choose(battle.Actions[index]);

    private void OnHit(MonsterInstance target, int damage, bool critical, Element? element)
    {
        MonsterPanel panel = PanelOf(target);
        panel.Show(target, false);                         // false: bar HP turun pelan
        panel.PlayHurt();
    }
}