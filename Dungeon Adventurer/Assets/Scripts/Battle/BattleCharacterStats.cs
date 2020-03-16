using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleCharacterStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI charName;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI type;

    [SerializeField] TextMeshProUGUI str;
    [SerializeField] TextMeshProUGUI con;
    [SerializeField] TextMeshProUGUI dex;
    [SerializeField] TextMeshProUGUI intel;
    [SerializeField] TextMeshProUGUI lck;

    [SerializeField] TextMeshProUGUI life;
    [SerializeField] TextMeshProUGUI mana;
    [SerializeField] Slider lifeSlider;
    [SerializeField] Slider manaSlider;

    Hero _selectedHero = null;

    private void OnEnable()
    {
        BattleView.SelectedCharChanged += Refresh;
    }
    private void OnDisable()
    {
        BattleView.SelectedCharChanged -= Refresh;
    }

    void Refresh()
    {
        var c = BattleView.SelectedChar;
        if (c.id < 0) return;
        if (_selectedHero != null)
            _selectedHero.OnLifeChanged.RemoveListener(DisplayValues);

        _selectedHero = (Hero)c;
        _selectedHero.OnLifeChanged.AddListener(DisplayValues);
        DisplayValues(0);
    }

    void DisplayValues(int o)
    {
        charName.text = _selectedHero.displayName;
        type.text = _selectedHero.fightClass.ToString();
        life.text = $"{_selectedHero.CurrentLife}";
        mana.text = $"{_selectedHero.CurrentMana}";

        str.text = $"{_selectedHero.Main.str}";
        con.text = $"{_selectedHero.Main.con}";
        dex.text = $"{_selectedHero.Main.dex}";
        intel.text = $"{_selectedHero.Main.intel}";
        lck.text = $"{_selectedHero.Main.lck}";

        lifeSlider.maxValue = _selectedHero.MaxLife;
        lifeSlider.value = _selectedHero.CurrentLife;

        manaSlider.maxValue = _selectedHero.MaxMana;
        manaSlider.value = _selectedHero.CurrentMana;
    }
}
