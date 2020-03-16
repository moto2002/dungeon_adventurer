using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image backgroundImage;

    Action<int> _onCharClicked;
    Action<BattleSlot, int> _onTargetCallback;
    bool _isTargeted;
    public int AppliedId
    {
        get
        {
            if (transform.childCount > 0)
                return transform.GetChild(0).GetComponent<BattleChar>().ReturnCharId();
            if (_onCharClicked != null)
                RemoveChar();
            return 0;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isTargeted)
        {
            _onTargetCallback?.Invoke(this, AppliedId);
        }
        if (AppliedId == 0) return;
        _onCharClicked?.Invoke(AppliedId);
    }

    public void SetData(Action<int> callback, Action<BattleSlot, int> targetCallback)
    {
        _onCharClicked = callback;
        _onTargetCallback = targetCallback;
    }

    public void RemoveChar()
    {
        _onCharClicked = null;
        _onTargetCallback = null;
    }

    public void SetColor(Color c)
    {
        backgroundImage.color = c;
    }

    public void SetTarget(bool isTarget)
    {
        backgroundImage.color = isTarget ? Colors.TARGET_HIGHLIGHT : Color.white;
        _isTargeted = isTarget;
    }

    public Transform GetBattlecharTransform()
    {
        if (transform.childCount > 0)
            return transform.GetChild(0);
        else
            return null;
    }
}
