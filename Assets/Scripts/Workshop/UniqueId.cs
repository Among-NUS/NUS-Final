using UnityEngine;
using System;

[DisallowMultipleComponent]
public class UniqueId : MonoBehaviour
{
    [SerializeField] private string id;
    public string Id => id;

    /// <summary>保证存在 GUID（运行时/编辑器都可用）</summary>
    public void EnsureId()
    {
        if (string.IsNullOrEmpty(id))
            id = Guid.NewGuid().ToString();
    }
}
