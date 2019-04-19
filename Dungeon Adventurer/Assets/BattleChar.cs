using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleChar : MonoBehaviour
{
    [SerializeField] SpriteRenderer charIcon;
    [SerializeField] Animator feedBackAnimator;
    [SerializeField] TextMeshProUGUI healthText;

    Character _appliedChar;

    public void SetData(Character character, Sprite sprite) {

        _appliedChar = character;
        _appliedChar.OnLifeChanged.AddListener(ShowDamage);
        _appliedChar.OnStatusChanged.AddListener(ShowStatus);
        _appliedChar.OnMarkerChanged.AddListener(ShowMarker);
        _appliedChar.OnCharacterDeath.AddListener(OnDeath);
        charIcon.sprite = sprite;
    }

    private void OnDisable() {
        _appliedChar.OnLifeChanged.RemoveListener(ShowDamage);
        _appliedChar.OnStatusChanged.RemoveListener(ShowStatus);
        _appliedChar.OnMarkerChanged.RemoveListener(ShowMarker);
        _appliedChar.OnCharacterDeath.RemoveListener(OnDeath);
    }

    void OnDeath(int id) {
        if (_appliedChar.id == id)
            feedBackAnimator.SetTrigger("Death");
    }

    public void RemoveCharacter() {

        Destroy(gameObject);

    }

    public void ShowDamage(int newValue) {
        var isPos = (newValue >= 0);
        healthText.text = isPos ? $"+{newValue}" : $"{newValue}";
        healthText.color = isPos ? Colors.BATTLE_ACTIVE : Colors.TARGET_HIGHLIGHT;
        feedBackAnimator.SetTrigger("Health");
    }

    public void ShowStatus() {
        foreach (var markers in _appliedChar.GetAppliedStatuses()) {
            Debug.Log($"Char has {markers.Key.ToString()} Statuses");
        }
    }

    public void ShowMarker() {
        Debug.Log("Marker");
        foreach (var markers in _appliedChar.GetAppliedMarkers()) {
            Debug.Log($"Char has {markers.Key.ToString()} Markers");
        }
    }

    public int ReturnCharId() {
        return _appliedChar.id;
    }
}
