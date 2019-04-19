using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonView : View
{
    [SerializeField] Button battleButton;


    public override void OnControllerChanged(Controller newController)
    {
        var controller = (DungeonController)newController;
        controller.Init();
        battleButton.onClick.AddListener(controller.StartBattle);
    }
}
