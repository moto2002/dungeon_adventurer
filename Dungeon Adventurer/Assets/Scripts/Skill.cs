using System.Collections.Generic; // Import the System.Collections.Generic class to give us access to List<>
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Custom/Skill")]
public class Skill : ScriptableObject
{
    public int cost;
    public bool isPassive;
    public Sprite icon;
    public TargetType targetType;
    public TargetAmount targetCount;

    [HideInInspector]
    public bool[] possiblePositions = new bool[9];
    [HideInInspector]
    public bool[] possibleTargets = new bool[9];
    [HideInInspector]
    public List<SkillEffect> SkillEffects = new List<SkillEffect>();

    void AddNew()
    {
        SkillEffects.Add(new SkillEffect());
    }

    void Remove(int index)
    {
        SkillEffects.RemoveAt(index);
    }

    public void UseSkill(Character[] chars)
    {
        Debug.Log($"using skill against {chars.Length} targets");
        foreach (var charse in chars) {
            foreach (var skill in SkillEffects) {
                skill.effect.UseEffect(charse);
            }
        }
    }

    public bool CheckForPossibleTarget(int pos) { return possibleTargets[pos]; } 
    public bool CheckForPossible(int pos) { return possiblePositions[pos]; }
}
[System.Serializable]
public class SkillEffect
{
    public Effect effect;
}

public enum TargetType
{
    Self,
    Team,
    Enemy
}

public enum TargetAmount
{
    Single,
    Multiple,
    All,
    GlobalAll
}
