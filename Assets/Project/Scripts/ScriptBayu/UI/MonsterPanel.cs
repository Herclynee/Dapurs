using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private TextMeshProUGUI txtHP;
    [SerializeField] private Image hpFill;
    [SerializeField] private Image expFill;
    [SerializeField] private Image body; // Sprite monster (opsional, untuk animasi)

    private MonsterInstance shown;
    private MonsterData displayData; // Untuk evolusi: tampilkan nama lama sampai animasi selesai
    private float hpShow;
    private bool hurting;
    private bool evolving;
    private float lungeStart;
    private Vector3 home;
    private bool faceRight;

    void Awake()
    {
        if (body != null) home = body.transform.localPosition;
    }

    // Parameter displayAs digunakan saat Level Up/Evolusi agar nama lama tetap tampil
    public void Show(MonsterInstance monster, bool isPlayer, MonsterInstance displayAs = null)
    {
        shown = monster;
        displayData = displayAs?.Data ?? monster.Data;
        hpShow = monster.CurrentHP;
        
        txtName.text = displayData.DisplayName;
        txtLevel.text = "Lv " + monster.Level;
        
        if (hpFill != null)
        {
            hpFill.fillAmount = Mathf.Max(0, monster.CurrentHP / (float)monster.MaxHP);
            hpFill.color = HpColors.For(hpFill.fillAmount);
        }
        
        UpdateHPText();
        
        if (expFill != null && isPlayer)
        {
            expFill.fillAmount = monster.Exp / (float)monster.ExpToNext;
        }
    }

    void Update()
    {
        if (shown == null) return;

        // Animasi HP turun perlahan (lerp)
        if (!hurting && !evolving)
        {
            float k = 1f - Mathf.Pow(0.85f, Time.deltaTime * 60f);
            hpShow = Mathf.Lerp(hpShow, shown.CurrentHP, k);
        }

        if (hpFill != null)
        {
            float ratio = Mathf.Max(0, hpShow / shown.MaxHP);
            hpFill.fillAmount = ratio;
            hpFill.color = HpColors.For(ratio);
        }

        UpdateHPText();

        // Animasi lunge (menerjang)
        if (body != null)
        {
            float p = (Time.time - lungeStart) / 0.26f;
            float dx = p < 1f ? Mathf.Sin(p * Mathf.PI) * 0.875f * (faceRight ? 1 : -1) : 0f;
            body.transform.localPosition = home + new Vector3(dx, 0f, 0f);
        }
    }

    private void UpdateHPText()
    {
        if (txtHP != null)
        {
            txtHP.text = $"{Mathf.RoundToInt(hpShow)}/{shown.MaxHP}";
        }
    }

    // --- Animasi ---
    public void PlayLunge(bool faceRight)
    {
        this.faceRight = faceRight;
        lungeStart = Time.time;
    }

    public void PlayHurt()
    {
        StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        hurting = true;
        if (body != null) body.color = Color.white; // Kedip putih
        yield return new WaitForSeconds(0.1f);
        if (body != null) body.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        if (body != null) body.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        if (body != null) body.color = Color.white;
        hurting = false;
    }

    public void PlayFaint()
    {
        StartCoroutine(FaintRoutine());
    }

    private IEnumerator FaintRoutine()
    {
        Vector3 startPos = transform.localPosition;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Turun 20px dan memudar
            transform.localPosition = startPos + new Vector3(0, -20f * t, 0); 
            
            if (body != null)
            {
                Color c = body.color;
                c.a = 1f - t;
                body.color = c;
            }
            yield return null;
        }
    }

    public void PlayEvolve(MonsterData from, MonsterData to, float duration)
    {
        StartCoroutine(EvolveRoutine(from, to, duration));
    }

    private IEnumerator EvolveRoutine(MonsterData from, MonsterData to, float duration)
    {
        evolving = true;
        float elapsed = 0f;
        float swapInterval = 0.3f;
        float lastSwap = 0f;
        bool showNew = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            // Swap sprite makin cepat
            if (elapsed - lastSwap > swapInterval)
            {
                showNew = !showNew;
                swapInterval *= 0.7f; // Makin cepat
                lastSwap = elapsed;
                
                // Jika ada body sprite, bisa ditambahkan logika ganti sprite di sini
            }
            yield return null;
        }

        evolving = false;
    }
}