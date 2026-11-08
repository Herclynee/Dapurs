using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMovement : MonoBehaviour
{
[SerializeField] private PlayerConfig config;   

    private IMoveInput input;                     
    private Rigidbody2D body;
    private Vector2 velocity;                     

    public Vector2 Velocity => velocity;
    public Vector2 Facing { get; private set; } = Vector2.down;

    public void Construct(IMoveInput moveInput) => input = moveInput;

    void Awake() => body = GetComponent<Rigidbody2D>();

    void Update()
    {
        if (input == null) return;
        Vector2 move = input.Move;
        float speed = input.IsRunning ? config.RunSpeed : config.WalkSpeed;
        velocity = move * speed;
        if (move.sqrMagnitude > 0.01f) Facing = ToCardinal(move);
    }

    void FixedUpdate() => body.linearVelocity = velocity;

    private static Vector2 ToCardinal(Vector2 v) =>
        Mathf.Abs(v.x) > Mathf.Abs(v.y)
            ? new Vector2(Mathf.Sign(v.x), 0f)
            : new Vector2(0f, Mathf.Sign(v.y));

    public void Teleport(Vector2 position, Vector2 facing)
    {
        transform.position = position;
        if (body != null) { body.position = position; body.linearVelocity = Vector2.zero; }
        velocity = Vector2.zero;
        Facing = facing;
        Physics2D.SyncTransforms();
    }
}