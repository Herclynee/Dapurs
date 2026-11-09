using System.Collections;
using UnityEngine;

public class SaveStone : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip saveClip;
    [SerializeField] private PixelBurst burst;          // ← TAMBAH: slot untuk PixelBurst
    private GameServices services;
    private Transform player;

    public void Construct(GameServices s, Transform playerTransform) { services = s; player = playerTransform; }

    public void Interact(PlayerInteractor who)
    {
        if (!services.Dialogue.IsRunning) services.Dialogue.Run(Use());
    }

    private IEnumerator Use()
    {
        if (services.Audio != null) services.Audio.PlaySfx(saveClip, 0f);
        if (services.Party.Partner != null) services.Party.Partner.FullHeal();

        // ← TAMBAH: kilau biru di atas batu
        if (burst != null)
            burst.Play(transform.position + Vector3.up * 0.5f,
                       new Color32(120, 190, 255, 255), 14, 40f, 0.7f);

        bool saved = services.Save.Save(SaveMapper.Capture(services, player.position));
        yield return services.Dialogue.Say("", saved
            ? "Batu Simpan bersinar. HP partner pulih dan perjalanan tersimpan."
            : "Batu Simpan bersinar. HP partner pulih, tetapi penyimpanan gagal.");
    }
}