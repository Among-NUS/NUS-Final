using UnityEngine;
using System.Collections.Generic;

public class RecordableObject : MonoBehaviour
{
    public string uniqueID;

    private static Dictionary<string, RecordableObject> allObjects = new();

    void Awake()
    {
        if (!string.IsNullOrEmpty(uniqueID))
        {
            allObjects[uniqueID] = this;
        }
    }

    public virtual ObjectState CaptureState()
    {
        return new ObjectState
        {
            id = uniqueID,
            position = transform.position,
            rotation = transform.rotation,
            scale = transform.localScale,
            extraData = CaptureExtraData()
        };
    }

    public virtual void RestoreState(ObjectState state)
    {
        transform.position = state.position;
        transform.rotation = state.rotation;
        transform.localScale = state.scale;
        RestoreExtraData(state.extraData);
    }

    protected virtual Dictionary<string, object> CaptureExtraData()
    {
        return null; // 默认无额外数据
    }

    protected virtual void RestoreExtraData(Dictionary<string, object> data)
    {
        // 默认无操作
    }

    public static RecordableObject FindByID(string id)
    {
        return allObjects.TryGetValue(id, out var obj) ? obj : null;
    }
}
