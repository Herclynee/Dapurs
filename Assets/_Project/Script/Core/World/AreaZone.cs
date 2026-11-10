using UnityEngine;

// Memaksa GameObject ini memiliki Collider2D (biasanya BoxCollider2D)
[RequireComponent(typeof(Collider2D))]
public class AreaZone : MonoBehaviour
{
    // Nama area yang akan muncul di Console/HUD
    [SerializeField] private string areaName = "Desa Awal";
    
    // Properti read-only agar script lain bisa membaca nama area ini
    public string AreaName => areaName;

    // Method ini dipanggil otomatis saat ada objek masuk ke area trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Cek: Apakah yang masuk adalah Player?
        // Jika BUKAN Player, hentikan fungsi (return).
        if (!other.CompareTag("Player")) return;

        // 2. Jika YA (Player), cetak pesan ke Console
        Debug.Log($"Masuk area: {areaName}");
        
        // Nanti di P5/P6, di sini kita akan tambahkan logika ganti musik atau update HUD
    }
}