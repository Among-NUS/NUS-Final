using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectManager : MonoBehaviour
{
    public static DynamicObjectManager Instance { get; private set; }

    List<GameObject> activeObjects = new();                  // 当前活跃的动态对象
    List<DynamicObjectState> savedStates = new();            // 最近一次快照

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    public void Register(GameObject go)
    {
        activeObjects.Add(go);
    }

    public void Unregister(GameObject go)
    {
        activeObjects.Remove(go);
    }

    public void Capture()
    {
        savedStates.Clear();
        foreach (var obj in activeObjects)
        {
            if (obj == null) continue;

            var b = obj.GetComponent<BulletBehaviour>();
            if (b == null) continue;

            savedStates.Add(new DynamicObjectState
            {
                prefabName = b.prefabName,
                position = obj.transform.position,
                rotation = obj.transform.rotation,
                direction = b.direction
            });
        }
    }

    public void Restore()
    {
        // 1⃣ 清场
        foreach (var obj in activeObjects)
            if (obj) Destroy(obj);
        activeObjects.Clear();

        // 2⃣ 复原
        foreach (var s in savedStates)
        {
            var prefab = Resources.Load<GameObject>(s.prefabName);
            if (!prefab)
            {
                Debug.LogWarning($"找不到 prefab: {s.prefabName}");
                continue;
            }

            var go = Instantiate(prefab, s.position, s.rotation);
            var b = go.GetComponent<BulletBehaviour>();
            if (b)
            {
                b.direction = s.direction;
                Register(go);
            }
        }
    }

}
