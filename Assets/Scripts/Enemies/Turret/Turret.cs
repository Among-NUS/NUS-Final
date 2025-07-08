using System.Collections.Generic;
using UnityEngine;

public class Turret : RecordableObject
{
    public bool isAlive = true;

    protected override Dictionary<string, object> CaptureExtraData()
    {
        return new Dictionary<string, object>
        {
            { "isAlive", isAlive }
        };
    }

    protected override void RestoreExtraData(Dictionary<string, object> data)
    {
        if (data != null && data.TryGetValue("isAlive", out var value))
        {
            isAlive = (bool)value;
        }
        Debug.Log(isAlive);
    }
}
