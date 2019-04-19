using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeLineController : MonoBehaviour {
    [SerializeField] TimeLineEntry prefabEnemyEntry;
    [SerializeField] TimeLineEntry prefabPlayerEntry;

    [SerializeField] Transform container;

    const float TIMELINE_LENGTH = 550f;
    int maxSpeed = 0;

    Queue<TimeLineEntry> _entries = new Queue<TimeLineEntry>();
    float xPerSpeed = 0f;
    public static TimeLineController controller;

    public void SetData(Hero[] characters, Monster[] monsters) {
        controller = this;
        var entries = new List<TimeLineEntry>();

        foreach (var character in characters) {
            var entry = Instantiate(prefabPlayerEntry, container);
            entry.SetData(DataHolder._data.raceImages[(int)character.race], character.id, character.sub.speed);
            entry.ResetPosition();
            entries.Add(entry);
            character.OnCharacterDeath.AddListener(RemoveEntry);
        }

        foreach (var monster in monsters) {
            var entry = Instantiate(prefabEnemyEntry, container);
            entry.SetData(monster.icon, monster.id, monster.sub.speed);
            entry.ResetPosition();
            entries.Add(entry);
            monster.OnCharacterDeath.AddListener(RemoveEntry);
        }
        entries = entries.OrderByDescending(e => e.Speed).ToList();

        foreach (var entry in entries) {
            _entries.Enqueue(entry);
        }
        SetPosition();
    }

    void SetPosition() {
        /*var dif = 0;
        for (int i = 1; i < _entries.Count; i++)
        {
            if (_entries.ElementAt(i-1).Speed <= _entries.ElementAt(i).Speed)
            {

                dif += 5;
            }
            _entries.ElementAt(i).Speed -= dif;
        }
        if (_entries.LastOrDefault().Speed < 0)
        {
            foreach (var entry in _entries)
            {
                entry.Speed += _entries.LastOrDefault().Speed * -1;
            }
        }

        maxSpeed = _entries.FirstOrDefault().Speed;
        */
        xPerSpeed = TIMELINE_LENGTH / (_entries.Count - 1);

        for (int i = 0; i < _entries.Count; i++) {
            _entries.ElementAt(i).Move(xPerSpeed * i);
        }
    }

    public int FirstEntry() {
        return _entries.First().Id;
    }

    public int NextTurn() {
        var entry = _entries.Dequeue();
        _entries.Enqueue(entry);
        SetPosition();
        return _entries.First().Id;
    }


    void RemoveEntry(int id) {
        var list = _entries.ToList();
        var entry = _entries.FirstOrDefault(e => e.Id == id);
        list.Remove(entry);
        Destroy(entry.gameObject);
        _entries = new Queue<TimeLineEntry>(list);
        SetPosition();
    }
}
