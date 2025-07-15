// ��������������������������������������������������������������������������������������������������������������������������
// SerializationWrapper.cs
// ���� Unity JsonUtility ���л� List<T>
// ��������������������������������������������������������������������������������������������������������������������������
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
