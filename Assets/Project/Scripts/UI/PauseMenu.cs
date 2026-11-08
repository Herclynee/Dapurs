using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMenu menu;

    private GameServices s;
    private bool open;
    private int openedFrame;
    private static readonly string[] Options = { "Lanjut", "Simpan sekarang", "Kembali ke judul" };

    public void Construct(GameServices services)
    {
        s = services;
        menu.Picked += OnPicked;
        root.SetActive(false);
    }

    void Update()
    {
        if (s == null) return;
        bool esc = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        if (!esc) return;

        if (open) { if (Time.frameCount != openedFrame) Close(); }
        else if (CanOpen()) Open();
    }

    private bool CanOpen() =>
        s.Scenes.CurrentSceneName == SceneFlow.WorldScene && !s.Scenes.Busy
        && !s.Dialogue.IsRunning && s.Battle.Enemy == null && !s.Input.IsLocked;

    private void Open()
    {
        open = true;
        openedFrame = Time.frameCount;
        root.SetActive(true);
        s.Input.Lock();
        menu.Open(Options);
    }

    private void Close()
    {
        open = false;
        menu.Close();
        root.SetActive(false);
        s.Input.Unlock();
    }

    private void OnPicked(int index)
    {
        if (index == 0) Close();
        else if (index == 1)
        {
            bool ok = s.Player != null && s.Save.Save(SaveMapper.Capture(s, s.Player.position));
            s.Toast.Show(ok ? "Tersimpan" : "Gagal menyimpan");
            menu.Open(Options, null, 1);                  // buka lagi, pilihan tetap di "Simpan"
        }
        else { Close(); s.Scenes.GoTo(SceneFlow.TitleScene); }
    }
}