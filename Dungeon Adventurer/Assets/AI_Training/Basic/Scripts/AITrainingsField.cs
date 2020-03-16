using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

//Use this in Anaconda Prompt => activate ml-agents
//mlagents-learn C:\Users\Robin\models\config\trainer_config.yaml --run-id=firstRun --train

public class AITrainingsField : MonoBehaviour
{
    [SerializeField] BattleSlot[] enemySlots;
    [SerializeField] BattleSlot[] heroSlots;
    [SerializeField] MonsterAgent prefab;
    [SerializeField] BattleChar dummyPrefab;
    [SerializeField] CharacterChangeDisplayCollection displayCollection;

    Monster _trainedMonster;
    Skill _trainedSkill;
    AITrainingAcademy _academy;

    public void SetData(AITrainingAcademy academy, Monster trainedMonster, Skill trainedSkill)
    {
        _trainedMonster = trainedMonster;
        _trainedSkill = trainedSkill;
        _academy = academy;

        PositionEnemies();
        PositionHeroes();
    }

    void PositionEnemies()
    {
        foreach (var position in _trainedMonster.GetPossiblePositions())
        {
            if (!_trainedSkill.possibleTargets[position]) continue;
            prefab.SetData(_academy, this, _trainedMonster);
            var agent = Instantiate(prefab, enemySlots[position].transform);
        }
    }

    public void ResetHeroes()
    {
        foreach (var slot in heroSlots)
        {
            if (slot.transform.childCount != 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }
        PositionHeroes();
    }

    void PositionHeroes()
    {
        var heroCount = Random.Range(3, 7);
        var openPositions = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        for (var i = 0; i < heroCount; i++)
        {
            var pos = openPositions[Random.Range(0, openPositions.Count)];
            openPositions.Remove(pos);
            var agent = Instantiate(dummyPrefab, heroSlots[pos].transform);

            var hero = CharacterCreator.CreateHero();

            //var instance = ScriptableObject.CreateInstance(typeof(Monster)) as Monster;
            //instance.SetData(_trainedMonster);
            //instance.coreMaxLife = Random.Range(100, 500);
            agent.SetData(hero, null, displayCollection);
        }
    }

    public float[] GetObservations(BattleSlot parentSlot)
    {
        var obs = new float[10];

        for (var i = 0; i < heroSlots.Length; i++)
        {

            if (heroSlots[i].transform.childCount == 0)
            {
                obs[i] = 0f;
                continue;
            }
            obs[i] = ((Hero)heroSlots[i].transform.GetChild(0).GetComponent<BattleChar>().GetCharacter()).AverageToughness;
        }

        obs[9] = Array.IndexOf(enemySlots, parentSlot);

        return obs;
    }
}
