using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartnerHud : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI txtHP;
    [SerializeField] private Image hpFill;
    [SerializeField] private Image expFill;
    [SerializeField] private TextMeshProUGUI txtPotions; // Tetap ada, tapi opsional

    private PlayerParty party;

    public void Construct(PlayerParty playerParty)
    {
        party = playerParty;
        UpdateVisuals();
    }

    void Update()
    {
        if (party != null && party.Partner != null)
        {
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (party.Partner == null) return;

        var p = party.Partner;
        
        if (txtName != null)
            txtName.text = p.Data.DisplayName;
            
        if (txtHP != null)
            txtHP.text = $"{p.CurrentHP}/{p.MaxHP}";
        
        if (hpFill != null)
        {
            float ratio = Mathf.Max(0, p.CurrentHP / (float)p.MaxHP);
            hpFill.fillAmount = ratio;
            hpFill.color = HpColors.For(ratio);
        }

        if (expFill != null)
        {
            expFill.fillAmount = p.Exp / (float)p.ExpToNext;
        }

        // HAPUS/COMMENT bagian Potions karena tidak ada di PlayerParty
        // if (txtPotions != null)
        // {
        //     txtPotions.text = $"Ramuan x{party.Potions}";
        // }
    }
}