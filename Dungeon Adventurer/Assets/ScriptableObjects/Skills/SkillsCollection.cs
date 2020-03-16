#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#endif
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New SkillsCollection", menuName = "Custom/SkillsCollection")]
public class SkillsCollection : ScriptableObject
{
    [SerializeField] FightClass fightClass;
    [SerializeField] Skill[] skills;

    public FightClass FightClass => fightClass;
    public Skill[] Skills => skills;

    public Skill GetSkill(int id)
    {
        return skills.FirstOrDefault(skills => skills.id == id);
    }

#if UNITY_EDITOR
    public void AddSkill(Skill newSkill)
    {
        skills = new List<Skill>(skills) { newSkill }.ToArray();
        EditorUtility.SetDirty(this);
    }

    public void RemoveSkill(Skill removedSkill)
    {
        var skillList = new List<Skill>(skills);
        skillList.Remove(removedSkill);
        skills = skillList.ToArray();
        EditorUtility.SetDirty(this);
    }
#endif
}
