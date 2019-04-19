using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleSlot : MonoBehaviour, IPointerClickHandler {
    [SerializeField] Image backgroundImage;

    Action<int> _onCharClicked;
    Action<int> _onTargetCallback;
    bool _isTargeted;
    int _appliedID {
        get {
            if (transform.childCount > 0)
                return transform.GetChild(0).GetComponent<BattleChar>().ReturnCharId();
            if (_onCharClicked != null)
                RemoveChar();
            return 0;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (_appliedID == 0) return;
        _onCharClicked?.Invoke(_appliedID);
        if (_isTargeted) {
            _onTargetCallback?.Invoke(_appliedID);
            SetColor(Color.gray);
        }
    }

    public void SetData(Action<int> callback, Action<int> targetCallback) {
        _onCharClicked = callback;
        _onTargetCallback = targetCallback;
    }

    public void RemoveChar() {
        _onCharClicked = null;
        _onTargetCallback = null;
    }

    public void SetColor(Color c) {
        backgroundImage.color = c;
    }

    public void SetTarget(bool isTarget) {
        backgroundImage.color = isTarget ? Colors.TARGET_HIGHLIGHT : Color.white;
        _isTargeted = isTarget;
    }
}
