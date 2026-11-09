    using UnityEngine;

   [RequireComponent(typeof(Animator))]
   public class PlayerAnimator : MonoBehaviour
   {
       // Mengubah nama parameter string menjadi angka (lebih cepat untuk Unity)
       private static readonly int MoveX = Animator.StringToHash("MoveX");
       private static readonly int MoveY = Animator.StringToHash("MoveY");
       private static readonly int IsMoving = Animator.StringToHash("IsMoving");

       [SerializeField] private PlayerMovement movement;
       private Animator animator;

       void Awake() => animator = GetComponent<Animator>();

       void Update()
       {
           // Cek apakah player bergerak (kecepatan > 0.01)
           animator.SetBool(IsMoving, movement.Velocity.sqrMagnitude > 0.01f);
           
           // Kirim arah hadap ke Blend Tree
           animator.SetFloat(MoveX, movement.Facing.x);
           animator.SetFloat(MoveY, movement.Facing.y);
       }
   }