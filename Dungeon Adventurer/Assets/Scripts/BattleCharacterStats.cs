using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleCharacterStats : MonoBehaviour {
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI type;

    [SerializeField] TextMeshProUGUI str;
    [SerializeField] TextMeshProUGUI con;
    [SerializeField] TextMeshProUGUI dex;
    [SerializeField] TextMeshProUGUI intel;
    [SerializeField] TextMeshProUGUI lck;

    [SerializeField] TextMeshProUGUI life;
    [SerializeField] Slider lifeSlider;

    Hero _selectedHero = null;

    private void OnEnable() {
        BattleView.SelectedCharChanged += Refresh;
    }
    private void OnDisable() {
        BattleView.SelectedCharChanged -= Refresh;
    }

    void Refresh() {
        var c = BattleView.SelectedChar;
        if (c.id < 0) return;
        if (_selectedHero != null)
            _selectedHero.OnLifeChanged.RemoveListener(DisplayValues);

        _selectedHero = (Hero)c;
        _selectedHero.OnLifeChanged.AddListener(DisplayValues);
        DisplayValues(0);
    }

    void DisplayValues(int o) {
        name.text = _selectedHero.name;
        type.text = _selectedHero.fightClass.ToString();
        life.text = $"{_selectedHero.CurrentLife}";

        str.text = $"{_selectedHero.main.str}";
        con.text = $"{_selectedHero.main.con}";
        dex.text = $"{_selectedHero.main.dex}";
        intel.text = $"{_selectedHero.main.intel}";
        lck.text = $"{_selectedHero.main.lck}";

        lifeSlider.maxValue = _selectedHero.maxLife;
        lifeSlider.value = _selectedHero.CurrentLife;
    }
}
