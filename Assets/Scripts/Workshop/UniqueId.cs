using UnityEngine;
using System;

[DisallowMultipleComponent]
public class UniqueId : MonoBehaviour
{
    [SerializeField] private string id;
    public string Id => id;

    /// <summary>��֤���� GUID������ʱ/�༭�������ã�</summary>
    public void EnsureId()
    {
        if (string.IsNullOrEmpty(id))
            id = Guid.NewGuid().ToString();
    }
}
