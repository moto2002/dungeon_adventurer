using UnityEngine;

[CreateAssetMenu(fileName = "New StatusDisplay", menuName = "Custom/StatusDisplay")]
public class StatusDisplay : ScriptableObject
{
    public StatusType type;
    public Sprite statusIcon;
}
