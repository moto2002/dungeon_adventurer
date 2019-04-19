using System.Collections;
using UnityEngine;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement _manager;

    private void Awake()
    {
        _manager = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Show(string name, Controller controller)
    {
        StartCoroutine(ShowView(name, controller));       
    }

    public void ShowShopView()
    {
        StartCoroutine(ShowView("View_Shop", new CharactersController(DataHolder._data.CharData)));
    }

    public void ShowInventoryView()
    {
        StartCoroutine(ShowView("View_Characters", new CharactersController(DataHolder._data.CharData)));
    }

    public void ShowSmithView()
    {
        StartCoroutine(ShowView("View_Smith", new CharactersController(DataHolder._data.CharData)));
    }

    public void ShowTaverneView()
    {
        StartCoroutine(ShowView("View_Taverne", new CharactersController(DataHolder._data.CharData)));
    }

    public void ShowBattleOrderView()
    {
        StartCoroutine(ShowView("View_BattleOrder", new CharactersController(DataHolder._data.CharData)));
    }

    public IEnumerator ShowView(string name, Controller controller)
    {
        CoroutineWithData cd = new CoroutineWithData(this, UIManager._instance.LoadScene(name, false));
        yield return cd.coroutine;
        View view = (View)cd.result;
        view.Controller = controller;
        view.Show();
    }
}
