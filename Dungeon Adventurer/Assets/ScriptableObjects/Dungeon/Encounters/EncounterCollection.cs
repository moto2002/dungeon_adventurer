using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EncounterCollection", menuName = "Custom/EncounterCollection")]
public class EncounterCollection : ScriptableObject
{
    [Header("SkillEncounter")]
    [SerializeField] AEncounter[] skillEncounters;

    [Header("ObstacleEncounter")]
    [SerializeField] AEncounter[] obstacleEncounters;

    [Header("BattleEncounter")]
    [SerializeField] AEncounter[] battleEncounter;

    [Header("TrapEncounter")]
    [SerializeField] AEncounter[] trapEncounter;

    [Header("GenericEncoutner")]
    [SerializeField] AEncounter[] genericEncounter;

    [Header("GiftEncounter")]
    [SerializeField] AEncounter[] giftEncounter;

    public AEncounter GetRandom()
    {
        var encounters = new List<AEncounter>(skillEncounters);
        encounters.AddRange(obstacleEncounters);
        return encounters[Random.Range(0, encounters.Count)];
    }

    public AEncounter GetRandomEncounter(RoomContent content)
    {
        switch (content)
        {
            case RoomContent.Obstacle: return obstacleEncounters[Random.Range(0, obstacleEncounters.Length)];
            case RoomContent.SkillCheck: return skillEncounters[Random.Range(0, skillEncounters.Length)];
            case RoomContent.Battle: return battleEncounter[Random.Range(0, battleEncounter.Length)];
            case RoomContent.Trap: return trapEncounter[Random.Range(0, trapEncounter.Length)];
            case RoomContent.Generic: return genericEncounter[Random.Range(0, genericEncounter.Length)];
            case RoomContent.Gift: return giftEncounter[Random.Range(0, giftEncounter.Length)];
        }
        return GetRandom();
    }
}
