using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : UIBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] bool moveX;
    [SerializeField] bool moveY;

    [SerializeField] int minX;
    [SerializeField] int maxX;
    [SerializeField] int minY;
    [SerializeField] int maxY;

    Vector2 _startPos;
    Vector2 _currentPos;
    Vector2 _oldPos;

    public void OnDrag(PointerEventData eventData)
    {
        _currentPos = eventData.position;
        transform.localPosition = new Vector2(
            moveX ? Mathf.Clamp(_oldPos.x + (_currentPos.x - _startPos.x), minX, maxX) : _oldPos.x,
            moveY ? Mathf.Clamp(_oldPos.y + (_currentPos.y - _startPos.y), minY, maxY) : _oldPos.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _oldPos = transform.localPosition;
        _startPos = eventData.position;
    }
}
