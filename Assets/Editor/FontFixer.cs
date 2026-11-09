#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class FontFixer
{
    [MenuItem("Pulau Monster/Set Font Pixelify (scene aktif)")]
    private static void Fix()
    {
        string[] guids = AssetDatabase.FindAssets("PixelifySans-Regular t:Font");
        if (guids.Length == 0) { Debug.LogWarning("PixelifySans-Regular tidak ditemukan"); return; }
        Font font = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(guids[0]));

        int count = 0;
        foreach (Text t in Object.FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (t.font != null && t.font.name.StartsWith("PixelifySans")) continue;   // sudah benar
            Undo.RecordObject(t, "Set font");
            t.font = font;
            EditorUtility.SetDirty(t);
            count++;
        }
        Debug.Log($"Font diganti pada {count} teks.");
    }
}
#endif