using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterCreationData", menuName = "Custom/CharacterCreationData")]
public class CharacterCreationData: ScriptableObject
{
    public Rarity rarity;
    public int startingPoints;
    public int levelPoints;
}
