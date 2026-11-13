using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlow
{
    private readonly MonoBehaviour host;
    private readonly ScreenFader fader;
    private readonly GameServices s;

    public SceneFlow(MonoBehaviour host, ScreenFader fader, GameServices s)
    {
        this.host = host;
        this.fader = fader;
        this.s = s;
    }

    public void GoTo(string scene) => host.StartCoroutine(GoToRoutine(scene, false));
    public void GoToBattle(string scene) => host.StartCoroutine(GoToRoutine(scene, true));
    public void ReturnFromBattle(string b) => host.StartCoroutine(ReturnRoutine(b));

    private IEnumerator GoToRoutine(string scene, bool additive)
    {
        s.Input.Lock();
        SetPlayerEnabled(false);
        if (fader) yield return fader.FadeOut();

        if (additive)
            yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        else
            yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);

        foreach (var root in SceneManager.GetSceneByName(scene).GetRootGameObjects())
            foreach (var inst in root.GetComponentsInChildren<SceneInstaller>(true))
                inst.Install(s);

        if (!additive) { SetPlayerEnabled(true); s.Input.Unlock(); }
        if (fader) yield return fader.FadeIn();
    }

    private IEnumerator ReturnRoutine(string battleScene)
    {
        if (fader) yield return fader.FadeOut();
        yield return SceneManager.UnloadSceneAsync(battleScene);
        SetPlayerEnabled(true);
        if (fader) yield return fader.FadeIn();
        s.Input.Unlock();
    }

    private void SetPlayerEnabled(bool on)
    {
        var pm = Object.FindAnyObjectByType<PlayerMovement>();
        if (pm) pm.enabled = on;

        var pi = Object.FindAnyObjectByType<PlayerInteractor>();
        if (pi) pi.enabled = on;
    }
}