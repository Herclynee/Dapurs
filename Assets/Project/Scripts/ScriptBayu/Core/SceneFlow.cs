using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlow
{
    private GameBootstrap bootstrap;
    private ScreenFader fader;
    private GameServices services;

    public SceneFlow(GameBootstrap bootstrap, ScreenFader fader, GameServices services)
    {
        this.bootstrap = bootstrap;
        this.fader = fader;
        this.services = services;
    }

    public void GoTo(string sceneName)
    {
        fader.FadeOut(() =>
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(sceneName);
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe agar tidak dipanggil berkali-kali
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Cari SceneInstaller di scene yang baru saja di-load
        var installer = Object.FindAnyObjectByType<SceneInstaller>();
        
        if (installer != null)
        {
            Debug.Log($"✅ [SceneFlow] Installer ditemukan: {installer.GetType().Name}");
            installer.Install(services);
        }
        else
        {
            Debug.LogError($"❌ [SceneFlow] Installer TIDAK DITEMUKAN di {scene.name}!");
        }

        // Fade in setelah install selesai
        fader.FadeIn(null);
    }
}