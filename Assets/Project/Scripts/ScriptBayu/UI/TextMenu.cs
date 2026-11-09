using System;
using UnityEngine;
using TMPro; // Ganti dari UnityEngine.UI ke TMPro

public class TextMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI[] rows; // Ganti Text ke TextMeshProUGUI
    
    [Header("Colors")]
    [SerializeField] private Color normal = UiTheme.Text;
    [SerializeField] private Color selected = UiTheme.Gold;
    [SerializeField] private Color disabled = UiTheme.Disabled;

    public event Action<int> Picked;
    public int Selected => selectedIdx;

    private string[] labels;
    private bool[] usable;
    private int count;
    private int selectedIdx;
    private bool acceptInput;

    public void Open(string[] options, bool[] enabled = null, int start = 0)
    {
        labels = options;
        count = Mathf.Min(options.Length, rows.Length);
        usable = enabled ?? new bool[count];
        if (enabled == null) for (int i = 0; i < count; i++) usable[i] = true;

        for (int i = 0; i < rows.Length; i++) rows[i].gameObject.SetActive(i < count);

        selectedIdx = Mathf.Clamp(start, 0, count - 1);
        if (!usable[selectedIdx]) selectedIdx = FirstUsable();

        gameObject.SetActive(true);
        acceptInput = false;
        Redraw();
    }

    void Update()
    {
        if (count == 0) return;
        if (!acceptInput) { acceptInput = true; return; }

        if (MenuInput.Down) Move(1);
        if (MenuInput.Up) Move(-1);
        if (MenuInput.Confirm) Pick(selectedIdx);
    }

    private void Move(int dir)
    {
        int next = selectedIdx + dir;
        if (next < 0) next = count - 1;
        if (next >= count) next = 0;

        if (!usable[next])
        {
            int start = next;
            do {
                next += dir;
                if (next < 0) next = count - 1;
                if (next >= count) next = 0;
            } while (next != start && !usable[next]);
        }

        if (usable[next] && next != selectedIdx)
        {
            selectedIdx = next;
            Redraw();
        }
    }

    private void Pick(int index)
    {
        if (index < 0 || index >= count || !usable[index]) return;
        Picked?.Invoke(index);
    }

    private void Redraw()
    {
        for (int i = 0; i < count; i++)
        {
            bool on = i == selectedIdx;
            rows[i].text = (on ? "▶ " : "  ") + labels[i];
            rows[i].color = !usable[i] ? disabled : on ? selected : normal;
        }
    }

    private int FirstUsable()
    {
        for (int i = 0; i < count; i++) if (usable[i]) return i;
        return 0;
    }

    public void Click(int index)
    {
        if (index >= 0 && index < count && usable[index])
        {
            selectedIdx = index;
            Redraw();
            Pick(index);
        }
    }

    public void Hover(int index)
    {
        if (index >= 0 && index < count && usable[index])
        {
            selectedIdx = index;
            Redraw();
        }
    }
}