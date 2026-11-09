using UnityEngine;

public class PartnerFollower : MonoBehaviour
{
    [SerializeField] private SpriteRenderer body;
    private PlayerParty party;
    private PlayerMovement player;

    public void Construct(PlayerParty party, PlayerMovement player)
    {
        this.party = party;
        this.player = player;
    }

    void LateUpdate()
    {
        MonsterInstance m = party != null ? party.Partner : null;
        body.enabled = m != null;
        if (m == null) return;

        // Ikuti posisi player dengan offset
        Vector2 offset = new Vector2(0.5f, 0f); // Sederhana: di sebelah kanan
        transform.position = player.transform.position + (Vector3)offset;
        
        // Tampilkan sprite partner
        body.sprite = m.Data.Frames[0]; 
    }
}
