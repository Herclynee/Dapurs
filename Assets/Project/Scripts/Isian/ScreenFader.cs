using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float duration = 0.4f;
    private CanvasGroup group;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
        group.blocksRaycasts = false;
    }

    // target: 1 = hitam penuh, 0 = jernih
    public IEnumerator Fade(float target)
    {
        float start = group.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        group.alpha = target;
    }
}