using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeExtension
{
    public static string Stringify(this TimeSpan span)
    {
        var rv = string.Empty;
        if(span.TotalDays >= 1)
        {
            rv += $"{span:%d} d ";
        }
        if (span.TotalHours >= 1)
        {
            rv += $"{span:%h} h ";
        }
        if (span.TotalMinutes >= 1)
        {
            rv += $"{span:%m} m ";
        }
        if (span.TotalSeconds >= 0)
        {
            rv += $"{span:%s} s ";
        }

        return rv;
    }

}
