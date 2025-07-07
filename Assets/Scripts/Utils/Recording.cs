using System.Collections.Generic;
using UnityEngine;

public class Snapshot
{
    public Vector3 position;

    public Snapshot(Vector3 pos)
    {
        position = pos;
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
