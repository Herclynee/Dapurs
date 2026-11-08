using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleInstaller : SceneInstaller
{
    [SerializeField] private TextMenu menu;
    [SerializeField] private AudioClip titleMusic;               // BGM_Title
    [SerializeField] private RectTransform background;           // Image BG_Title_World
    [SerializeField] private Image[] starterImages;              // 4 Image
    [SerializeField] private MonsterData[] starters;             // Pyrit, Blubi, Sprig, Zapi

    private GameServices services;
    private readonly List<string> options = new List<string>();

    public override void Install(GameServices s)
    {
        services = s;
        options.Clear();
        if (s.Save.HasSave()) options.Add("Lanjutkan");
        options.Add("Mulai baru");
        options.Add("Keluar");
        menu.Picked += OnPicked;
        menu.Open(options.ToArray());
        s.Audio.PlayWorldMusic(titleMusic);

        Image bg = background.GetComponent<Image>();             // (tambahan) ukuran asli x4
        if (bg != null && bg.sprite != null) background.sizeDelta = bg.sprite.rect.size * 4f;
    }

    void Update()
    {
        // Latar bergeser pelan, batas aman = setengah selisih lebar gambar dan layar
        if (background != null)
        {
            Image bg = background.GetComponent<Image>();
            float amp = 240f;
            if (bg != null && bg.sprite != null) amp = Mathf.Min(240f, Mathf.Max(0f, (bg.sprite.rect.width - 320f) / 2f));
            background.anchoredPosition = new Vector2(Mathf.Sin(Time.time / 6f) * amp * 4f, 0f);
        }

        // 4 starter beranimasi tiap 0,35 detik
        int frame = Mathf.FloorToInt(Time.time / 0.35f) % 2;
        for (int i = 0; i < starterImages.Length && i < starters.Length; i++)
            starterImages[i].sprite = starters[i].Frames[frame];
    }

    private void OnPicked(int index)
    {
        switch (options[index])
        {
            case "Lanjutkan": Continue(); break;
            case "Mulai baru": NewGame(); break;
            default: Quit(); break;
        }
    }

    private void Continue()
    {
        SaveData data = services.Save.Load();
        if (data == null) { NewGame(); return; }
        SaveMapper.Apply(data, services);
        services.Save.PendingLoad = data;                 // posisi dipakai World
        services.Scenes.GoTo(SceneFlow.WorldScene);
    }

    private void NewGame()
    {
        services.Party.Reset();                           // Ramuan 3, tanpa partner
        services.Flags.Load(new List<string>());          // narasi diputar
        services.Save.SetPlaytime(0f);
        services.Scenes.GoTo(SceneFlow.WorldScene);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}