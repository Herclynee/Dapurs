using UnityEngine;

public static class HpColors
{
    public static Color For(float ratio)
    {
        // Hijau > 50%, Emas > 20%, Merah sisanya
        if (ratio > 0.5f) return Color.green;
        if (ratio > 0.2f) return new Color(1f, 0.78f, 0.1f); // emas
        return Color.red;
    }
}