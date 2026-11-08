using UnityEngine;

[RequireComponent(typeof(Animator))]   // otomatis menambahkan Animator jika belum ada
public class PlayerAnimator : MonoBehaviour
{
    // Nama parameter diubah ke angka (hash) SEKALI saja, lebih cepat daripada memakai teks tiap frame
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    [SerializeField] private PlayerMovement movement;   // diisi lewat Inspector
    private Animator animator;

    void Awake() => animator = GetComponent<Animator>();

    void Update()
    {
        // Sedang bergerak?
        animator.SetBool(IsMoving, movement.Velocity.sqrMagnitude > 0.01f);
        // Menghadap ke mana? (dipakai Blend Tree untuk memilih arah)
        animator.SetFloat(MoveX, movement.Facing.x);
        animator.SetFloat(MoveY, movement.Facing.y);
    }
}