using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleEncounterController : UIBehaviour
{
    [SerializeField] Button battleButton; 
    [SerializeField] Button cancelButton;

    protected override void Awake()
    {
        battleButton.onClick.AddListener(ServiceRegistry.Dungeon.StartBattle);
        cancelButton.onClick.AddListener(CloseEncounter);
    }

    public void CloseEncounter()
    {
        gameObject.SetActive(false);
    }
}
