public static class ElementChart
{
    public static float Multiplier(Element attacker, Element defender)
    {
        // Keuntungan (x1.5)
        if (attacker == Element.Pyro && defender == Element.Sprout) return 1.5f;
        if (attacker == Element.Aqua && defender == Element.Pyro) return 1.5f;
        if (attacker == Element.Sprout && defender == Element.Aqua) return 1.5f;
        if (attacker == Element.Volt && defender == Element.Aqua) return 1.5f;

        // Kerugian (x0.75) - Kebalikan dari atas
        if (attacker == Element.Sprout && defender == Element.Pyro) return 0.75f;
        if (attacker == Element.Pyro && defender == Element.Aqua) return 0.75f;
        if (attacker == Element.Aqua && defender == Element.Sprout) return 0.75f;
        if (attacker == Element.Aqua && defender == Element.Volt) return 0.75f;

        return 1f; // Normal
    }
}