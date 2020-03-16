using UnityEngine;

[CreateAssetMenu(fileName = "New BuffDisplay", menuName = "Custom/BuffDisplay")]
public class BuffDisplay : ScriptableObject
{
    public SubStat type;
    public Sprite statusIcon;
}