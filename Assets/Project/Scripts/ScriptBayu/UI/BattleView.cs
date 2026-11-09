using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // WAJIB ada untuk menggunakan Image
using TMPro;

public class BattleView : MonoBehaviour
{
    [Header("Panels")]
    public MonsterPanel playerPanel;  
    public MonsterPanel enemyPanel;   

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI txtMessage;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private TextMenu menu;

    [Header("Evolve Effects")]
    [SerializeField] private Image[] rings; // Array untuk 3 cincin evolusi

    private Coroutine typing;
    private int lastPick = 0;

    public void Initialize(MonsterInstance player, MonsterInstance enemy)
    {
        playerPanel.Show(player, true);
        enemyPanel.Show(enemy, false);
    }

    public void ShowMessage(string text)
    {
        if (hintText != null) hintText.enabled = false;
        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        txtMessage.text = "";
        float secondsPerChar = 0.018f;
        
        for (int i = 0; i < text.Length; i++)
        {
            txtMessage.text += text[i];
            if (MenuInput.Confirm) 
            {
                txtMessage.text = text;
                break;
            }
            yield return new WaitForSeconds(secondsPerChar);
        }
    }

    public void ShowMenu(IReadOnlyList<string> labels, IReadOnlyList<bool> usable, string prompt, string hint = "")
    {
        if (typing != null) StopCoroutine(typing);
        txtMessage.text = prompt;
        
        if (hintText != null)
        {
            hintText.text = hint;
            hintText.enabled = !string.IsNullOrEmpty(hint);
        }

        int start = usable.Count > lastPick && usable[lastPick] ? lastPick : 0;
        menu.Open(new List<string>(labels).ToArray(), new List<bool>(usable).ToArray(), start);
    }

    public void SetLastPick(int index) => lastPick = index;

    public void PlayIntro() 
    { 
        Debug.Log("Layar menggelap (Intro Battle)"); 
        // Nanti bisa diganti dengan ScreenFader.FadeOut
    }

    public void PlayFlash() 
    { 
        Debug.Log("Layar berkedip putih"); 
        // Nanti bisa diganti dengan animasi flash putih
    }

    // ==========================================
    // TAMBAHAN BARU: Animasi Cincin Evolusi
    // ==========================================
    public void PlayEvolveRings(float duration)
    {
        StartCoroutine(EvolveRingsRoutine(duration));
    }

    private IEnumerator EvolveRingsRoutine(float duration)
    {
        // 1. Aktifkan dan reset ukuran cincin
        for (int i = 0; i < rings.Length; i++)
        {
            if (rings[i] != null)
            {
                rings[i].gameObject.SetActive(true);
                rings[i].transform.localScale = Vector3.one * 0.1f;
                
                // Reset alpha ke 1
                Color c = rings[i].color;
                c.a = 1f;
                rings[i].color = c;
            }
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            for (int i = 0; i < rings.Length; i++)
            {
                if (rings[i] != null)
                {
                    // Setiap cincin mulai dengan delay berbeda (0, 0.15, 0.3 detik)
                    float delay = i * 0.15f;
                    float ringElapsed = Mathf.Max(0f, elapsed - delay);
                    float ringDuration = duration - delay;
                    
                    if (ringDuration > 0)
                    {
                        float t = Mathf.Clamp01(ringElapsed / ringDuration);
                        
                        // Membesar dari 0.1x ke 3.0x
                        float scale = 0.1f + t * 2.9f;
                        rings[i].transform.localScale = Vector3.one * scale;
                        
                        // Fade out di separuh akhir animasi
                        float alpha = 1f - Mathf.Clamp01((t - 0.5f) * 2f);
                        Color c = rings[i].color;
                        c.a = alpha;
                        rings[i].color = c;
                    }
                }
            }

            yield return null;
        }

        // 2. Sembunyikan cincin setelah animasi selesai
        for (int i = 0; i < rings.Length; i++)
        {
            if (rings[i] != null)
            {
                rings[i].gameObject.SetActive(false);
            }
        }
    }
}