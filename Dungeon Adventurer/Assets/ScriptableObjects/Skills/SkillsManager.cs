using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New SkillManager", menuName = "Custom/SkillManager")]
public class SkillsManager : ScriptableObject
{
    [SerializeField] SkillsCollection[] collections;

    public SkillsCollection[] Collections => collections;
    public SkillsCollection GetCollectionByClass(FightClass fightClass)
    {
        return collections.FirstOrDefault(col => col.FightClass == fightClass);
    }

    public Skill GetSkill(FightClass fightClass, int id)
    {
        var tempSkill = ScriptableObject.CreateInstance<Skill>();
        tempSkill.CopyFrom(collections.First(col => col.FightClass == fightClass).GetSkill(id));

        return tempSkill;
    }

    public Skill GetRandomSkill(FightClass fightClass)
    {
        var collection = collections.FirstOrDefault(col => col.FightClass == fightClass);
        var skillCount = collection.Skills.Count();
        return collection.Skills[UnityEngine.Random.Range(0, skillCount)];
    }
}
