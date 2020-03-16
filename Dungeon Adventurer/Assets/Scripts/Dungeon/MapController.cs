using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField] Transform prefabRow;
    
    [Header("Room Prefabs")]
    [SerializeField] MapRoom prefabStartRoom;
    [SerializeField] MapRoom prefabEndRoom;
    [SerializeField] MapRoom prefabRoom;
    [SerializeField] Transform emptyRoom;

    [Header("Connection Prefabs")]
    [SerializeField] Transform prefabShortConnection;
    [SerializeField] Transform prefabMiddleConnection;
    [SerializeField] Transform prefabLongConnection;
    [SerializeField] Transform connectionContent;
    [SerializeField] Transform content;

    [SerializeField] SpriteRenderer fog;
    [SerializeField] ScrollRect view;

    Dictionary<int, Transform> _createdColumns;
    Dictionary<RoomPosition, MapRoom> _mappedRooms;
    List<RoomConnection> _createdConnections;
    Dictionary<MapRoom, List<GameObject>> _connections = new Dictionary<MapRoom, List<GameObject>>();
    List<MapRoom> _visibleRooms;
    Transform _startRow;
    Transform _endRow;

    Action<MapRoom> _roomCallback;
    Vector2 _oldContentPos = new Vector2(0, 0);
    Vector2 _currentContentPos;
    bool _moving;
    MapRoom _currentMapRoom;
    int _lastCol = 0;
    DungeonRoom[] _currentRooms;
    Rect _contentRect;
    bool _init;
    int _p;

    void Awake()
    {
        view.onValueChanged.AddListener(AdjustOffset);
    }

    void LateUpdate()
    {
        if (!_init && _p >= 2)
        {
            _init = true;
            _contentRect = content.GetComponent<RectTransform>().rect;
            StartCoroutine(CreateConnections(_currentRooms));
        }

        if (_p <= 1) _p++;
    }

    public void SetCallback(Action<MapRoom> callback)
    {
        _roomCallback = callback;
    }

    public void CreateMap(DungeonRoom[] rooms)
    {
        ClearContent();

        _visibleRooms = new List<MapRoom>();
        _currentRooms = rooms;

        var maxHeight = _currentRooms.Max(e => e.Row);
        var height = Mathf.Abs(maxHeight) + Mathf.Abs(_currentRooms.Min(e => e.Row)) + 1;
        _mappedRooms = new Dictionary<RoomPosition, MapRoom>();
        _createdColumns = new Dictionary<int, Transform>();
        _createdConnections = new List<RoomConnection>();

        _startRow = Instantiate(prefabRow, content);
        var startRoom = Instantiate(prefabStartRoom, _startRow);
        var startDungeonRoom = _currentRooms.FirstOrDefault(e => e.Content == RoomContent.Start);
        startRoom.SetData(startDungeonRoom, RoomSelected);
        _mappedRooms.Add(startDungeonRoom.Position, startRoom);
        foreach (var room in _currentRooms)
        {
            if (room.Content == RoomContent.Start || room.Content == RoomContent.Boss) continue;

            var pos = room.Position;

            if (!_createdColumns.TryGetValue(pos.Col, out var column))
            {
                column = Instantiate(prefabRow, content);

                for (int i = 0; i < height; i++)
                {
                    var mapRoom = Instantiate(prefabRoom, column);
                    _mappedRooms.Add(new RoomPosition(pos.Col, maxHeight - i), mapRoom);
                }
                _createdColumns.Add(pos.Col, column);
                _lastCol = pos.Col;
            }
            var mappedRoom = _mappedRooms[pos];
            mappedRoom.SetData(room, RoomSelected);
        }
        _endRow = Instantiate(prefabRow, content);
        var endRoom = Instantiate(prefabEndRoom, _endRow);
        var endDungeonRoom = _currentRooms.FirstOrDefault(e => e.Content == RoomContent.Boss);
        endRoom.SetData(endDungeonRoom, RoomSelected);
        endRoom.ShowRoom(true);
        _mappedRooms.Add(endDungeonRoom.Position, endRoom);

        FillRow(_startRow, startRoom, startDungeonRoom.Position, maxHeight, height);
        FillRow(_endRow, endRoom, endDungeonRoom.Position, maxHeight, height);

        _currentMapRoom = startRoom;
        _init = false;
        _p = 0;
    }

    void GoToStartRoom()
    {
        GoToRoom(_currentMapRoom);
    }

    IEnumerator CreateConnections(DungeonRoom[] rooms)
    {
        yield return new WaitForEndOfFrame();
        foreach (var room in rooms.Reverse())
        {
            foreach (var connection in room.Connections)
            {
                CreateConnections(connection);
            }
        }

        GoToStartRoom();
    }

    void CreateConnections(RoomConnection connection)
    {
        if (_createdConnections.Contains(connection)) return;

        var firstRoom = connection.rooms[0];
        var secondRoom = connection.rooms[1];
        var isVertical = false;
        var isHorizontal = false;
        var angle = 0;
        if (firstRoom.Column > secondRoom.Column)
        {
            angle = 180;
        }
        if (firstRoom.Column != secondRoom.Column)
        {
            if (firstRoom.Row > secondRoom.Row)
            {
                angle = angle == 0 ? angle - 25 : angle + 25;
            } else if (firstRoom.Row < secondRoom.Row)
            {
                angle = angle == 0 ? angle + 25 : angle - 25;
            }
            if (angle == 0 || angle == 180) isHorizontal = true;
        } else
        {
            angle = firstRoom.Row > secondRoom.Row ? angle - 90 : angle + 90;
            isVertical = true;
        }

        var connect = Instantiate(isVertical ? prefabShortConnection : (isHorizontal ? prefabMiddleConnection : prefabLongConnection), connectionContent);
        var firstMapRoom = _mappedRooms[connection.rooms[0].Position];
        connect.position = firstMapRoom.transform.position;
        connect.Rotate(new Vector3(0, 0, angle));
        _createdConnections.Add(connection);
        AddConnection(firstMapRoom, connect.gameObject);
    }

    void AddConnection(MapRoom room, GameObject connection)
    {
        if (_connections.TryGetValue(room, out var connections))
        {
            connections.Add(connection);
        } else
        {
            _connections.Add(room, new List<GameObject>() { connection });
        }
    }

    void ClearContent()
    {
        foreach (Transform t in content.transform)
        {
            if (t == connectionContent) continue;
            Destroy(t.gameObject);
        }

        foreach (Transform s in connectionContent.transform)
        {
            Destroy(s.gameObject);
        }
    }

    void FillRow(Transform row, MapRoom room, RoomPosition position, int maxHeight, int count)
    {
        for (int i = 1; i < count; i++)
        {
            Instantiate(emptyRoom, row);
        }
        room.transform.SetSiblingIndex(maxHeight - position.Row);
    }

    void RoomSelected(RoomPosition pos)
    {
        if (_currentMapRoom == null) throw new ArgumentException("CurrentMapRoom is null");

        SetConnectedRoomsInteractable(false);
        GoToRoom(_mappedRooms[pos]);
    }

    void GoToRoom(MapRoom room)
    {
        _currentMapRoom = room;
        _currentMapRoom.SetHighlight(true);
        SetConnectedRoomsInteractable(true);

        if (_connections.ContainsKey(room))
        {
            foreach (var connection in _connections[room])
            {
                connection.GetComponent<Image>().enabled = true;
            }
        }

        AdjustContent();

        _roomCallback?.Invoke(_currentMapRoom);
        //_currentMapRoom.Encounter.StartEncounter(battleRoom.transform, _her)
        //if (_currentMapRoom.Content == RoomContent.Battle)
        //{
        //    TriggerBattleRoom(true);
        //}

        if (!_visibleRooms.Contains(room))
        {
            _visibleRooms.Add(room);
        }

        //AdjustTex();
    }

    void SetConnectedRoomsInteractable(bool value)
    {
        var interactableRooms = _currentMapRoom.GetConnectedRooms();
        interactableRooms.ForEach(r => {
            if (_mappedRooms.TryGetValue(r, out var mappedRoom))
            {
                mappedRoom.SetInteractable(value);
                mappedRoom.ShowRoom(true);
                if (!_visibleRooms.Contains(mappedRoom))
                {
                    _visibleRooms.Add(mappedRoom);
                }
            }
        });
    }

    public Texture2D DrawCircle(Texture2D tex, Color color, int x, int y, int radius = 3)
    {
        float rSquared = radius * radius;
        float alpha = 0f;

        for (int u = x - radius; u < x + radius + 1; u++)
            for (int v = y - radius; v < y + radius + 1; v++)
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                {
                    color.a = alpha;
                    tex.SetPixel(u, v, color);
                }

        return tex;
    }

    void AdjustContent()
    {
        _currentContentPos = GetRoomPos(_currentMapRoom);
        //@TODO: change Content Movement to transition
        content.localPosition = Vector2.Lerp(_oldContentPos, _currentContentPos, 1.0f);
    }

    Vector2 GetRoomPos(MapRoom room)
    {
        var parent = room.transform.parent;
        var contentheight = content.GetComponent<RectTransform>().rect.height;
        var cameraY = parent.localPosition.y - room.transform.localPosition.y - 240;
        var minY = -(contentheight / 2);
        var maxY = -minY;
        if (cameraY < minY) cameraY = minY;
        else if (cameraY > maxY) cameraY = maxY;

        if (_currentContentPos != null) _oldContentPos = _currentContentPos;
        _currentContentPos = new Vector2(-parent.localPosition.x + 425, cameraY);
        return _currentContentPos;
    }

    void AdjustTex()
    {
        var texSize = new Vector2(((int)_contentRect.width), ((int)_contentRect.height));

        var tex = new Texture2D((int)texSize.x, (int)texSize.y);
        var fillColor = Color.black;
        var fillColorArray = tex.GetPixels();

        var ratio = _contentRect.width / _contentRect.height;

        for (var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = fillColor;
        }
        tex.SetPixels(fillColorArray);

        foreach (var r in _visibleRooms)
        {
            var pos = GetRoomPos(r);
            var ssd = new Vector2((-(pos.x - 425)), (-pos.y));
            tex = DrawCircle(tex, Color.white, (int)ssd.x, (int)ssd.y, 150);
        }
        tex.Apply();

        fog.material.SetTexture("_RoomTexture", tex);
        fog.material.SetVector("_Tiling", new Vector2(854 / _contentRect.width, 480 / _contentRect.height));
    }

    void AdjustOffset(Vector2 a)
    {
        var y = 1 - (480 / _contentRect.height);
        var x = 1 - (854 / _contentRect.width);
        fog.material.SetVector("_Offset", new Vector2(x * a.x, -y / 2 + y * a.y));
    }
}
