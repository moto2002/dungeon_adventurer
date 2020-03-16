using UnityEngine;

[CreateAssetMenu(fileName = "New DotDisplay", menuName = "Custom/DotDisplay")]
public class DotDisplay : ScriptableObject
{
    public DoTDamageType type;
    public Sprite statusIcon;
}
