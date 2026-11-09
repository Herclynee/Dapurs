using UnityEngine;

public static class HpColors
{
    public static readonly Color High = new Color(0.47f, 0.82f, 0.55f);   // BARU: public
    private static readonly Color Red = new Color(0.90f, 0.25f, 0.25f);

    public static Color For(float ratio) => ratio > 0.5f ? High : ratio > 0.2f ? UiTheme.Gold : Red;
}