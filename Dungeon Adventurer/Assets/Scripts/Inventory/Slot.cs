using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{

    public GameObject item
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
        set
        {
            value.transform.SetParent(transform);
            value.transform.localScale = Vector3.one;
            value.transform.localPosition = Vector3.zero;
        }
    }

    public int position;

    public string Description;

    #region IDropHandler implementation
    public void OnDrop(PointerEventData eventData)
    {
        //DragHandler.itemBeingDragged.transform.SetParent(transform);

        //Transform tp_parent = GameObject.FindGameObjectWithTag("CharacterSlots").transform;

        //if (transform.parent.name != "StatPanel" && transform.parent.name != "PositionPanel")
        //{
        //    if (DragHandler.itemBeingDragged.GetComponent<ItemManager>().appliedItem.type != ItemType.Amulet && Description != "Backpack" && Description != "Inventory" && Description != "Shop")
        //    {
        //        return;
        //    }
        //}

        //if (!item && ((DragHandler.itemBeingDragged.GetComponent<DragHandler>().pos && transform.name == "PositionSlot") || (!DragHandler.itemBeingDragged.GetComponent<DragHandler>().pos && transform.name != "PositionSlot")))
        //{
        //    DragHandler.itemBeingDragged.transform.SetParent(transform);

        //    if (transform.name != "PositionSlot")
        //        DragHandler.itemBeingDragged.GetComponent<ItemManager>().appliedItem.id = position;

        //    // check if Position Item being dragged
        //    if (!DragHandler.itemBeingDragged.GetComponent<DragHandler>().pos)
        //    {
        //        if (transform.parent == tp_parent || item.GetComponent<ItemManager>().equipped || transform.parent == GameObject.FindGameObjectWithTag("Backpack").transform)
        //        {

        //            if (item.GetComponent<ItemManager>().equipped && transform.parent != tp_parent)
        //            {
        //                item.GetComponent<ItemManager>().equipped = false;
        //            }
        //            ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged(true, false));
        //            ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged(true, true));
        //        }
        //        else
        //        {
        //            ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged(true, false));
        //        }
        //    }
        //    else
        //    {
        //        ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged(false, true));
        //    }

        //    return;
        //}

        //if (item && !DragHandler.itemBeingDragged.GetComponent<DragHandler>().pos)
        //{

        //    Transform tp_itemparent = DragHandler.startParent;
        //    Transform tp_childItem = transform.GetChild(0);

        //    if (transform.name == "CharacterSlot" || (transform.name != "CharacterSlot" && tp_childItem.GetComponent<ItemManager>().appliedItem.name == tp_itemparent.GetComponent<Slot>().Description))
        //    {
        //        tp_childItem.SetParent(tp_itemparent);
        //        tp_childItem.GetComponent<ItemManager>().appliedItem.id = tp_itemparent.GetComponent<Slot>().position;

        //        DragHandler.itemBeingDragged.transform.SetParent(transform);
        //        DragHandler.itemBeingDragged.GetComponent<ItemManager>().appliedItem.id = position;

        //        ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged(true, false));
        //        ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged(true, true));

        //        if (tp_itemparent.name == "CharacterSlot" || transform.name == "CharacterSlot")
        //            Inventory.k_inventory.UpdateThis();
        //    }
        //    return;
        //}
    }
}
#endregion
