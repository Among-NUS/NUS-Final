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

    public ObjectState CaptureState()
    {
        return new ObjectState
        {
            id = uniqueID,
            position = transform.position,
            rotation = transform.rotation,
            scale = transform.localScale
        };
    }

    public void RestoreState(ObjectState state)
    {
        transform.position = state.position;
        transform.rotation = state.rotation;
        transform.localScale = state.scale;
    }

    public static RecordableObject FindByID(string id)
    {
        return allObjects.TryGetValue(id, out var obj) ? obj : null;
    }
}
