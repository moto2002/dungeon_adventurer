using MLAgents;
using UnityEngine;

public class AITrainingAcademy: MonoBehaviour
{
    [SerializeField] MonsterAgent agentPrefab;
    [SerializeField] Monster trainedMonster;
    [SerializeField] AITrainingsField[] fields;

    private void Awake()
    {
        Academy.Instance.OnEnvironmentReset += ResetFields;
    }

    public void OnEnable()
    {
        var skillCount = trainedMonster.Skills.Length;
        for (var i = 0; i < fields.Length; i++)
        {
            fields[i].SetData(this, trainedMonster, trainedMonster.Skills[i % skillCount]);
        }
    }

    void ResetFields()
    {
        foreach (var field in fields)
        {
            field.ResetHeroes();
        }
    }
}