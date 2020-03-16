using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CompSlot : MonoBehaviour, IPointerClickHandler
{
    static readonly Color DefaultColor = Color.white;
    static readonly Color FavPositionColor = new Color(0f, 1f, 0f, 1f);
    static readonly Color AdvPositionColor = new Color(0f, 0.5f, 0f, 1f);
    static readonly Color WorsePositionColor = new Color(0.5f, 0f, 0f, 1f);
    static readonly Color BadPositionColor = new Color(1f, 0f, 0f, 1f);

    [SerializeField] Image backgroundImage;

    int _position;

    public void OnPointerClick(PointerEventData eventData)
    {
        var slot = BattleOrderView._instance.SlotSelected(_position);
        if (slot != null)
        {
            CleanTransform();
            slot.transform.SetParent(transform);
        }
    }

    public void ApplyBattleOrderSlot(BattleOrderSlot slot)
    {
        CleanTransform();
        slot.transform.SetParent(transform);
        slot.transform.localScale = Vector3.one;
        slot.GetComponent<Image>().enabled = false;
    }

    public void SetData(int pos)
    {
        _position = pos;
    }

    public void CleanTransform()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public int GetPosition()
    {
        return _position;
    }

    public void SetColor(int posSkills)
    {
        switch (posSkills)
        {
            case 0:
                backgroundImage.color = BadPositionColor;
                break;
            case 1:
                backgroundImage.color = WorsePositionColor;
                break;
            case 2:
                backgroundImage.color = DefaultColor;
                break;
            case 3:
                backgroundImage.color = AdvPositionColor;
                break;
            case 4:
                backgroundImage.color = FavPositionColor;
                break;
            default:
                backgroundImage.color = DefaultColor;
                break;
        }
    }
}
