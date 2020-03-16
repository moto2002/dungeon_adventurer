using UnityEngine;

[CreateAssetMenu(fileName = "New MarkerDisplay", menuName = "Custom/MarkerDisplay")]
public class MarkerDisplay : ScriptableObject
{
    public MarkerType type;
    public Sprite statusIcon;
}
