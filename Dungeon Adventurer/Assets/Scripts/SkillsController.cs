using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsController : ACharacterNavigationEntry
{
    [Header("Skills")]
    [SerializeField] Image[] skills;

    Hero _viewedHero;

    public override void RefreshHero(Hero hero)
    {
        _viewedHero = hero;

        SetSkillImages();
    }

    private void SetSkillImages()
    {
        for (var i = 0; i < skills.Length; i++)
        {
            skills[i].sprite = _viewedHero.Skills[i].icon;
        }
    }
}
