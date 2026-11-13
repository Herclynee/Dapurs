public static class ElementChart
{
    public static float Multiplier(Element attacker, Element defender)
    {
        switch (attacker)
        {
            case Element.Pyro:
                return defender == Element.Sprout ? 1.5f
                     : defender == Element.Aqua   ? 0.75f : 1f;
            case Element.Aqua:
                return defender == Element.Pyro   ? 1.5f
                     : defender == Element.Sprout ? 0.75f
                     : defender == Element.Volt   ? 0.75f : 1f;
            case Element.Sprout:
                return defender == Element.Aqua ? 1.5f
                     : defender == Element.Pyro ? 0.75f : 1f;
            case Element.Volt:
                return defender == Element.Aqua ? 1.5f : 1f;
            default: return 1f;
        }
    }
}