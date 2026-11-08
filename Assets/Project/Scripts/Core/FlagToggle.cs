using UnityEngine;

public class FlagToggle : MonoBehaviour
{
    [SerializeField] private GameObject hideWhenSet;
    [SerializeField] private string flagKey = StoryFlags.HasStarter;
    private StoryFlags flags;

    public void Construct(StoryFlags storyFlags)
    {
        flags = storyFlags;
        flags.Changed += OnChanged;
        Apply();
    }

    private void OnChanged(string key) { if (key == flagKey) Apply(); }
    private void Apply() => hideWhenSet.SetActive(!flags.Has(flagKey));
    void OnDestroy() { if (flags != null) flags.Changed -= OnChanged; }
}