using System;
using UnityEngine;

[CreateAssetMenu]
public class ApplyMarker : Effect
{
    public int markerCount;
    public int turnCount;
    public MarkerType type;

    public override void UseEffect(Character ch)
    {
        ch.AddMarker(type, turnCount);
        Debug.Log($"Apply {markerCount} {type} Markers for {turnCount}");
    }
}

[Serializable]
public enum MarkerType
{
    HuntersMark,
    MarkOfTheDead,
    AssasinationMark
}
