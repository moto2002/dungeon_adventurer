using UnityEngine;
using UnityEngine.EventSystems;

public class CompSlot : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] int position;

    Hero _appliedCharacter;

    public void OnPointerClick(PointerEventData eventData)
    {
        var slot = BattleOrderView._instance.SlotSelected(position);
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
    }


    public void SetData(int pos)
    {
        position = pos;
    }

    public void Refresh(Hero character)
    {
        _appliedCharacter = character;
    }

    public void CleanTransform()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public Hero GetCharacter()
    {
        return _appliedCharacter;
    }

    public int GetPosition()
    {
        return position;
    }
}
