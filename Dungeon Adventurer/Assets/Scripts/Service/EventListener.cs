using UnityEngine;
using UnityEngine.EventSystems;

public class EventListener : UIBehaviour
{
    protected override void OnEnable()
    {
        ServiceRegistry.AddListener(this);
    }

    protected override void OnDisable()
    {
        ServiceRegistry.RemoveListener(this);
    }
}
