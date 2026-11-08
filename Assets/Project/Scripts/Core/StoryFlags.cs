using System;
using System.Collections.Generic;

public class StoryFlags
{
    public const string IntroSeen = "intro_seen";
    public const string HasStarter = "has_starter";
    public const string Gift = "gift";
    public const string BossDefeated = "boss_defeated";

    private readonly HashSet<string> set = new HashSet<string>();
    public event Action<string> Changed;

    public bool Has(string key) => set.Contains(key);
    public void Set(string key) { if (set.Add(key)) Changed?.Invoke(key); }
    public void Load(List<string> keys) { set.Clear(); foreach (string k in keys) set.Add(k); }
    public List<string> ToList() => new List<string>(set);
}