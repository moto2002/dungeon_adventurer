using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonModel
{
    public Dungeon Dungeon => _appliedDungeon;
    public DungeonRoom[] Rooms => _rooms;
    public List<Hero> EnteredHeroes => _enteredHeroes;

    Dungeon _appliedDungeon;
    DungeonRoom[] _rooms;
    RoomPosition _currentPosition;
    DungeonRoom _currentRoom;
    DungeonCreator _creator = new DungeonCreator();
    List<Hero> _enteredHeroes;
    List<DungeonBuff> _dungeonBuffs;
    Dictionary<int, RoomBuff> _roomBuffs;
    Dictionary<int, BattleBuff> _battleBuffs;

    public DungeonModel(List<Hero> enteredHeroes, Dungeon dungeon)
    {
        _appliedDungeon = dungeon;
        _enteredHeroes = enteredHeroes;

        _dungeonBuffs = new List<DungeonBuff>();
        _roomBuffs = new Dictionary<int, RoomBuff>();
        _battleBuffs = new Dictionary<int, BattleBuff>();

        Init();
    }

    public void Init()
    {
        var length = UnityEngine.Random.Range(5, 10);
        var posSideRooms = length - UnityEngine.Random.Range(1, 4);
        _rooms = _creator.CreateDungeon(length, posSideRooms, _appliedDungeon);
        _currentPosition = new RoomPosition(-1, 0);
        _currentRoom = _rooms.FirstOrDefault(e => e.Position.Equals(_currentPosition));
    }

    public void ApplyDungeonBuff(DungeonBuff buff)
    {
        _dungeonBuffs.Add(buff);
    }

    public void ApplyBattleBuff(BattleBuff buff, Hero hero)
    {
        _battleBuffs.Add(hero.id, buff);
    }

    public void ReduceBattleBuffs()
    {
        foreach(var buff in _battleBuffs.ToList())
        {
            buff.Value.battleCount -= 1;
            if(buff.Value.battleCount <= 0)
            {
                _battleBuffs.Remove(buff.Key);
            }
        }
    }

    public void ApplyRoomBuff(RoomBuff buff, Hero hero)
    {
        _roomBuffs.Add(hero.id, buff);
    }

    public void ReduceRoomBuffs()
    {
        foreach (var buff in _roomBuffs.ToList())
        {
            buff.Value.roomCount -= 1;
            if (buff.Value.roomCount <= 0)
            {
                _roomBuffs.Remove(buff.Key);
            }
        }
    }

}

[Serializable]
public abstract class ADungeonBuff
{
    public MainStatChange[] mainBuffs;
    public SubStatChange[] subBuffs;
}

[Serializable]
public class DungeonBuff: ADungeonBuff
{
}

[Serializable]
public class BattleBuff : ADungeonBuff
{
    public int battleCount;
}

[Serializable]
public class RoomBuff : ADungeonBuff
{
    public int roomCount;
}

[Serializable]
public class MainStatChange
{
    public MainStat stat;
    public int flat;
    public float percent;
}

[Serializable]
public class SubStatChange
{
    public SubStat stat;
    public int flat;
    public float percent;
}

public enum BattleTypes
{
    Normal,
    Elite,
    Champion,
    Boss
}