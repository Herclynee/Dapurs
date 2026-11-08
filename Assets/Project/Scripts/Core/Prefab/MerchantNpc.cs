using System.Collections;
using UnityEngine;

public class MerchantNpc : Npc
{
    [SerializeField] private AudioClip healClip;

    protected override IEnumerator Talk()
    {
        if (Flags.Has(StoryFlags.Gift))
        {
            yield return Dialogue.Say(NpcName, "Semoga perjalananmu menyenangkan!");
            yield break;
        }
        yield return Dialogue.Say(NpcName, "Petualang baru? Bawa 3 Ramuan ini. Gratis untuk hari pertama!");
        Services.Party.AddPotions(3);                          // 3 + 3 = Ramuan ×6
        Flags.Set(StoryFlags.Gift);
        if (Services.Audio != null) Services.Audio.PlaySfx(healClip, 0f);
        Services.Toast.Show("Mendapat 3 Ramuan");
    }
}