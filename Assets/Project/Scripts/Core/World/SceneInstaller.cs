using System.Collections.Generic;
using UnityEngine;

public abstract class SceneInstaller : MonoBehaviour
{
    public abstract void Install(GameServices services);

    protected List<T> FindInScene<T>() where T : Component
    {
        var result = new List<T>();
        foreach (T c in FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (c.gameObject.scene == gameObject.scene) result.Add(c);
        return result;
    }
}