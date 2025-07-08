using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Snapshot
{
    public Vector3 playerPosition;
    public List<ObjectState> objectStates;

    // 空构造函数，不再包含 Unity API 调用
    public Snapshot() { }

    // 显式初始化方法，在合适的时机（如 Start）调用
    public void Capture()
    {
        GameObject hero = GameObject.FindWithTag("Player");
        playerPosition = hero ? hero.transform.position : Vector3.zero;

        objectStates = new List<ObjectState>();
        foreach (var obj in GameObject.FindObjectsOfType<RecordableObject>())
            objectStates.Add(obj.CaptureState());

        DynamicObjectManager.Instance.Capture();
    }

    public void Restore()
    {
        foreach (var state in objectStates)
        {
            if (state.id == "Player") continue;

            var obj = RecordableObject.FindByID(state.id);
            if (obj != null)
            {
                obj.RestoreState(state);
            }
        }

        DynamicObjectManager.Instance.Restore();
    }
}




public class Record
{
    public List<char> keys;

    public Record(List<char> keysThisFrame)
    {
        keys = keysThisFrame;
    }
}

