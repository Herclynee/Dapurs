using UnityEngine;

public class Npc : MonoBehaviour, IInteractable   // satu induk (MonoBehaviour) + interface
{
    [SerializeField] private string npcName = "Warga";
    [SerializeField] private SpriteRenderer body;                       // pemilik gambar NPC
    [SerializeField] private Sprite faceDown, faceLeft, faceRight, faceUp;  // 4 sprite hadap

    public void Interact(PlayerInteractor who)
    {
        // Arah dari NPC ke Player = posisi Player - posisi NPC
        Face(who.transform.position - transform.position);
        Debug.Log($"{npcName}: Halo!");    // nanti diganti dialog sungguhan
    }

    private void Face(Vector2 d) =>
        body.sprite = Mathf.Abs(d.x) > Mathf.Abs(d.y)
            ? (d.x > 0 ? faceRight : faceLeft)   // lebih horizontal
            : (d.y > 0 ? faceUp : faceDown);     // lebih vertikal
}