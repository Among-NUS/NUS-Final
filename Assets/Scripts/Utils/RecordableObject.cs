using UnityEngine;
using System;
using System.Collections.Generic;

[ExecuteAlways]               // �� OnValidate �ڱ༭����Ҳִ��
public class RecordableObject : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private string uniqueID;   // ���ٱ�¶�� Inspector

    public string UniqueID => uniqueID;   // ֻ������

    private static readonly Dictionary<string, RecordableObject> allObjects = new();

    /* ---------- �������� ---------- */

    void OnValidate()          // �༭��״̬�£�ÿ�θĶ����ᴥ��
    {
        EnsureID();
    }

    void Awake()               // ����ʱ���ף�ȷ����̬����ʵ��Ҳ�� ID
    {
        EnsureID();

        // ע�ᵽȫ���ֵ�
        if (!allObjects.ContainsKey(uniqueID))
            allObjects.Add(uniqueID, this);
        else
            Debug.LogWarning($"Duplicate ID detected: {uniqueID} on {name}", this);
    }

    /* ---------- ���սӿ� ---------- */

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

    /* ---------- ���߷��� ---------- */

    /// <summary>��֤ <c>uniqueID</c> �ǿ���Ψһ��</summary>
    private void EnsureID()
    {
        if (string.IsNullOrEmpty(uniqueID) || !IsIDUnique(uniqueID))
        {
            uniqueID = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            // ��������ݣ�ȷ������/Ԥ���屣��
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

    /* ---------- �ɱ����าд����չ�� ---------- */

    protected virtual Dictionary<string, object> CaptureExtraData() => null;

    protected virtual void RestoreExtraData(Dictionary<string, object> data) { }

    public static RecordableObject FindByID(string id) =>
        allObjects.TryGetValue(id, out var obj) ? obj : null;
}
