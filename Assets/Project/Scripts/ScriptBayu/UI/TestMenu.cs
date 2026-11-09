using UnityEngine;

public class TestMenu : MonoBehaviour
{
    [SerializeField] private TextMenu menu;

    void Start()
    {
        // Tampilkan menu dengan 4 pilihan saat game mulai
        menu.Open(new string[] { "Serang", "Jurus", "Ramuan", "Kabur" });
    }
}