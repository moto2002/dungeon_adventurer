using UnityEngine;

public class ItemInfoView : MonoBehaviour
{
    [SerializeField] ItemInfoStats equippedItemStats;
    [SerializeField] ItemInfoStats selectedItemStats;

    public void SetData(Item item)
    {
        selectedItemStats.SetData(item);
        gameObject.SetActive(true);
    }
}
