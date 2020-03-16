using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement _manager;

    private void Awake()
    {
        _manager = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowStart()
    {
        ViewUtility.Show<StartView>();
    }

    public void ShowShopView()
    {
        ViewUtility.Show<ShopView>();
    }

    public void ShowInventoryView()
    {
        ViewUtility.ShowThenHide<CharacterOverviewView, StartView>();
    }

    public void ShowSmithView()
    {
        // ViewUtility.Show<ShowSmithView>();
    }

    public void ShowTaverneView()
    {
        ViewUtility.ShowThenHide<TaverneView, StartView>();
    }

    public void ShowBattleOrderView()
    {
        ViewUtility.ShowThenHide<BattleOrderView, StartView>();
    }
}
