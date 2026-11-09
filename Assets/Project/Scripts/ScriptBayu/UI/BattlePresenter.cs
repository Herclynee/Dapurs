using System.Collections.Generic;
using UnityEngine;

public class BattlePresenter
{
    private BattleSystem battle;
    private BattleView view;

    public BattlePresenter(BattleSystem battleSystem, BattleView battleView)
    {
        this.battle = battleSystem;
        this.view = battleView;

        // Subscribe ke semua event dari BattleSystem
        battle.Started += OnStarted;
        battle.MessageShown += view.ShowMessage;
        battle.ChoiceRequested += OnChoiceRequested;
        battle.Attacked += user => PanelOf(user).PlayLunge(user == battle.Player);
        battle.Hit += (target, dmg, crit, elem) => {
            PanelOf(target).PlayHurt();
        };
        battle.Fainted += target => PanelOf(target).PlayFaint();
        battle.LevelUp += shown => {
        // Tampilkan data lama (From) sampai evolusi selesai
         view.playerPanel.Show(battle.Player, true, shown);  // shown adalah MonsterInstance
        };
        battle.Evolving += (fromInstance, toInstance) => {
          // PERBAIKI: Ambil .Data dari MonsterInstance
          view.playerPanel.PlayEvolve(fromInstance.Data, toInstance.Data, 2.4f);
          view.PlayEvolveRings(2.4f);
        };
        battle.Evolved += m => {
            view.PlayFlash();
            view.playerPanel.Show(m, true);
        };
        battle.Ended += result => {
            // Nanti: handle kembali ke dunia
        };
    }

    private void OnStarted()
    {
        view.Initialize(battle.Player, battle.Enemy);
        view.PlayIntro();
    }

    private void OnChoiceRequested()
    {
        var labels = new List<string> { "Serang", "Jurus", "Ramuan", "Kabur" };
        var usable = new List<bool> { 
            true, 
            true, 
            battle.Player.CurrentHP < battle.Player.MaxHP, 
            !battle.IsBoss 
        };
        
        string prompt = $"Apa yang akan {battle.Player.Data.DisplayName} lakukan?";
        string hint = $"Jurus: {battle.Player.Data.SkillName}";

        // PERBAIKAN: Kirim parameter 'hint'
        view.ShowMenu(labels, usable, prompt, hint);
    }

    private MonsterPanel PanelOf(MonsterInstance m)
    {
        return (m == battle.Player) ? view.playerPanel : view.enemyPanel;
    }
}