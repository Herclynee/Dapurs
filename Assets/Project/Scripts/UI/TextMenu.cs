using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextMenu : MonoBehaviour
{
    [SerializeField] private Text[] rows;        // Btn_0..Btn_3 (komponen Text)
    [SerializeField] private int columns = 1;    // battle: 2

    public event Action<int> Picked;
    public int Selected { get; private set; }

    private string[] labels;
    private bool[] usable;
    private int count;
    private bool acceptInput;
    private bool locked;

    void Awake()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            int index = i;                                   // SALINAN LOKAL, wajib (lihat tabel masalah)
            rows[i].raycastTarget = true;

            Button btn = rows[i].GetComponent<Button>();
            if (btn == null) btn = rows[i].gameObject.AddComponent<Button>();   // jangan pakai '??'
            btn.targetGraphic = rows[i];
            btn.transition = Selectable.Transition.None;
            btn.navigation = new Navigation { mode = Navigation.Mode.None };
            btn.onClick.AddListener(() => Click(index));

            EventTrigger trig = rows[i].GetComponent<EventTrigger>();
            if (trig == null) trig = rows[i].gameObject.AddComponent<EventTrigger>();
            var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener(_ => Hover(index));
            trig.triggers.Add(enter);
        }
    }

    public void Open(string[] options, bool[] enabled = null, int start = 0)
    {
        labels = options;
        count = Mathf.Min(options.Length, rows.Length);
        usable = enabled ?? new bool[count];
        if (enabled == null) for (int i = 0; i < count; i++) usable[i] = true;

        for (int i = 0; i < rows.Length; i++) rows[i].gameObject.SetActive(i < count);

        Selected = Mathf.Clamp(start, 0, count - 1);
        if (!usable[Selected]) Selected = FirstUsable();

        gameObject.SetActive(true);
        acceptInput = false;       // abaikan tombol yang tadi membuka menu
        locked = false;
        Redraw();
    }

    public void Close()
    {
        count = 0;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (count == 0 || locked) return;
        if (!acceptInput) { acceptInput = true; return; }

        if (MenuInput.Down)  Move(+1, 0);
        if (MenuInput.Up)    Move(-1, 0);
        if (MenuInput.Right) Move(0, +1);
        if (MenuInput.Left)  Move(0, -1);
        if (MenuInput.Confirm) Pick(Selected);
    }

    private void Move(int dRow, int dCol)
    {
        int row = Selected / columns + dRow;
        int col = Selected % columns + dCol;
        if (col < 0 || col >= columns || row < 0) return;
        int index = row * columns + col;
        if (index >= count) return;
        Selected = index;
        Redraw();
    }

    private void Pick(int index)
    {
        if (index < 0 || index >= count || !usable[index]) return;   // redup: tidak bisa dipilih
        locked = true;                                               // cegah pilih ganda
        Picked?.Invoke(index);
    }

    private void Click(int index) { if (count != 0 && !locked) { Selected = index; Redraw(); Pick(index); } }
    private void Hover(int index) { if (count != 0 && index < count && Selected != index) { Selected = index; Redraw(); } }

    private int FirstUsable()
    {
        for (int i = 0; i < count; i++) if (usable[i]) return i;
        return 0;
    }

    private void Redraw()
    {
        for (int i = 0; i < count; i++)
        {
            bool on = i == Selected;
            rows[i].text = (on ? "▶ " : "   ") + labels[i];
            rows[i].color = !usable[i] ? UiTheme.Disabled : on ? UiTheme.Gold : UiTheme.Text;
        }
    }
}