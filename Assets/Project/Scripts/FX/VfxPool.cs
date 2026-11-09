// VfxPool.cs: pool bawaan Unity (UnityEngine.Pool, Unity 2021+)
using UnityEngine;
using UnityEngine.Pool;

public class VfxPool : MonoBehaviour
{
    [SerializeField] private SpriteVfx prefab;
    private ObjectPool<SpriteVfx> pool;

    void Awake()
    {
        pool = new ObjectPool<SpriteVfx>(
            createFunc: () => { var v = Instantiate(prefab, transform); v.Init(x => pool.Release(x)); return v; },
            actionOnGet: v => v.gameObject.SetActive(true),
            actionOnRelease: v => v.gameObject.SetActive(false),
            actionOnDestroy: v => Destroy(v.gameObject),
            collectionCheck: false, defaultCapacity: 4, maxSize: 16);
    }
    public void Play(Vector3 pos) => pool.Get().Play(pos);
}