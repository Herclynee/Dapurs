using System.Collections;

// Class abstrak: tidak bisa dibuat objeknya langsung, harus diturunkan
public abstract class BattleAction
{
    // Prioritas giliran (Kabur = 2, Ramuan = 1, Serang/Jurus = 0)
    public abstract int Priority { get; }

    // Method yang akan dijalankan saat giliran tiba
    public abstract IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target);
}