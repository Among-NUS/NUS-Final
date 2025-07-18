using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjectState
{
    public string id;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public Dictionary<string, object> extraData;
}

[System.Serializable]
public struct DynamicObjectState
{
    public string prefabName;      // Resources.Load ·��
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 direction;
    public bool isEnemy;
}

