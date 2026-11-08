using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterPanel : MonoBehaviour
{
    [Header("Dunia")]
    [SerializeField] private SpriteRenderer body;      // EnemyMon / PlayerMon
    [SerializeField] private bool faceRight;           // pemain: true (menerjang ke kanan, gambar dibalik)
    [SerializeField] private float targetPixels = 48f; // tinggi tampil di layar demo

    [Header("UI")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text elementText;         // boleh kosong
    [SerializeField] private Text hpText;              // boleh kosong
    [SerializeField] private Image hpFill;
    [SerializeField] private Image expFill;           // boleh kosong

    public bool Boss;                                  // tampilkan ★ di depan nama

    private MonsterInstance shown;
    private MonsterData shownData;
    private Vector3 footPos;           // posisi kaki dari Editor
    private Vector3 home;              // posisi body setelah koreksi pivot
    private float hpShow;              // HP yang tampak (mengejar HP asli pelan-pelan)
    private float lungeStart = -10f;
    private float faintDrop;
    private bool hurting, evolving;

    void Awake()
    {
        footPos = body.transform.localPosition;
        home = footPos;
        body.flipX = faceRight;
    }

    // snap = true: bar HP langsung loncat. displayAs: tampilkan wujud/nama ini (untuk evolusi)
    public void Show(MonsterInstance m, bool snap = true, MonsterData displayAs = null)
    {
        shown = m;
        shownData = displayAs != null ? displayAs : m.Data;
        if (snap) hpShow = m.CurrentHP;

        nameText.text = (Boss ? "★ " : "") + shownData.DisplayName;
        levelText.text = "Lv" + m.Level;
        if (elementText != null) elementText.text = shownData.Element.ToString();
        Fit(shownData);
    }

    // Ukuran seragam ±48 px: sprite 16/20/24 px -> skala 3 / 2,4 / 2. Posisi dikoreksi agar KAKI di footPos.
    private void Fit(MonsterData data)
    {
        Sprite f = data.Frames[0];
        float s = targetPixels / Mathf.Max(f.rect.width, f.rect.height);
        body.transform.localScale = new Vector3(s, s, 1f);
        home = footPos + Vector3.up * (-f.bounds.min.y * s);        // (tambahan) tahan segala pivot
    }

    void Update()
    {
        if (shown == null) return;

        if (!hurting && !evolving)
            body.sprite = shownData.Frames[Mathf.FloorToInt(Time.time / 0.35f) % 2];   // idle 2 frame

        float k = 1f - Mathf.Pow(0.85f, Time.deltaTime * 60f);      // mendekat 15% per frame
        hpShow = Mathf.Lerp(hpShow, shown.CurrentHP, k);
        hpFill.fillAmount = shown.CurrentHP <= 0 ? 0f : hpShow / shown.MaxHP;
        hpFill.color = HpColors.For(hpFill.fillAmount);
        if (hpText != null) hpText.text = $"{Mathf.RoundToInt(hpShow)}/{shown.MaxHP}";
        if (expFill != null) expFill.fillAmount = (float)shown.Exp / shown.ExpToNext;

        float p = (Time.time - lungeStart) / 0.26f;                 // menerjang 14 px (0,875 unit)
        float dx = p < 1f ? Mathf.Sin(p * Mathf.PI) * 0.875f * (faceRight ? 1 : -1) : 0f;
        body.transform.localPosition = home + new Vector3(dx, -faintDrop, 0f);
    }

    public void PlayLunge() => lungeStart = Time.time;
    public void PlayHurt() => StartCoroutine(Hurt());
    public void PlayFaint() => StartCoroutine(Faint());
    public void PlayEvolve(MonsterData from, MonsterData to, float seconds) => StartCoroutine(Evolve(from, to, seconds));

    private Sprite HurtFrame(MonsterData d) => d.Frames[Mathf.Min(3, d.Frames.Length - 1)];   // frame 3 = putih

    private IEnumerator Hurt()
    {
        hurting = true;
        for (float t = 0f; t < 0.4f; t += Time.deltaTime)
        {
            body.sprite = Mathf.FloorToInt(t / 0.08f) % 2 == 0 ? HurtFrame(shownData) : shownData.Frames[0];
            yield return null;
        }
        hurting = false;
    }

    private IEnumerator Faint()
    {
        hurting = true;
        body.sprite = HurtFrame(shownData);
        for (float t = 0f; t < 0.5f; t += Time.deltaTime)
        {
            float p = t / 0.5f;
            faintDrop = 1.25f * p;                                  // turun 20 px
            Color c = body.color; c.a = 1f - p; body.color = c;     // memudar
            yield return null;
        }
        Color end = body.color; end.a = 0f; body.color = end;
    }

    private IEnumerator Evolve(MonsterData from, MonsterData to, float seconds)
    {
        evolving = true;
        bool showNew = false;
        int swaps = 0;
        for (float t = 0f; t < seconds;)
        {
            MonsterData d = showNew ? to : from;
            Fit(d);
            body.sprite = (swaps % 4 == 3) ? HurtFrame(d) : d.Frames[0];   // sesekali putih
            float interval = Mathf.Lerp(0.30f, 0.05f, t / seconds);       // makin cepat
            yield return new WaitForSeconds(interval);
            t += interval; showNew = !showNew; swaps++;
        }
        evolving = false;       // nama/sprite akhir diatur oleh Show(m) saat event Evolved
    }
}