using System;
using System.Collections;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    [SerializeField] private DialogueView view;

    public bool IsRunning { get; private set; }
    private InputGate gate;
    private AudioService audioService;
    private bool clicked;
    private int picked;

    public void Construct(InputGate inputGate, AudioService audio)
    {
        gate = inputGate;
        audioService = audio;
    }

    void Awake()
    {
        view.Clicked += () => clicked = true;
        view.ChoicePicked += i => picked = i;
    }

    // ===== Menjalankan urutan =====
    public void Run(IEnumerator sequence)
    {
        if (!IsRunning && sequence != null) StartCoroutine(RunRoutine(sequence));
    }

    private IEnumerator RunRoutine(IEnumerator sequence)
    {
        IsRunning = true;
        gate.Lock();                       // pemain berhenti
        yield return null;                 // lewati tombol pembuka
        clicked = false;
        yield return sequence;             // urutan milik NPC / cutscene
        view.Hide();
        yield return null;                 // cegah dialog terbuka lagi oleh tombol penutup
        gate.Unlock();
        IsRunning = false;
    }

    // ===== Tiga perintah dasar =====
    public IEnumerator Say(string speaker, string text)
    { view.Show(false); yield return TypeLine(speaker, text, true); }

    public IEnumerator Narrate(string text)               // kotak biru
    { view.Show(true); yield return TypeLine("", text, true); }

    public IEnumerator Choose(string speaker, string prompt, string[] options, Action<int> result)
    {
        view.Show(false);
        yield return TypeLine(speaker, prompt, false);    // tanpa ▼
        picked = -1;
        view.ShowChoices(options);
        yield return new WaitUntil(() => picked >= 0);
        view.HideChoices();
        result?.Invoke(picked);
    }

    public IEnumerator PlayData(DialogueData data)        // dipakai Sari
    {
        foreach (string line in data.Lines) yield return Say(data.Speaker, line);
    }

    public void Play(DialogueData data) => Run(PlayData(data));   // versi lama tetap ada

    // ===== Mengetik =====
    private IEnumerator TypeLine(string speaker, string text, bool waitAdvance)
    {
        view.SetSpeaker(speaker);
        view.SetBody("");
        view.ShowHint(false);
        clicked = false;

        float shown = 0f;
        int blips = 0;
        while (shown < text.Length)
        {
            shown += Time.deltaTime * 55f;                 // 55 huruf per detik
            int n = Mathf.Min(text.Length, Mathf.FloorToInt(shown));
            view.SetBody(text.Substring(0, n));
            if (n / 3 > blips) { blips = n / 3; if (audioService != null) audioService.PlayBlip(); }  // blip tiap 3 huruf
            if (AdvancePressed()) break;                   // tekan = langsung tampil penuh
            yield return null;
        }
        view.SetBody(text);

        if (!waitAdvance) yield break;
        view.ShowHint(true);
        yield return null;
        while (!AdvancePressed()) yield return null;       // tekan lagi = lanjut
        view.ShowHint(false);
    }

    private bool AdvancePressed()
    {
        bool pressed = clicked || MenuInput.Confirm;       // klik / E / Spasi / Enter
        clicked = false;
        return pressed;
    }
}