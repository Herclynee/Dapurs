using System;
using System.IO;
using UnityEngine;

public class SaveService
{
    public const int CurrentVersion = 1;
    public SaveData PendingLoad;             // diisi tombol Lanjutkan, dipakai WorldInstaller

    private readonly int slot;
    private float playtimeBase;
    private float sessionStart = Time.realtimeSinceStartup;

    public SaveService(int slot = 0) { this.slot = slot; }

    private string FilePath => Path.Combine(Application.persistentDataPath, $"save_{slot}.json");

    public float Playtime => playtimeBase + (Time.realtimeSinceStartup - sessionStart);
    public void SetPlaytime(float seconds) { playtimeBase = seconds; sessionStart = Time.realtimeSinceStartup; }

    public bool HasSave() => File.Exists(FilePath);

    public bool Save(SaveData data)
    {
        try
        {
            data.savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            File.WriteAllText(FilePath, JsonUtility.ToJson(data, true));
            return true;
        }
        catch (Exception e) { Debug.LogError($"Gagal menyimpan: {e.Message}"); return false; }
    }

    public SaveData Load()
    {
        if (!HasSave()) return null;
        try
        {
            SaveData d = JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath));
            return d != null && d.version == CurrentVersion ? d : null;   // versi beda: abaikan
        }
        catch (Exception e) { Debug.LogError($"Gagal memuat: {e.Message}"); return null; }
    }
}