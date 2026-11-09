using UnityEngine;
using UnityEngine.UI;

public class EndInstaller : SceneInstaller
{
    [SerializeField] private Image partnerImage;
    [SerializeField] private Text partnerText;
    [SerializeField] private Text timeText;
    [SerializeField] private Button clickCatcher;
    [SerializeField] private AudioClip music;              // (tambahan) opsional

    private GameServices services;
    private MonsterInstance partner;
    private float ready;
    private bool leaving;

    public override void Install(GameServices s)
    {
        services = s;
        partner = s.Party.Partner;
        int total = Mathf.FloorToInt(s.Party.Playtime);
        partnerText.text = partner != null ? $"{partner.Data.DisplayName} · Lv {partner.Level}" : "";
        timeText.text = $"Waktu bermain {total / 60} menit {total % 60} detik";
        clickCatcher.onClick.AddListener(Leave);
        ready = Time.unscaledTime + 0.5f;                  // jeda 0,5 detik agar tidak terlewat
        if (music != null && s.Audio != null) s.Audio.PlayWorldMusic(music);
    }

    void Update()
    {
        if (services == null) return;
        if (partner != null)
            partnerImage.sprite = partner.Data.Frames[Mathf.FloorToInt(Time.unscaledTime / 0.35f) % 2];
        if (MenuInput.Confirm) Leave();
    }

    private void Leave()
    {
        if (leaving || Time.unscaledTime < ready) return;
        leaving = true;
        services.Scenes.GoTo(SceneFlow.TitleScene);
    }
}