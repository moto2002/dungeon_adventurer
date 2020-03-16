using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionButton : UIBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image percentageImage;
    [SerializeField] GameObject lockedImage;

    [SerializeField] TextMeshProUGUI desciption;
    [SerializeField] TextMeshProUGUI skillInfo;

    Option _data;
    Hero[] _heroes;
    Action<Option> _callback;

    protected override void Awake()
    {
        button.onClick.AddListener(Clicked);
    }

    public void SetData(Option data, Action<Option> optionCallback, params Hero[] heroes)
    {
        _heroes = heroes;
        _data = data;
        _callback = optionCallback;
        SetButtonData();
        SetSuccessRate();
    }

    void SetButtonData()
    {
        desciption.text = _data.startText;

        if(_data.requirements.Length != 0)
        {
            var heroValue = 0;
            foreach (var hero in _heroes)
            {
                heroValue += hero.Main.GetValue(_data.requirements[0].stat);
            }
            var interactable = _data.requirements[0].Value <= heroValue;
            button.interactable = interactable;
            lockedImage.SetActive(!interactable);
        } else
        {
            button.interactable = true;
        }
    }

    void SetSuccessRate()
    {
        if (!_data.successRate.NeedRoll)
        {
            skillInfo.text = string.Empty;
            percentageImage.color = new Color(1f, 1f, 1f, 0f);
            percentageImage.transform.localScale = Vector3.one;
        } else
        {
            percentageImage.color = Colors.ByMainStat(_data.successRate.stat);
            var heroValue = 0;
            foreach (var hero in _heroes)
            {
                heroValue += hero.Main.GetValue(_data.successRate.stat);
            }

            Debug.Log($"{Mathf.Max(heroValue - _data.successRate.MinValue, 0f)} {_data.successRate.Difference}");
            var successChance = Mathf.Min((Mathf.Max(heroValue - _data.successRate.MinValue, 0f) / _data.successRate.Difference), 0.95f);
            percentageImage.transform.localScale = new Vector3(successChance, 1f, 1f);
            skillInfo.text = $"{_data.successRate.stat.ToString()} Check: {successChance:P1}";
        }
    }

    void Clicked()
    {
        _callback?.Invoke(_data);
    }
}
