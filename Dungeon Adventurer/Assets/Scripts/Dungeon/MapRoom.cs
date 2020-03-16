using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using static DungeonRoom;

public class MapRoom : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject content;
    [SerializeField] TextMeshProUGUI roomHint;
    [SerializeField] GameObject highlight;

    public RoomContent Content => _info.Content;
    public EncounterData Encounter => _info.Encounter;

    DungeonRoom _info;
    bool _interactable;
    Action<RoomPosition> _callback;
    Image _roomImage;

    public void SetData(DungeonRoom roomInfo, Action<RoomPosition> callback)
    {
        _info = roomInfo;
        _callback = callback;
        _roomImage = GetComponent<Image>();

        switch (_info.Content)
        {
            case RoomContent.None:
                roomHint.text = "You hear nothing!";
                break;
            case RoomContent.Battle:
                roomHint.text = "Sounds of Metal";
                break;
            case RoomContent.Obstacle:
                roomHint.text = "The Path is blocked";
                break;
            case RoomContent.Trap:
                roomHint.text = "This seems unsafe";
                break;
            case RoomContent.Generic:
                roomHint.text = "What could be here?";
                break;
            case RoomContent.Gift:
                roomHint.text = "Loot! Loot! Loot!";
                break;
            case RoomContent.Start:
                break;
            case RoomContent.Boss:
                break;
        }
    }

    public List<RoomPosition> GetConnectedRooms() {

        var positions = new List<RoomPosition>();
        foreach (var connection in _info.Connections)
        {
            positions.Add(connection.rooms.FirstOrDefault(e => !e.Position.Equals(_info.Position)).Position);
        }
        return positions;
    }

    public void SetHighlight(bool value)
    {
        highlight.SetActive(value);
    }

    public void SetInteractable(bool value)
    {
        _interactable = value;
        SetHighlight(false);
    }

    public void ShowRoom(bool show)
    {
        content.SetActive(show);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_interactable) _callback?.Invoke(_info.Position);
    }
}