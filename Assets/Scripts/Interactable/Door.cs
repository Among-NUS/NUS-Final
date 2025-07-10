using System.Collections.Generic;
using UnityEngine;

public class Door : RecordableObject
{
    public bool isOpen = false;

    protected override Dictionary<string, object> CaptureExtraData()
    {
        return new Dictionary<string, object>
        {
            { "isOpen", isOpen }
        };
    }

    protected override void RestoreExtraData(Dictionary<string, object> data)
    {
        if (data != null && data.TryGetValue("isOpen", out var value))
        {
            isOpen = (bool)value;
        }
    }
}
