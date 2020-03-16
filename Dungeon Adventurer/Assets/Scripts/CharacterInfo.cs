using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI strValue;
    [SerializeField] TextMeshProUGUI conValue;
    [SerializeField] TextMeshProUGUI dexValue;
    [SerializeField] TextMeshProUGUI intValue;
    [SerializeField] TextMeshProUGUI lckValue;

    [SerializeField] SkillEntry[] skillSlots;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI classText;

    [SerializeField] TextMeshProUGUI healthValue;
    [SerializeField] Slider healthSlider;

    Hero _viewedHero;

    public void SetData(Hero viewedHero)
    {
        _viewedHero = viewedHero;

        SetGeneralInfo();
        SetStats();
        SetSkills();
    }

    void SetGeneralInfo()
    {
        nameText.text = _viewedHero.displayName;
        classText.text = $"{_viewedHero.fightClass}";
        levelText.text = $"{_viewedHero.GetLevel()}";
        healthValue.text = $"{_viewedHero.MaxLife}";
    }

    void SetStats()
    {
        strValue.text = $"{_viewedHero.Main.str}";
        conValue.text = $"{_viewedHero.Main.con}";
        dexValue.text = $"{_viewedHero.Main.dex}";
        intValue.text = $"{_viewedHero.Main.intel}";
        lckValue.text = $"{_viewedHero.Main.lck}";
    }

    void SetSkills()
    {
        for (var i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].SetData(_viewedHero, _viewedHero.Skills[i], -1, null);
        }
    }
}
