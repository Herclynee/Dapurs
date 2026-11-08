using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;   // untuk tahu arah hadap
    [SerializeField] private float reach = 0.6f;        // seberapa jauh titik cek di depan Player
    [SerializeField] private float radius = 0.3f;       // jari-jari lingkaran cek
    [SerializeField] private LayerMask interactableMask;// hanya cek layer Interactable
    private IActionInput input;

    // Input "disuntikkan" dari luar (dipanggil WorldInstaller)
    public void Construct(IActionInput actionInput) => input = actionInput;

    // Titik di depan Player = posisi + arah hadap * jarak
    private Vector2 ProbePoint => (Vector2)transform.position + movement.Facing * reach;

    void Update()
    {
        if (input == null || !input.InteractPressed) return;   // E tidak ditekan? selesai

        // Gambar lingkaran tak kasat mata di ProbePoint, ambil collider pertama di layer Interactable
        Collider2D hit = Physics2D.OverlapCircle(ProbePoint, radius, interactableMask);

        // Kalau ada dan punya IInteractable, panggil Interact
        if (hit != null && hit.TryGetComponent(out IInteractable target))
            target.Interact(this);
    }
}