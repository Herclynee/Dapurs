using System.Collections;
using UnityEngine;

public class AudioService : MonoBehaviour
{
    [SerializeField] private AudioSource musicA, musicB, sfx;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField] private AudioClip moveClip, confirmClip, cancelClip, blipClip;

    private AudioSource active, standby;
    private AudioClip worldMusic;
    private Coroutine fading;

    public bool Muted { get; private set; }

    void Awake()
    {
        active = musicA; standby = musicB;
        musicA.loop = true; musicB.loop = true;
        AreaZone.Entered += OnArea;
    }

    void OnDestroy() => AreaZone.Entered -= OnArea;

    void Update() { if (MenuInput.Mute) ToggleMute(); }                // tombol M

    private void OnArea(AreaZone zone) { if (zone.Music != null) PlayWorldMusic(zone.Music); }

    public void PlayWorldMusic(AudioClip clip) { worldMusic = clip; PlayMusic(clip); }
    public void PlayBattleMusic(AudioClip clip) => PlayMusic(clip);
    public void ResumeWorldMusic() => PlayMusic(worldMusic);           // dipanggil SceneFlow setelah battle

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (active.clip == clip && active.isPlaying) return;
        if (fading != null) StopCoroutine(fading);
        fading = StartCoroutine(Crossfade(clip));
    }

    private IEnumerator Crossfade(AudioClip clip)                      // 1 detik, tidak terpengaruh timescale
    {
        AudioSource outgoing = active, incoming = standby;
        active = incoming; standby = outgoing;
        incoming.clip = clip;
        incoming.volume = 0f;
        incoming.Play();
        float startOut = outgoing.volume;
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime)
        {
            incoming.volume = Mathf.Lerp(0f, musicVolume, t);
            outgoing.volume = Mathf.Lerp(startOut, 0f, t);
            yield return null;
        }
        incoming.volume = musicVolume;
        outgoing.Stop();
        outgoing.volume = 0f;
    }

    public void PlaySfx(AudioClip clip, float pitchJitter = 0.05f)
    {
        if (clip == null) return;
        sfx.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        sfx.PlayOneShot(clip);
    }

    public void PlayMove() => PlaySfx(moveClip, 0f);
    public void PlayConfirm() => PlaySfx(confirmClip, 0f);
    public void PlayCancel() => PlaySfx(cancelClip, 0f);
    public void PlayBlip() => PlaySfx(blipClip, 0.1f);

    public bool ToggleMute()
    {
        Muted = !Muted;
        AudioListener.volume = Muted ? 0f : 1f;
        return Muted;
    }
}