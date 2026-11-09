using System.Collections.Generic;
using UnityEngine;

public class PixelBurst : MonoBehaviour
{
    [SerializeField] private Sprite pixel;          // UI_Pixel 4×4 putih
    [SerializeField] private float gravity = 2.5f;  // unit/detik², menarik partikel ke bawah
    private const float PPU = 16f;                  // 16 piksel demo = 1 unit dunia

    // [Pelengkap] satu partikel. Harus class (bukan struct), kalau tidak
    // perubahan p.life dan p.velocity di Update hanya mengubah salinan.
    private class Particle
    {
        public SpriteRenderer sr;
        public Vector2 velocity;
        public float life;   // sisa umur
        public float max;    // umur awal (untuk menghitung alpha)
    }

    private readonly List<Particle> active = new List<Particle>();  // sedang tampil
    private readonly Stack<Particle> free = new Stack<Particle>();  // sedang istirahat

    // [Pelengkap, opsional] kalau slot Pixel lupa diisi, buat sprite putih 1×1 sendiri
    void Awake()
    {
        if (pixel != null) return;
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        pixel = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    public void Play(Vector3 pos, Color color, int count, float speed, float life)
    {
        for (int i = 0; i < count; i++)
        {
            Particle p = free.Count > 0 ? free.Pop() : Create();  // pakai ulang; kosong baru buat
            float a = Random.Range(0f, Mathf.PI * 2f);             // arah acak
            float s = Random.Range(speed * 0.3f, speed) / PPU;     // kecepatan acak, piksel → unit
            p.velocity = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * s;
            p.life = p.max = life;
            p.sr.color = color;
            p.sr.transform.position = pos;
            p.sr.gameObject.SetActive(true);
            active.Add(p);
        }
    }

    // [Pelengkap] hanya dipanggil bila Stack free kosong
    private Particle Create()
    {
        var go = new GameObject("Px");
        go.transform.SetParent(transform, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = pixel;
        sr.sortingLayerName = "Characters";
        sr.sortingOrder = 100;
        float px = Random.value < 0.5f ? 1f : 2f;                       // ukuran 1 atau 2 piksel demo
        go.transform.localScale = Vector3.one * (px / PPU / pixel.bounds.size.x);
        return new Particle { sr = sr };
    }

    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = active.Count - 1; i >= 0; i--)   // mundur, supaya RemoveAt aman
        {
            Particle p = active[i];
            p.life -= dt;
            if (p.life <= 0f)
            {
                p.sr.gameObject.SetActive(false);
                active.RemoveAt(i);
                free.Push(p);                          // dipakai lagi nanti
                continue;                              // lewati sisa loop untuk partikel ini
            }
            p.velocity.y -= gravity * dt;                              // gravitasi
            p.sr.transform.position += (Vector3)(p.velocity * dt);     // bergerak
            Color c = p.sr.color;
            c.a = p.life / p.max;                                      // memudar 1 → 0
            p.sr.color = c;
        }
    }
}