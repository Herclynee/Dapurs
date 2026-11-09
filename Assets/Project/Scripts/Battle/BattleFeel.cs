using System.Collections;
using UnityEngine;

public class BattleFeel : MonoBehaviour
{
    [SerializeField] private Transform cameraRig;          // Main Camera
    [SerializeField] private SpriteRenderer playerBody;    // PlayerMon
    [SerializeField] private SpriteRenderer enemyBody;     // EnemyMon
    [SerializeField] private PixelBurst burst;
    [SerializeField] private VfxPool hitVfx;               // boleh kosong

    private BattleSystem battle;
    private Vector3 cameraHome;
    private int shakes;

    public void Construct(BattleSystem battleSystem)
    {
        battle = battleSystem;
        cameraHome = cameraRig.localPosition;
        battle.Hit += OnHit;
        battle.Healed += OnHealed;
    }

    private void OnHit(MonsterInstance target, int damage, bool critical, Element? element)
    {
        SpriteRenderer body = target == battle.Player ? playerBody : enemyBody;
        Vector3 center = body.bounds.center;

        if (hitVfx != null) hitVfx.Play(center);                                   // tebasan 4 frame
        Color c = element.HasValue ? StarterGiver.ElementColor(element.Value) : Color.white;
        burst.Play(center, c, element.HasValue ? 22 : 12, 90f, 0.5f);              // Jurus 22 percikan, Serang 12
        StartCoroutine(Shake(critical ? 0.35f : 0.2f));
        StartCoroutine(HitStop());
    }

    private void OnHealed(MonsterInstance m)
        => burst.Play(playerBody.bounds.center, HpColors.High, 16, 50f, 0.6f);     // percikan hijau

    private IEnumerator Shake(float duration)
    {
        shakes++;
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)             // unscaled: tetap jalan saat hit-stop
        {
            float k = 1f - t / duration;                                           // makin lama makin kecil
            float x = Mathf.Round(Random.Range(-1f, 1f) * 3f * k) / 16f;           // kelipatan 1 piksel
            float y = Mathf.Round(Random.Range(-1f, 1f) * 3f * k) / 16f;
            cameraRig.localPosition = cameraHome + new Vector3(x, y, 0f);
            yield return null;
        }
        shakes--;
        if (shakes <= 0) { shakes = 0; cameraRig.localPosition = cameraHome; }
    }

    private IEnumerator HitStop()
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(0.06f);                            // Realtime: tidak ikut melambat
        Time.timeScale = 1f;
    }

    void OnDestroy()
    {
        if (battle != null) { battle.Hit -= OnHit; battle.Healed -= OnHealed; }
        Time.timeScale = 1f;                                                       // WAJIB
    }
}