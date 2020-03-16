using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New DisplayCollection", menuName = "Custom/DisplayCollection")]
public class CharacterChangeDisplayCollection : ScriptableObject
{
    [SerializeField] StatusDisplay[] statusData;
    [SerializeField] MarkerDisplay[] markerData;
    [SerializeField] DotDisplay[] dotData;
    [SerializeField] BuffDisplay[] buffData;

    public StatusDisplay GetDataByType(StatusType type)
    {
        return statusData.FirstOrDefault(data => data.type == type);
    }

    public MarkerDisplay GetDataByType(MarkerType type)
    {
        return markerData.FirstOrDefault(data => data.type == type);
    }

    public DotDisplay GetDataByType(DoTDamageType type)
    {
        return dotData.FirstOrDefault(data => data.type == type);
    }

    public BuffDisplay GetDataByType(SubStat type)
    {
        return buffData.FirstOrDefault(data => data.type == type);
    }
}
