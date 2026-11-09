using UnityEngine;

public class MonsterInstance : IDamageable
{
    // Data statis (bisa berubah jika evolusi)
    public MonsterData Data { get; private set; }
    
    // Data hidup
    public int Level { get; private set; }
    public int CurrentHP { get; private set; }
    public int Exp { get; private set; }

    // Event untuk memberi tahu UI jika ada perubahan (akan dipakai di P5)
    public event System.Action Changed;

    // Rumus Skala: Level 1 = 1.0x, Level 2 = 1.1x, dst.
    private float Scale => 1f + (Level - 1) * 0.1f;

    // Property Stat Dinamis
    public int MaxHP => Mathf.RoundToInt(Data.BaseHP * Scale) + Level * 2;
    public int Attack => Mathf.RoundToInt(Data.BaseATK * Scale);
    public int ExpToNext => Level * 10;

    // Constructor
    public MonsterInstance(MonsterData data, int startLevel)
    {
        Data = data;
        Level = startLevel;
        CurrentHP = MaxHP; // HP penuh saat dibuat
    }

    // Method dari IDamageable
    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Changed?.Invoke();
    }

    // Logika EXP dan Evolusi
    public void GainExp(int amount)
    {
        Exp += amount;
        
        // Naik Level (Sederhana: langsung naik jika EXP cukup)
        while (Exp >= ExpToNext)
        {
            Exp -= ExpToNext;
            Level++;
            CurrentHP = MaxHP; // HP penuh saat naik level
            Debug.Log($"{Data.DisplayName} naik ke Level {Level}!");
            
            // Cek Evolusi
            CheckEvolution();
        }
        Changed?.Invoke();
    }

    private void CheckEvolution()
    {
        // Jika sudah mencapai level evolusi DAN ada bentuk selanjutnya
        if (Data.EvolveLevel > 0 && Level >= Data.EvolveLevel && Data.EvolvesTo != null)
        {
            MonsterData nextForm = Data.EvolvesTo;
            Debug.Log($"{Data.DisplayName} berevolusi menjadi {nextForm.DisplayName}!");
            
            Data = nextForm; // Ganti data statis ke bentuk baru
            CurrentHP = MaxHP; // Sesuaikan HP dengan bentuk baru
        }
    }
}