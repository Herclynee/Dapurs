using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    public void FadeOut(System.Action onComplete)
    {
        StartCoroutine(FadeRoutine(0f, 1f, fadeDuration, onComplete));
    }

    public void FadeIn(System.Action onComplete)
    {
        StartCoroutine(FadeRoutine(1f, 0f, fadeDuration, onComplete));
    }

    private IEnumerator FadeRoutine(float from, float to, float duration, System.Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, to);
        onComplete?.Invoke();
    }
} 