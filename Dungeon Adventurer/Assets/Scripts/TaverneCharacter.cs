using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaverneCharacter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI clas;
    [SerializeField] TextMeshProUGUI level;

    [SerializeField] TextMeshProUGUI str;
    [SerializeField] TextMeshProUGUI con;
    [SerializeField] TextMeshProUGUI dex;
    [SerializeField] TextMeshProUGUI intel;
    [SerializeField] TextMeshProUGUI lck;

    [SerializeField] Slider lifeSlider;
    [SerializeField] TextMeshProUGUI life;

    [SerializeField] Image border;
    [SerializeField] Image portrait;

    Hero _appliedCharacter;

    public void SetData(Hero chars) {

        _appliedCharacter = chars;

        name.text = _appliedCharacter.name;
        clas.text = _appliedCharacter.fightClass.ToString();
        level.text = "" + 1;

        str.text = $"{_appliedCharacter.main.str}";
        con.text = $"{_appliedCharacter.main.con}";
        dex.text = $"{_appliedCharacter.main.dex}";
        intel.text = $"{_appliedCharacter.main.intel}";
        lck.text = $"{_appliedCharacter.main.lck}";

        lifeSlider.maxValue = _appliedCharacter.maxLife;
        lifeSlider.value = _appliedCharacter.maxLife;
        life.text = $"{_appliedCharacter.maxLife}";

        border.color = Colors.ByRarity(_appliedCharacter.rarity);
        portrait.sprite = DataHolder._data.raceImages[(int)_appliedCharacter.race];
    }
}
