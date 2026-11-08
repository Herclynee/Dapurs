using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject root;            // DialogueRoot
    [SerializeField] private Button clickCatcher;
    [SerializeField] private Image box;
    [SerializeField] private Sprite navySprite;          // UI_Panel_Navy
    [SerializeField] private Sprite narrSprite;          // UI_Panel_Narr
    [SerializeField] private Text body;
    [SerializeField] private RectTransform speakerTag;
    [SerializeField] private Text speaker;
    [SerializeField] private GameObject continueHint;    // ▼
    [SerializeField] private TextMenu choices;
    [SerializeField] private RectTransform choicesRect;

    public event Action Clicked;
    public event Action<int> ChoicePicked;
    private bool hintOn;

    void Awake()
    {
        root.SetActive(false);                                   // sembunyikan DULU
        clickCatcher.onClick.AddListener(() => Clicked?.Invoke());
        choices.Picked += i => ChoicePicked?.Invoke(i);
    }

    void Update()                                        // ▼ berkedip tiap 0,3 detik
    {
        continueHint.SetActive(hintOn && Mathf.FloorToInt(Time.unscaledTime / 0.3f) % 2 == 0);
    }

    public void Show(bool narration)
    {
        root.SetActive(true);
        box.sprite = narration ? narrSprite : navySprite;
        canvas.sortingOrder = narration ? 110 : 20;       // narasi harus di atas Fader (100)
        HideChoices();
        ShowHint(false);
    }

    public void Hide()
    {
        HideChoices();
        root.SetActive(false);
    }

    public void SetSpeaker(string name)
    {
        bool has = !string.IsNullOrEmpty(name);
        speakerTag.gameObject.SetActive(has);
        if (!has) return;
        speaker.text = name;
        speakerTag.sizeDelta = new Vector2(Mathf.Min(488f, name.Length * 20f + 64f), speakerTag.sizeDelta.y);
    }

    public void SetBody(string text) => body.text = text;
    public void ShowHint(bool on) { hintOn = on; if (!on) continueHint.SetActive(false); }

    public void ShowChoices(string[] options)
    {
        choicesRect.sizeDelta = new Vector2(choicesRect.sizeDelta.x, options.Length * 56f + 32f);
        choices.Open(options);
    }

    public void HideChoices() => choices.Close();
}