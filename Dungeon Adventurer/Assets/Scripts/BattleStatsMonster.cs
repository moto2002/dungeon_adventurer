using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatsMonster : MonoBehaviour {
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI level;

    [SerializeField] TextMeshProUGUI life;
    [SerializeField] Slider lifeSlider;

    Monster _selectedMonster = null;

    private void OnEnable() {
        BattleView.SelectedCharChanged += Refresh;
    }
    private void OnDisable() {
        BattleView.SelectedCharChanged -= Refresh;
    }

    void Refresh() {
        var c = BattleView.SelectedChar;
        if (c.id > 0) return;
        if (_selectedMonster != null)
            _selectedMonster.OnLifeChanged.RemoveListener(DisplayValues);

        _selectedMonster = (Monster)c;
        _selectedMonster.OnLifeChanged.AddListener(DisplayValues);
        DisplayValues(0);
    }

    void DisplayValues(int o) {
        name.text = _selectedMonster.name;
        level.text = $"{_selectedMonster.level}";
        life.text = $"{_selectedMonster.CurrentLife}";

        lifeSlider.maxValue = _selectedMonster.maxLife;
        lifeSlider.value = _selectedMonster.CurrentLife;
    }
}
