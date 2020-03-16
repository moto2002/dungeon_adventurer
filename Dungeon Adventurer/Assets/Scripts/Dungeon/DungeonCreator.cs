using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonCreator
{
    const float VerticalConnectionChance = 0.75f;

    DungeonRoom startRoom;
    DungeonRoom bossRoom;
    Dictionary<int, List<DungeonRoom>> roomCols = new Dictionary<int, List<DungeonRoom>>();

    public DungeonRoom[] CreateDungeon(int dungeonLength, int additionalRooms, Dungeon dungeon)
    {
        startRoom = new DungeonRoom(-1, 0, dungeon.minLvl, RoomContent.Start);
        var currentRow = 0;
        for (int i = 0; i < dungeonLength; i++)
        {
            var level = Random.Range(dungeon.minLvl, dungeon.maxLvl + 1);
            currentRow = currentRow - 1 + (1 * Random.Range(0, 3));
            var content = (RoomContent)Random.Range(0, 3);
            var room = new DungeonRoom(i, currentRow, level, content);
            var rooms = new List<DungeonRoom>();
            rooms.Add(room);
            roomCols.Add(i, rooms);
        }
        bossRoom = new DungeonRoom(dungeonLength, currentRow, dungeon.maxLvl, RoomContent.Boss);
        CreateAdditionalRooms(additionalRooms, dungeon);
        CreateConnections();

        List<DungeonRoom> returnList = new List<DungeonRoom>();
        returnList.Add(startRoom);
        returnList.Add(bossRoom);
        foreach (var list in roomCols.Values)
        {
            returnList.AddRange(list);
        }
        return returnList.ToArray();
    }

    void CreateAdditionalRooms(int count, Dungeon dungeon)
    {
        var possibleRooms = new Dictionary<int, List<DungeonRoom>>(roomCols);
        for (int i = 0; i < count; i++)
        {
            var level = Random.Range(dungeon.minLvl, dungeon.maxLvl + 1);
            var col = Random.Range(0, possibleRooms.Keys.Count);
            var keys = possibleRooms.Keys.ToArray();
            var key = keys[col];
            var existingRoom = possibleRooms[key][0];
            var row = existingRoom.Row - 1 + (2 * Random.Range(0, 2));

            var content = (RoomContent)Random.Range(0, 3);
            var room = new DungeonRoom(key, row, level, content);
            roomCols[key].Add(room);
            possibleRooms.Remove(key);
        }
    }

    void CreateConnections()
    {
        MakeConnection(startRoom, roomCols[0][0]);
        for (int i = 0; i < roomCols.Keys.Count; i++)
        {
            if (roomCols.ContainsKey(i + 1))
            {
                MakeConnection(roomCols[i][0], roomCols[i + 1][0]);
            }

            if (roomCols[i].Count > 1)
            {
                if (roomCols.TryGetValue(i - 1, out var listBack))
                {
                    listBack.ForEach(room => {
                        if (room.Column == roomCols[i][1].Column)
                        {
                            MakeConnection(room, roomCols[i][1]);
                        }
                    });
                }
                if (roomCols.TryGetValue(i + 1, out var listFront))
                {
                    listFront.ForEach(room => {
                        if (room.Column == roomCols[i][1].Column)
                        {
                            MakeConnection(room, roomCols[i][1]);
                        }
                    });
                }
                if (roomCols[i][1].Connections.Count == 0 || Random.value < VerticalConnectionChance)
                {
                    MakeConnection(roomCols[i][0], roomCols[i][1]);
                }
            }
        }
        MakeConnection(roomCols[bossRoom.Column - 1][0], bossRoom);
    }

    void MakeConnection(DungeonRoom first, DungeonRoom second)
    {
        var connection = new RoomConnection(new[] { first, second });
        first.AddConnection(connection);
        second.AddConnection(connection);
    }
}

public struct RoomPosition
{
    public int Row;
    public int Col;

    public RoomPosition(int col, int row)
    {
        Row = row;
        Col = col;
    }

    public void MoveTo(RoomPosition newPos)
    {
        Row = newPos.Row;
        Col = newPos.Col;
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(RoomPosition)) return false;
        var otherRoom = (RoomPosition)obj;
        return otherRoom.Col == Col && otherRoom.Row == Row;
    }

    public override int GetHashCode()
    {
        var hashCode = 1084646500;
        hashCode = hashCode * -1521134295 + Row.GetHashCode();
        hashCode = hashCode * -1521134295 + Col.GetHashCode();
        return hashCode;
    }
}

public struct RoomConnection
{
    public DungeonRoom[] rooms;
    public Obstacle obstacle;
    public bool obstacleCompleted;

    public RoomConnection(DungeonRoom[] rooms, Obstacle obstacle = Obstacle.None, bool obstacleCompleted = false)
    {
        this.rooms = rooms;
        this.obstacle = obstacle;
        this.obstacleCompleted = obstacleCompleted;
    }
}

public enum Obstacle
{
    None,
    Wall,
    Mud,
    Bush
}

public enum RoomContent
{
    None,
    Battle,
    Gift,
    Generic,
    Obstacle,
    Trap,
    Start,
    Boss,
    SkillCheck
}

public class DungeonRoom
{
    public class EncounterData
    {
        public AEncounter encounter;
        public int level;
        public Rarity rarity;
    }

    List<RoomConnection> _connections;
    readonly RoomPosition _position;
    RoomContent _content;
    EncounterData _encounter;

    public List<RoomConnection> Connections => _connections;
    public int Column => _position.Col;
    public int Row => _position.Row;
    public RoomPosition Position => _position;
    public RoomContent Content => _content;
    public EncounterData Encounter => _encounter;

    public DungeonRoom(int col, int row, int level, RoomContent content = RoomContent.None)
    {
        _position = new RoomPosition(col, row);
        _content = content;
        _connections = new List<RoomConnection>();

        if (content == RoomContent.Start || content == RoomContent.Boss || content == RoomContent.None) return;
        _encounter = new EncounterData()
        {
            encounter = DataHolder._data.Encounters.GetRandomEncounter(_content),
            level = level,
            rarity = (Rarity)Random.Range(0, 5)
        };
    }

    public string PositionToString()
    {
        return $"({_position.Col}/{_position.Row})";
    }

    public void AddConnection(RoomConnection connection)
    {
        _connections.Add(connection);
    }

}
