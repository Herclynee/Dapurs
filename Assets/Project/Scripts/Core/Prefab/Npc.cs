using System.Collections;
using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName = "Warga";
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private Sprite faceDown, faceLeft, faceRight, faceUp;
    [SerializeField] private DialogueData dialogue;           // dipakai Sari

    protected GameServices Services { get; private set; }
    protected DialogueRunner Dialogue { get; private set; }
    protected StoryFlags Flags { get; private set; }
    protected string NpcName => npcName;

    public void Construct(GameServices s)                     // dipanggil WorldInstaller
    {
        Services = s;
        Dialogue = s.Dialogue;
        Flags = s.Flags;
        OnConstructed();
    }

    public void Interact(PlayerInteractor who)
    {
        if (Dialogue == null || Dialogue.IsRunning) return;
        Face(who.transform.position - transform.position);
        Dialogue.Run(Talk());
    }

    // Versi dasar: DialogueData biasa (dipakai Sari)
    protected virtual IEnumerator Talk()
    {
        if (dialogue != null) yield return Dialogue.PlayData(dialogue);
    }

    protected virtual void OnConstructed() { }

    private void Face(Vector2 d) =>
        body.sprite = Mathf.Abs(d.x) > Mathf.Abs(d.y)
            ? (d.x > 0 ? faceRight : faceLeft)
            : (d.y > 0 ? faceUp : faceDown);
}