using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : MonoBehaviour
{
    [SerializeField] private TextMenu menu;
    [SerializeField] private Text messageText;
    [SerializeField] private Text hintText;
    [SerializeField] private MonsterPanel enemyPanel;
    [SerializeField] private MonsterPanel playerPanel;
    [SerializeField] private Image[] rings;            // Ring_0..2
    [SerializeField] private Image flash;
    [SerializeField] private RectTransform wipeTop;
    [SerializeField] private RectTransform wipeBottom;

    public MonsterPanel EnemyPanel => enemyPanel;
    public MonsterPanel PlayerPanel => playerPanel;
    public event Action<int> ActionClicked;

    private Coroutine typing;
    private int lastPick;

    void Awake()
    {
        menu.Picked += i => { lastPick = i; ActionClicked?.Invoke(i); };
        SetAlpha(flash, 0f);
        foreach (Image r in rings) r.enabled = false;
        wipeTop.gameObject.SetActive(true);                // tirai menutup layar sejak awal
        wipeBottom.gameObject.SetActive(true);
        messageText.text = "";
        hintText.text = "";
    }

    void Start() => menu.Close();

    public void ShowMessage(string text)
    {
        menu.Close();                                      // saat pesan tampil, menu tersembunyi
        hintText.enabled = false;
        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(Type(text));
    }

    private IEnumerator Type(string text)
    {
        messageText.text = "";
        float shown = 0f;
        while (shown < text.Length)
        {
            shown += Time.deltaTime * 55f;                 // 55 huruf per detik
            messageText.text = text.Substring(0, Mathf.Min(text.Length, Mathf.FloorToInt(shown)));
            yield return null;
        }
        messageText.text = text;
    }

    public void ShowMenu(string[] labels, bool[] usable, string prompt, string hint)
    {
        if (typing != null) StopCoroutine(typing);
        messageText.text = prompt;                         // "Apa yang akan Pyrit lakukan?"
        hintText.text = hint;                              // "Jurus elemen: Semburan Api"
        hintText.enabled = true;
        int start = usable[lastPick] ? lastPick : 0;       // ingat pilihan terakhir
        menu.Open(labels, usable, start);
    }

    // ===== Efek layar =====
    public void PlayIntro() => StartCoroutine(Intro());
    public void PlayFlash() => StartCoroutine(Flash());
    public void PlayEvolveRings(float seconds) => StartCoroutine(Rings(seconds));

    private IEnumerator Intro()                            // dua tirai membuka 0,48 s
    {
        for (float t = 0f; t < 0.48f; t += Time.deltaTime)
        {
            float d = Mathf.SmoothStep(0f, 360f, t / 0.48f);
            wipeTop.anchoredPosition = new Vector2(0f, d);
            wipeBottom.anchoredPosition = new Vector2(0f, -d);
            yield return null;
        }
        wipeTop.gameObject.SetActive(false);
        wipeBottom.gameObject.SetActive(false);
    }

    private IEnumerator Flash()                            // putih memudar 0,5 s
    {
        for (float t = 0f; t < 0.5f; t += Time.deltaTime) { SetAlpha(flash, 1f - t / 0.5f); yield return null; }
        SetAlpha(flash, 0f);
    }

    private IEnumerator Rings(float seconds)               // 3 cincin melebar 125 px/s
    {
        for (float t = 0f; t < seconds; t += Time.deltaTime)
        {
            for (int i = 0; i < rings.Length; i++)
            {
                float local = t - i * 0.5f;                // tiap cincin menyusul 0,5 s
                if (local < 0f) continue;
                float cycle = local % 1.2f;
                rings[i].enabled = true;
                float diameter = cycle * 125f * 2f * 4f + 16f;       // piksel demo -> unit kanvas
                rings[i].rectTransform.sizeDelta = Vector2.one * diameter;
                Color c = UiTheme.Gold; c.a = 1f - cycle / 1.2f; rings[i].color = c;
            }
            yield return null;
        }
        foreach (Image r in rings) r.enabled = false;
    }

    private static void SetAlpha(Image img, float a) { Color c = img.color; c.a = a; img.color = c; }
}