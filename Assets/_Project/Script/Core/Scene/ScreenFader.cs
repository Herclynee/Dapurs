using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    private CanvasGroup cg;
    void Awake() { cg = GetComponent<CanvasGroup>(); cg.alpha = 0; cg.blocksRaycasts = false; }

    public IEnumerator Fade(float to, float dur)
    {
        cg.blocksRaycasts = to > 0;
        float from = cg.alpha, t = 0;
        while (t < dur) { t += Time.unscaledDeltaTime; cg.alpha = Mathf.Lerp(from, to, t / dur); yield return null; }
        cg.alpha = to; cg.blocksRaycasts = to > 0;
    }
    public IEnumerator FadeOut(float d = 0.35f) => Fade(1, d);
    public IEnumerator FadeIn(float d = 0.35f)  => Fade(0, d);
}