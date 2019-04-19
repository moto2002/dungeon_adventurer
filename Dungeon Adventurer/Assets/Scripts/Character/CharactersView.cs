using UnityEngine;
using UnityEngine.UI;

public class CharactersView : View
{
    private CharactersController _controller;

    [SerializeField] CharacterStatsView _statsView;
    [SerializeField] GameObject _characterIconPrefab;
    [SerializeField] Transform _scrollContent;
    [SerializeField] Sprite[] _icons;
    [SerializeField] GameObject characterOverview;
    [SerializeField] InventoryView inventoryView;

    public override void OnControllerChanged(Controller newController)
    {
        _controller = (CharactersController) newController;
        _statsView.Controller = new CharacterController (_controller._model.characters[0]);
        inventoryView.SetData(DataHolder._data.InventoryItems);
        MakeCharacterIcons();
    }

    public void ChangeCharacter(int id)
    {
        _statsView.Controller = new CharacterController(_controller.ChangeCharacterView(id));
    }

    private void MakeCharacterIcons()
    {
        for (int i = 0; i < _controller._model.characters.Length; i++)
        {
            Hero c = _controller._model.characters[i];
            Transform g = (Instantiate(_characterIconPrefab) as GameObject).transform;
            g.SetParent(_scrollContent);
            g.localPosition = Vector3.zero;
            g.GetChild(0).GetComponent<Image>().sprite = DataHolder._data.raceImages[(int)c.race];
            g.GetChild(1).GetComponent<Image>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            AddFunctionToButton(g.GetComponent<Button>(), i);
        }
    }

    public override void Close()
    {
        Debug.Log("herer");
        //UIManager._instance.CloseScene("View_Details");
        base.Close();
    }

    void CloseCharacterOverview()
    {
        characterOverview.SetActive(false);
    }

    private void AddFunctionToButton(Button b, int id)
    {
        b.onClick.AddListener(delegate { ChangeCharacter(id); CloseCharacterOverview(); });
    }


}
