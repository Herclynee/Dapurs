using UnityEngine;

public static class UiTheme
{
    // Warna tema navy-emas
    public static readonly Color Ink       = Hex("#121027");
    public static readonly Color Panel     = Hex("#1C1540");
    public static readonly Color Gold      = Hex("#FFC61A");
    public static readonly Color Text      = Hex("#EDE9F7");
    public static readonly Color Muted     = Hex("#A9A3C9");
    public static readonly Color Disabled  = Hex("#5C5680");
    public static readonly Color Exp       = Hex("#6F9BFF");
    public static readonly Color BarBg     = Hex("#0B0918");

    // Helper: konversi hex ke Color
    private static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }
}