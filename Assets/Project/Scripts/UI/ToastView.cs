using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ToastView : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private Text label;
    [SerializeField] private CanvasGroup group;
    private Coroutine running;

    void Awake() => group.alpha = 0f;

    public void Show(string message, float seconds = 2.5f)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(Routine(message, seconds));
    }

    private IEnumerator Routine(string message, float seconds)
    {
        label.text = message;
        panel.sizeDelta = new Vector2(Mathf.Min(1120f, message.Length * 20f + 64f), panel.sizeDelta.y);
        for (float t = 0f; t < 0.15f; t += Time.unscaledDeltaTime) { group.alpha = t / 0.15f; yield return null; }
        group.alpha = 1f;
        yield return new WaitForSecondsRealtime(seconds);
        for (float t = 0f; t < 0.4f; t += Time.unscaledDeltaTime) { group.alpha = 1f - t / 0.4f; yield return null; }
        group.alpha = 0f;
    }
}