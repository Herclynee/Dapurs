using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlow
{
    public const string BattleScene = "Scn_Battle";

    private readonly MonoBehaviour runner;      // pemilik coroutine (GameBootstrap, tidak ikut di-unload)
    private readonly ScreenFader fader;
    private readonly GameServices services;
    private Scene worldScene;

    public SceneFlow(MonoBehaviour runner, ScreenFader fader, GameServices services)
    {
        this.runner = runner;
        this.fader = fader;
        this.services = services;
    }

    public void GoTo(string sceneName) => runner.StartCoroutine(GoToRoutine(sceneName));
    public void EnterBattle() => runner.StartCoroutine(EnterBattleRoutine());
    public void ExitBattle(BattleResult result) => runner.StartCoroutine(ExitBattleRoutine(result));

    private IEnumerator GoToRoutine(string sceneName)
    {
        yield return fader.Fade(1f);
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        worldScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(worldScene);
        InstallScene(worldScene);
        yield return fader.Fade(0f);
    }

    private IEnumerator EnterBattleRoutine()
    {
        services.Input.Lock();                       // Player berhenti
        yield return fader.Fade(1f);                 // layar hitam (Player sempat berhenti total)
        SetWorldActive(false);                       // bekukan dunia, termasuk kamera dunia
        yield return SceneManager.LoadSceneAsync(BattleScene, LoadSceneMode.Additive);
        Scene battle = SceneManager.GetSceneByName(BattleScene);
        SceneManager.SetActiveScene(battle);
        InstallScene(battle);
        yield return fader.Fade(0f);
    }

    private IEnumerator ExitBattleRoutine(BattleResult result)
    {
        yield return fader.Fade(1f);
        yield return SceneManager.UnloadSceneAsync(BattleScene);
        SetWorldActive(true);
        SceneManager.SetActiveScene(worldScene);
        services.Input.Unlock();
        services.Battle.Finish(result);              // aman 2,2 detik dimulai di sini
        yield return fader.Fade(0f);
    }

    private void SetWorldActive(bool active)
    {
        foreach (GameObject root in worldScene.GetRootGameObjects())
            root.SetActive(active);
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