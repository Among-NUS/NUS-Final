using System.Collections.Generic;
using UnityEngine;

public class TimedSwitch : RecordableObject
{
    public bool isOn = false;
    public float switchCooldown = 0f;

    protected override Dictionary<string, object> CaptureExtraData()
    {
        return new Dictionary<string, object>
        {
            { "isOn", isOn },
            { "switchCooldown",switchCooldown}
        };
    }

    protected override void RestoreExtraData(Dictionary<string, object> data)
    {
        if (data != null && data.TryGetValue("isOn", out var onObj))
        {
            isOn = (bool)onObj;
        }
        if (data != null && data.TryGetValue("switchCooldown", out var cdObj))
        {
            switchCooldown = System.Convert.ToSingle(cdObj);
        }
    }
}
