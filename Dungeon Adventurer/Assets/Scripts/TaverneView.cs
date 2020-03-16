using UnityEngine;
using UnityEngine.UI;

public class TaverneView : View, CurrencyService.OnCurrencyChanged
{
    const int HeroCount = 4;

    [SerializeField] TaverneCharacter prefab;
    [SerializeField] Transform container;
    [SerializeField] Button closeButton;

    CurrencyModel _model;

    protected override void Awake()
    {
        closeButton.onClick.AddListener(()=> ViewUtility.ShowThenHide<StartView, TaverneView>());
    }

    public void OnModelChanged(CurrencyModel model)
    {
        _model = model;
        ClearContainer();
        for (int i = 0; i < HeroCount; i++)
        {
            var character = CharacterCreator.CreateHero();
            var entry = Instantiate(prefab);

            entry.transform.SetParent(container);
            entry.transform.localScale = Vector3.one;
            entry.transform.localPosition = Vector3.one;
            entry.SetData(character, _model);
        }
    }

    void ClearContainer() {

        foreach (Transform trans in container) {
            Destroy(trans.gameObject);
        }
    }
}
