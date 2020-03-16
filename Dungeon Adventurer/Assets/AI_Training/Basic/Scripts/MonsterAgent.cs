using MLAgents;
using UnityEngine;
using UnityEngine.UI;

public class MonsterAgent  : Agent
{
    [Header("Specific to Basic")]

    [SerializeField] Image monsterIcon;
    public AITrainingAcademy _academy;
    public Monster _trainedMonster;
    public AITrainingsField _field;
    public BrainParameters brainParams;

    public void SetData(AITrainingAcademy academy, AITrainingsField field, Monster trainedMonster)
    {
        _academy = academy;
        _trainedMonster = trainedMonster;
        _field = field;
        monsterIcon.sprite = _trainedMonster.icon;
        brainParams.vectorActionSize[0] = _trainedMonster.coreSkills.Length;
    }

    public override void InitializeAgent()
    {
    }

    public override void CollectObservations()
    {
        var slot = transform.parent.GetComponent<BattleSlot>();
        AddVectorObs(_field.GetObservations(slot));
    }

    public override void AgentAction(float[] vectorAction)
    {
        Debug.Log(vectorAction[0]);
        AddReward(0.2f * vectorAction[0]);
    }

    public override void AgentReset()
    {

    }
}
