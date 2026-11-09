using UnityEngine;
using UnityEngine.UI;

public class PartnerHud : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private Text nameText;      
    [SerializeField] private Image hpFill;
    [SerializeField] private Text hpText;
    [SerializeField] private Image expFill;
    [SerializeField] private Text areaText;      
    [SerializeField] private Text potionText;
    [SerializeField] private GameObject partnerPanel;       // objek PartnerPanel    

    private PlayerParty party;

    public void Construct(PlayerParty playerParty)
    {
        party = playerParty;
        party.PotionsChanged += RefreshPotions;
        AreaZone.Entered += OnArea;
        RefreshPotions();
    }

    void OnDestroy()
    {
        if (party != null) party.PotionsChanged -= RefreshPotions;
        AreaZone.Entered -= OnArea;
    }

    private void OnArea(AreaZone zone) => areaText.text = zone.AreaName;
    private void RefreshPotions() => potionText.text = $"Ramuan ×{party.Potions}";

    void Update()
    {
        if (party == null) return;
        MonsterInstance m = party.Partner;

        if (partnerPanel != null) partnerPanel.SetActive(m != null);   // tampil hanya jika ada partner
        if (m == null) return;

        portrait.sprite = m.Data.Frames[0];
        portrait.preserveAspect = true;
        nameText.text = $"{m.Data.DisplayName}  Lv{m.Level}";
        float r = (float)m.CurrentHP / m.MaxHP;
        hpFill.fillAmount = r;
        hpFill.color = HpColors.For(r);
        hpText.text = $"{m.CurrentHP}/{m.MaxHP}";
        expFill.fillAmount = (float)m.Exp / m.ExpToNext;
    }
}