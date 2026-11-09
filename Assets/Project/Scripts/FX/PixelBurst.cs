using System.Collections.Generic;
using UnityEngine;

public class PixelBurst : MonoBehaviour
{
    private class Particle                                   // class, bukan struct (isinya diubah terus)
    {
        public SpriteRenderer sr;
        public Vector2 velocity;
        public float life, max;
    }

    [SerializeField] private Sprite pixel;                   // UI_Pixel 4×4
    [SerializeField] private float gravity = 2.5f;
    [SerializeField] private string sortingLayer = "Characters";
    [SerializeField] private int sortingOrder = 100;
    [SerializeField] private Material material;              // (tambahan) opsional: material Unlit

    private readonly List<Particle> active = new List<Particle>();
    private readonly Stack<Particle> free = new Stack<Particle>();

    public void Play(Vector3 pos, Color color, int count, float speed, float life)
    {
        for (int i = 0; i < count; i++)
        {
            Particle p = free.Count > 0 ? free.Pop() : Create();     // pakai ulang bila ada
            float a = Random.Range(0f, Mathf.PI * 2f);               // arah acak
            float s = Random.Range(speed * 0.3f, speed) / 16f;       // piksel demo → unit dunia
            p.velocity = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * s;
            p.life = p.max = life;

            float px = Random.value < 0.5f ? 1f : 2f;                // ukuran 1 atau 2 piksel
            p.sr.transform.localScale = Vector3.one * (px / 16f / pixel.bounds.size.x);
            p.sr.color = color;
            p.sr.transform.position = pos;
            p.sr.gameObject.SetActive(true);
            active.Add(p);
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = active.Count - 1; i >= 0; i--)                  // MUNDUR: aman menghapus saat berulang
        {
            Particle p = active[i];
            p.life -= dt;
            if (p.life <= 0f)
            {
                p.sr.gameObject.SetActive(false);
                active.RemoveAt(i);
                free.Push(p);                                        // kembali ke pool
                continue;
            }
            p.velocity.y -= gravity * dt;                            // jatuh
            p.sr.transform.position += (Vector3)(p.velocity * dt);
            Color c = p.sr.color;
            c.a = p.life / p.max;                                    // memudar
            p.sr.color = c;
        }
    }

    private Particle Create()
    {
        var go = new GameObject("Pixel");
        go.transform.SetParent(transform, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = pixel;
        sr.sortingLayerName = sortingLayer;
        sr.sortingOrder = sortingOrder;
        if (material != null) sr.sharedMaterial = material;
        go.SetActive(false);
        return new Particle { sr = sr };
    }
}