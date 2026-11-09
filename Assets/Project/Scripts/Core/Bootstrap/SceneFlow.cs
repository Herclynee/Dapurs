using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlow
{
    public const string TitleScene = "Scn_Title";
    public const string WorldScene = "Scn_World";
    public const string BattleScene = "Scn_Battle";
    public const string EndScene = "Scn_End";

    public bool HoldFade;                     
    public bool Busy { get; private set; }
    public string CurrentSceneName => current.IsValid() ? current.name : "";

    private readonly MonoBehaviour runner;
    private readonly ScreenFader fader;
    private readonly GameServices services;
    private Scene current;                    

    public SceneFlow(MonoBehaviour runner, ScreenFader fader, GameServices services)
    {
        this.runner = runner; this.fader = fader; this.services = services;
    }

    public void GoTo(string sceneName) => runner.StartCoroutine(GoToRoutine(sceneName));
    public void EnterBattle() => runner.StartCoroutine(EnterBattleRoutine());
    public void ExitBattle(BattleResult result) => runner.StartCoroutine(ExitBattleRoutine(result));
    public void EndBattle(BattleResult result) => ExitBattle(result);

    public IEnumerator ReleaseFade()
    {
        HoldFade = false;
        yield return fader.Fade(0f);
    }

    private IEnumerator GoToRoutine(string sceneName)
    {
        Busy = true;
        yield return fader.Fade(1f);
        if (current.IsValid() && current.isLoaded)
            yield return SceneManager.UnloadSceneAsync(current);          
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        current = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(current);
        InstallScene(current);                                           
        if (!HoldFade) yield return fader.Fade(0f);
        Busy = false;
    }

    private IEnumerator EnterBattleRoutine()
    {
        Busy = true;
        services.Input.Lock();
        yield return fader.Fade(1f);
        SetCurrentActive(false);
        yield return SceneManager.LoadSceneAsync(BattleScene, LoadSceneMode.Additive);
        Scene battle = SceneManager.GetSceneByName(BattleScene);
        SceneManager.SetActiveScene(battle);
        InstallScene(battle);
        yield return fader.Fade(0f);
        Busy = false;
    }

    private IEnumerator ExitBattleRoutine(BattleResult result)
    {
        Busy = true;
        yield return fader.Fade(1f);
        yield return SceneManager.UnloadSceneAsync(BattleScene);
        SetCurrentActive(true);
        SceneManager.SetActiveScene(current);
        if (services.Audio != null) services.Audio.ResumeWorldMusic();    
        services.Input.Unlock();
        services.Battle.Finish(result);
        yield return fader.Fade(0f);
        Busy = false;
    }

    private void SetCurrentActive(bool active)
    {
        foreach (GameObject root in current.GetRootGameObjects()) root.SetActive(active);
    }

    private void InstallScene(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            SceneInstaller installer = root.GetComponentInChildren<SceneInstaller>(true);
            if (installer != null) { installer.Install(services); return; }
        }
        Debug.LogWarning($"Tidak ada SceneInstaller di scene {scene.name}");
    }
}