using UnityEngine;

public static class HpColors
{
    private static readonly Color Green = new Color(0.47f, 0.82f, 0.55f);
    private static readonly Color Red   = new Color(0.90f, 0.25f, 0.25f);

    // hijau > 50%, emas > 20%, selebihnya merah
    public static Color For(float ratio) => ratio > 0.5f ? Green : ratio > 0.2f ? UiTheme.Gold : Red;
}