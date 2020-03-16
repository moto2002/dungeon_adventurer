using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleChar : MonoBehaviour
{
    [SerializeField] Image charIcon;
    [SerializeField] Animator feedBackAnimator;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] StatusBar statusBar;

    Character _appliedChar;
    CharacterChangeDisplayCollection _displayCollection;

    public void SetData(Character character, Sprite sprite, CharacterChangeDisplayCollection displayCollection)
    {
        _displayCollection = displayCollection;
        _appliedChar = character;
        _appliedChar.OnLifeChanged.AddListener(ShowDamage);
        _appliedChar.OnStatusChanged.AddListener(ShowStatus);
        _appliedChar.OnMarkerChanged.AddListener(ShowMarker);
        _appliedChar.OnDotsChanged.AddListener(ShowDot);
        _appliedChar.OnBuffsChanged.AddListener(ShowBuff);
        _appliedChar.OnCharacterDeath.AddListener(OnDeath);

        if(sprite != null)
        charIcon.sprite = sprite;
    }

    private void OnDisable()
    {
        if (_appliedChar == null) return;
        _appliedChar.OnLifeChanged.RemoveListener(ShowDamage);
        _appliedChar.OnStatusChanged.RemoveListener(ShowStatus);
        _appliedChar.OnMarkerChanged.RemoveListener(ShowMarker);
        _appliedChar.OnCharacterDeath.RemoveListener(OnDeath);
    }

    void OnDeath(int id)
    {
        if (_appliedChar.id == id)
            feedBackAnimator.SetTrigger("Death");
    }

    public void RemoveCharacter()
    {
        Destroy(gameObject);
    }

    public void ShowDamage(int newValue)
    {
        var isPos = (newValue >= 0);
        healthText.text = isPos ? $"+{newValue}" : $"{newValue}";
        healthText.color = isPos ? Colors.BATTLE_ACTIVE : Colors.TARGET_HIGHLIGHT;
        feedBackAnimator.SetTrigger("Health");
    }

    public void ShowStatus(StatusInfo info)
    {
        statusBar.ShowStatus(info, _displayCollection.GetDataByType(info.Type));
    }

    public void ShowMarker(MarkerInfo info)
    {
       statusBar.ShowMarker(info, _displayCollection.GetDataByType(info.Type));
    }

    public void ShowDot(DotInfo info)
    {
       statusBar.ShowDots(info, _displayCollection.GetDataByType(info.Type));
    }

        public void ShowBuff(BuffInfo info)
    {
       statusBar.ShowBuff(info, _displayCollection.GetDataByType(info.AffectedStat));
    }

    public Character GetCharacter()
    {
        return _appliedChar;
    }

    public int ReturnCharId()
    {
        return _appliedChar.id;
    }
}
