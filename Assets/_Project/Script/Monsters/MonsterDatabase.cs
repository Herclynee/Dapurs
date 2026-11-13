using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDatabase",
                 menuName = "Pulau Monster/Monster Database")]
public class MonsterDatabase : ScriptableObject
{
    [SerializeField] private List<MonsterData> monsters = new List<MonsterData>();

    public IReadOnlyList<MonsterData> Monsters => monsters;

    // Cari monster berdasarkan id, contoh: "MON_001"
    public MonsterData Get(string id)
    {
        foreach (MonsterData m in monsters)
            if (m != null && m.Id == id) return m;
        return null;
    }
}