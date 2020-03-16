using UnityEngine;
using UnityEngine.UI;

public class CharacterOverviewView : View, CharactersService.OnCharactersChanged
{
    [SerializeField] SimpleCharacterPlate _characterPlatePrefab;
    [SerializeField] Transform _scrollContent;
    [SerializeField] Button closeButton;

    CharactersModel _heroesModel;

    protected override void Awake()
    {
        closeButton.onClick.AddListener(()=>ViewUtility.ShowThenHide<StartView, CharacterOverviewView>());
    }

    private void MakeCharacterIcons()
    {
        RemoveCharacterOverview();
        for (int i = 0; i < _heroesModel.Characters.Length; i++)
        {
            Hero hero = _heroesModel.Characters[i];
            var plate = Instantiate(_characterPlatePrefab, _scrollContent);
            plate.SetData(hero, () => OpenCharacterView(hero));
        }
    }

    void RemoveCharacterOverview()
    {
        foreach (Transform trans in _scrollContent)
        {
            Destroy(trans.gameObject);
        }
    }

    void OpenCharacterView(Hero hero)
    {
        ViewUtility.ShowThenHide<CharactersView, CharacterOverviewView>(View => View.SetCurrentHero(hero));
    }

    public void OnModelChanged(CharactersModel model)
    {
        _heroesModel = model;
        MakeCharacterIcons();
    }
}
