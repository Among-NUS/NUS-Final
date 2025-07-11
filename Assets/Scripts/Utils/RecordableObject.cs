using UnityEngine;
using System;
using System.Collections.Generic;

[ExecuteAlways]               // 让 OnValidate 在编辑器下也执行
public class RecordableObject : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private string uniqueID;   // 不再暴露给 Inspector

    public string UniqueID => uniqueID;   // 只读访问

    private static readonly Dictionary<string, RecordableObject> allObjects = new();

    /* ---------- 生命周期 ---------- */

    void OnValidate()          // 编辑器状态下，每次改动都会触发
    {
        EnsureID();
    }

    void Awake()               // 运行时兜底，确保动态生成实例也有 ID
    {
        EnsureID();

        // 注册到全局字典
        if (!allObjects.ContainsKey(uniqueID))
            allObjects.Add(uniqueID, this);
        else
            Debug.LogWarning($"Duplicate ID detected: {uniqueID} on {name}", this);
    }

    /* ---------- 快照接口 ---------- */

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

    /* ---------- 工具方法 ---------- */

    /// <summary>保证 <c>uniqueID</c> 非空且唯一。</summary>
    private void EnsureID()
    {
        if (string.IsNullOrEmpty(uniqueID) || !IsIDUnique(uniqueID))
        {
            uniqueID = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            // 标记脏数据，确保场景/预制体保存
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    private static bool IsIDUnique(string id) =>
        !string.IsNullOrEmpty(id) && !allObjects.ContainsKey(id);

#if UNITY_EDITOR
    [ContextMenu("Regenerate ID")]
    private void RegenerateID()
    {
        uniqueID = string.Empty;
        EnsureID();
    }
#endif

    /* ---------- 可被子类覆写的扩展点 ---------- */

    protected virtual Dictionary<string, object> CaptureExtraData() => null;

    protected virtual void RestoreExtraData(Dictionary<string, object> data) { }

    public static RecordableObject FindByID(string id) =>
        allObjects.TryGetValue(id, out var obj) ? obj : null;
}
