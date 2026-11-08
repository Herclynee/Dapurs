using UnityEngine;

[CreateAssetMenu(fileName = "Dlg_", menuName = "Pulau Monster/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private string speaker;
    [SerializeField, TextArea] private string[] lines;
    public string Speaker => speaker;
    public string[] Lines => lines;
}