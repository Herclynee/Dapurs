// SpriteVfx.cs: satu tebasan, memutar 4 frame lalu kembali ke pool
using System;
using UnityEngine;

public class SpriteVfx : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite[] frames;           // 4 frame tebasan
    [SerializeField] private float frameTime = 0.05f;
    private Action<SpriteVfx> release;
    private float timer; private int index;

    public void Init(Action<SpriteVfx> r) => release = r;
    public void Play(Vector3 pos)
    { transform.position = pos; index = 0; timer = 0f; sr.sprite = frames[0]; }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < frameTime) return;
        timer = 0f; index++;
        if (index >= frames.Length) { release(this); return; }   // selesai → kembali ke pool
        sr.sprite = frames[index];
    }
}