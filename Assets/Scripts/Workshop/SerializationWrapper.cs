// ─────────────────────────────────────────────────────────────
// SerializationWrapper.cs
// 用于 Unity JsonUtility 序列化 List<T>
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

[Serializable]
public class SerializationWrapper<T>
{
    public List<T> items;

    public SerializationWrapper(List<T> items)
    {
        this.items = items;
    }
}
