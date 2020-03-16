using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    const int ActiveCount = 4;

    [SerializeField] Transform content;
    [SerializeField] CharacterChangeInfo displayPrefab;

    readonly Dictionary<MarkerType, CharacterChangeInfo> _displayedMarker = new Dictionary<MarkerType, CharacterChangeInfo>();
    readonly Dictionary<DoTDamageType, CharacterChangeInfo> _displayedDots = new Dictionary<DoTDamageType, CharacterChangeInfo>();
    readonly Dictionary<SubStat, CharacterChangeInfo> _displayedBuffs = new Dictionary<SubStat, CharacterChangeInfo>();
    readonly Dictionary<StatusType, CharacterChangeInfo> _displayedStatus = new Dictionary<StatusType, CharacterChangeInfo>();

    public void ShowMarker(MarkerInfo info, MarkerDisplay display)
    {
        if (_displayedMarker.TryGetValue(info.Type, out var displayed))
        {
            if (info.TurnCount <= 0)
            {
                _displayedMarker.Remove(info.Type);
                Destroy(displayed.gameObject);
            }
            else
            {
                displayed.SetData(display.statusIcon, info.TurnCount);
            }
        }
        else
        {
            var marker = Instantiate(displayPrefab, content);
            marker.SetData(display.statusIcon, info.TurnCount);
            _displayedMarker.Add(info.Type, marker);
        }
        RefreshDisplay();
    }

    public void ShowDots(DotInfo info, DotDisplay display)
    {
        if (_displayedDots.TryGetValue(info.Type, out var displayed))
        {
            if (info.TurnCount <= 0)
            {
                _displayedDots.Remove(info.Type);
                Destroy(displayed.gameObject);
            }
            else
            {
                displayed.SetData(display.statusIcon, info.TurnCount);
            }
        }
        else
        {
            var dot = Instantiate(displayPrefab, content);
            dot.SetData(display.statusIcon, info.TurnCount);
            _displayedDots.Add(info.Type, dot);
        }
        RefreshDisplay();
    }

    public void ShowStatus(StatusInfo info, StatusDisplay display)
    {
        if (_displayedStatus.TryGetValue(info.Type, out var displayed))
        {
            if (info.TurnCount <= 0)
            {
                _displayedStatus.Remove(info.Type);
                Destroy(displayed.gameObject);
            }
            else
            {
                displayed.SetData(display.statusIcon, info.TurnCount);
            }
        }
        else
        {
            var status = Instantiate(displayPrefab, content);
            status.SetData(display.statusIcon, info.TurnCount);
            _displayedStatus.Add(info.Type, status);
        }
        RefreshDisplay();
    }

    public void ShowBuff(BuffInfo info, BuffDisplay display)
    {
        if (_displayedBuffs.TryGetValue(info.AffectedStat, out var displayed))
        {
            if (info.TurnCount <= 0)
            {
                _displayedBuffs.Remove(info.AffectedStat);
                Destroy(displayed.gameObject);
            }
            else
            {
                displayed.SetData(display.statusIcon, info.TurnCount);
            }
        }
        else
        {
            var buff = Instantiate(displayPrefab, content);
            buff.SetData(display.statusIcon, info.TurnCount);
            _displayedBuffs.Add(info.AffectedStat, buff);
        }
        RefreshDisplay();
    }

    void RefreshDisplay()
    {
        var changeInfos = _displayedMarker.Values.ToArray();
        _displayedBuffs.Values.ToList().ForEach(buff => changeInfos.Append(buff));
        _displayedDots.Values.ToList().ForEach(dot => changeInfos.Append(dot));
        _displayedStatus.Values.ToList().ForEach(status => changeInfos.Append(status));

        changeInfos.OrderBy(info => info.TurnCount);

        for (var i = 0; i < changeInfos.Length; i++)
        {
            changeInfos[i].gameObject.SetActive(i < ActiveCount);
        }
    }
}
