using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassRustle : MonoBehaviour
{
    [SerializeField] private Tilemap tallGrass;          // Tilemap TallGrass (P2)
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PixelBurst burst;
    [SerializeField] private float interval = 0.15f;
    [SerializeField] private Color leaf = new Color32(124, 192, 97, 255);  // hijau daun
    private float timer;

    void Update()
    {
        if (movement.Velocity.sqrMagnitude < 0.01f) return;      // diam → tidak ada daun
        Vector3Int cell = tallGrass.WorldToCell(
            transform.position + Vector3.up * 0.1f);             // sampel di kaki, digeser sedikit ke atas
        if (!tallGrass.HasTile(cell)) return;                    // bukan rumput tinggi
        timer -= Time.deltaTime;
        if (timer > 0f) return;                                  // pola cooldown
        timer = interval;
        burst.Play(transform.position + Vector3.up * 0.2f,
                   leaf, 3, 25f, 0.35f);                         // 3 daun, hidup 0,35 s
    }
}