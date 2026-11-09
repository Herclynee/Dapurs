using UnityEngine;

public static class ElementChart
{
    // Penyerang kuat melawan: Pyro>Sprout, Aqua>Pyro, Sprout>Aqua, Volt>Aqua
    private static bool IsStrong(Element a, Element d) =>
        (a == Element.Pyro   && d == Element.Sprout) ||
        (a == Element.Aqua   && d == Element.Pyro)   ||
        (a == Element.Sprout && d == Element.Aqua)   ||
        (a == Element.Volt   && d == Element.Aqua);

    public static float Multiplier(Element attacker, Element defender)
    {
        if (IsStrong(attacker, defender)) return 1.5f;   // menang
        if (IsStrong(defender, attacker)) return 0.75f;  // kebalikannya
        return 1f;                                       // lainnya
    }
}