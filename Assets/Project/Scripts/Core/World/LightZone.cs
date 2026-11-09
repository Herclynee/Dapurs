using UnityEngine;
using UnityEngine.Rendering.Universal;   // [Pelengkap] namespace Light2D
                                         // (Unity lama: UnityEngine.Experimental.Rendering.Universal)

[RequireComponent(typeof(Collider2D))]
public class LightZone : MonoBehaviour
{
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Light2D playerLight;
    [SerializeField] private Light2D[] extraLights;      // 5 kristal + BossLight
    [SerializeField] private float insideIntensity = 0.15f;  // terang global di dalam gua
    [SerializeField] private float fadeSpeed = 2f;           // perubahan intensitas per detik
    private float target = 1f;                               // tujuan terang global

    void OnTriggerEnter2D(Collider2D o)
    { if (o.CompareTag("Player")) target = insideIntensity; }   // masuk → gelap
    void OnTriggerExit2D(Collider2D o)
    { if (o.CompareTag("Player")) target = 1f; }                // keluar → terang

    void Update()
    {
        // geser pelan-pelan ke target (kecepatan tetap, tidak melewati target)
        globalLight.intensity = Mathf.MoveTowards(
            globalLight.intensity, target, fadeSpeed * Time.deltaTime);

        float dark = 1f - globalLight.intensity;   // terang → 0, gelap → 0.85
        playerLight.intensity = dark;              // lampu kecil menyala saat dunia gelap
        foreach (Light2D l in extraLights)
            if (l != null) l.intensity = dark;     // null-check: bos bisa saja sudah hilang
    }
}