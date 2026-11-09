using UnityEngine;

[CreateAssetMenu(menuName = "Pulau Monster/Monster Database")]
public class MonsterDatabase : ScriptableObject
{
    [SerializeField] private MonsterData[] allMonsters;

    public MonsterData Get(string id)
    {
        foreach (var m in allMonsters)
            if (m.Id == id) return m;
        return null;
    }
}