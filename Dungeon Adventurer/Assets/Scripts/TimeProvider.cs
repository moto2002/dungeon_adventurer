using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimeProvider : UIBehaviour
{
    public interface OnTimeChanged
    {
        string TimeKey { get; }
        void OnTimeChanged(TimeModel model);
    }
    private delegate void OnModelChanged(TimeModel model);
    private event OnModelChanged changedEvent;

    static List<OnTimeChanged> listeners = new List<OnTimeChanged>();
    static Dictionary<string, DateTime> requestedTimes = new Dictionary<string, DateTime>();

    public void RegisterListener(EventListener listener)
    {
        if (listener is OnTimeChanged listen)
        {
            listeners.Add(listen);
            var key = listen.TimeKey;
            if (!requestedTimes.ContainsKey(key))
            {
                requestedTimes.Add(key, !PlayerPrefs.HasKey(key) ? DateTime.UtcNow : DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(key))));
            }
            listen.OnTimeChanged(new TimeModel(DateTime.UtcNow, requestedTimes[listen.TimeKey]));
        }
    }

    public void UnregisterListener(EventListener listener)
    {
        if (listener is OnTimeChanged listen)
        {
            listeners.Remove(listen);
        }
    }

    public static TimeProvider Instance;

    protected override void Awake()
    {
        if (Instance == null)
            Instance = this;

        StartCoroutine(TimeLoop());
    }

    IEnumerator TimeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Publish();
        }
    }

    static void Publish()
    {
        foreach (var listener in listeners)
        {
            listener.OnTimeChanged(new TimeModel(DateTime.UtcNow, requestedTimes[listener.TimeKey]));
        }
    }

    public static void RegisterTime(string key, TimeSpan addedTime)
    {
        var endTime = DateTime.UtcNow.Add(addedTime);
        PlayerPrefs.SetString(key, endTime.ToBinary().ToString());
        if (requestedTimes.ContainsKey(key))
        {
            requestedTimes[key] = endTime;
            Publish();
        } else
        {
            throw new Exception("Trying to register key, without a listener to this key");
        }
    }

    public class TimeModel
    {
        DateTime _currentTime;
        DateTime _endTime;

        public TimeSpan RemainingTime => _endTime.Subtract(_currentTime);

        public TimeModel(DateTime time, DateTime end)
        {
            _currentTime = time;
            _endTime = end;
        }
    }
}
