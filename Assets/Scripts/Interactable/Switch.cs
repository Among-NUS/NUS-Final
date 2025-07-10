using System.Collections.Generic;
using UnityEngine;

public class Switch : RecordableObject
{
    public bool isOn = false;

    protected override Dictionary<string, object> CaptureExtraData()
    {
        return new Dictionary<string, object>
        {
            { "isOn", isOn }
        };
    }

    protected override void RestoreExtraData(Dictionary<string, object> data)
    {
        if (data != null && data.TryGetValue("isOn", out var value))
        {
            isOn = (bool)value;
        }
    }
}
