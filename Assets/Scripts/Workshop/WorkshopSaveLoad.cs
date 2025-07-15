using UnityEngine;
using System.Collections.Generic;
using System.IO;

/*─────────────────────────────────────────────────────────────
 * 数据结构
 *────────────────────────────────────────────────────────────*/
[System.Serializable]
public class ComponentData
{
    public string type;   // 组件类型（含程序集）
    public string path;   // 在 Prefab 层级中的相对路径  (/Child/GrandChild)
    public string json;   // 组件字段序列化后的 Json
}

[System.Serializable]
public class ObjectData
{
    public string prefabName;
    public Vector3 position;
    public float rotationZ;
    public List<ComponentData> comps = new();
}

/*─────────────────────────────────────────────────────────────
 * WorkshopSaveLoad
 *────────────────────────────────────────────────────────────*/
public class WorkshopSaveLoad : MonoBehaviour
{
    public Transform spawnParent;   // SpawnRoot
    public float gridSpacing = 1f;
    public RuntimeInspectorNamespace.RuntimeHierarchy hierarchy;

    /*────────────────────────── 路径工具 ─────────────────────────*/
    string ResolvePath(string fileOrPath)
    {
        return Path.IsPathRooted(fileOrPath)
             ? fileOrPath
             : Path.Combine(Application.dataPath, "Scenes", fileOrPath);
    }

    /*────────────────────────── 保存布局 ─────────────────────────*/
    public void SaveLayout(string fileName)
    {
        string full = ResolvePath(fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(full));

        var list = new List<ObjectData>();

        foreach (Transform wrapper in spawnParent)
        {
            // Wrapper 命名：PrefabName_Wrapper
            string prefabName = wrapper.name.Replace("_Wrapper", "");

            var od = new ObjectData
            {
                prefabName = prefabName,
                position = wrapper.position,
                rotationZ = wrapper.eulerAngles.z
            };

            // 子物体是真实 Prefab 根
            if (wrapper.childCount > 0)
            {
                Transform childRoot = wrapper.GetChild(0);

                // 遍历所有脚本（包括禁用对象）
                foreach (var mb in childRoot.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    // 跳过不需要保存的脚本
                    if (mb is TransformLock2D) continue;

                    od.comps.Add(new ComponentData
                    {
                        type = mb.GetType().AssemblyQualifiedName,
                        path = GetRelativePath(childRoot, mb.transform),
                        json = JsonUtility.ToJson(mb)
                    });
                }
            }

            list.Add(od);
        }

        string jsonAll = JsonUtility.ToJson(new SerializationWrapper<ObjectData>(list), true);
        File.WriteAllText(full, jsonAll);
        Debug.Log("[SaveLayout] " + full);
    }

    /*────────────────────── 加载布局（编辑器） ─────────────────────*/
    public void LoadLayoutForEditor(string fileName)
    {
        string full = ResolvePath(fileName);
        if (!File.Exists(full))
        {
            Debug.LogError("Layout not found: " + full);
            return;
        }

        // 清空旧 Wrapper
        foreach (Transform t in spawnParent)
            Destroy(t.gameObject);

        var wrapperJson = File.ReadAllText(full);
        var dataWrapper = JsonUtility.FromJson<SerializationWrapper<ObjectData>>(wrapperJson);

        if (dataWrapper?.items == null)
        {
            Debug.LogError("Deserialize failed");
            return;
        }

        foreach (var od in dataWrapper.items)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Workshop/" + od.prefabName);
            if (!prefab) { Debug.LogWarning("Missing prefab " + od.prefabName); continue; }

            /* 创建 Wrapper */
            GameObject w = new GameObject(od.prefabName + "_Wrapper");
            w.transform.SetParent(spawnParent, true);
            w.transform.position = od.position;
            w.transform.rotation = Quaternion.Euler(0, 0, od.rotationZ);

            /* 实例化 Prefab */
            GameObject childRoot = Instantiate(prefab, w.transform);
            childRoot.transform.localPosition = Vector3.zero;
            childRoot.transform.localRotation = Quaternion.identity;

            /* 禁用物理脚本 */
            PhysicsScriptDisabler.Disable(childRoot);

            /* 恢复脚本字段 */
            RestoreComponentData(childRoot, od.comps);

            /* 碰撞盒 & 网格锁定 */
            WrapperColliderUtils.AddBoxColliderToWrapper(w, childRoot);
            var lock2D = w.AddComponent<TransformLock2D>();
            lock2D.gridSize = gridSpacing;
        }

        hierarchy?.Refresh();
        Debug.Log("[LoadLayoutForEditor] " + full);
    }

    /*────────────────────── 加载布局（游戏） ─────────────────────*/
    public void LoadLayoutForGame(string fileName)
    {
        string full = ResolvePath(fileName);
        if (!File.Exists(full))
        {
            Debug.LogError("Layout not found: " + full);
            return;
        }

        foreach (Transform t in spawnParent)
            Destroy(t.gameObject);

        var wrapperJson = File.ReadAllText(full);
        var dataWrapper = JsonUtility.FromJson<SerializationWrapper<ObjectData>>(wrapperJson);

        if (dataWrapper?.items == null)
        {
            Debug.LogError("Deserialize failed");
            return;
        }

        foreach (var od in dataWrapper.items)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Workshop/" + od.prefabName);
            if (!prefab) { Debug.LogWarning("Missing prefab " + od.prefabName); continue; }

            GameObject go = Instantiate(prefab, od.position,
                                        Quaternion.Euler(0, 0, od.rotationZ),
                                        spawnParent);

            RestoreComponentData(go, od.comps);
        }

        Debug.Log("[LoadLayoutForGame] " + full);
    }

    /*───────────────── 辅助：写回脚本字段 ─────────────────*/
    static void RestoreComponentData(GameObject root, List<ComponentData> datas)
    {
        if (datas == null) return;

        foreach (var cd in datas)
        {
            var type = System.Type.GetType(cd.type);
            if (type == null) continue;

            Transform target = FindByPath(root.transform, cd.path);
            if (!target) continue;

            var comp = target.GetComponent(type) ?? target.gameObject.AddComponent(type);
            JsonUtility.FromJsonOverwrite(cd.json, comp);
        }
    }

    /* 路径: 从 childRoot 开始的 /Child/GrandChild  */
    static string GetRelativePath(Transform root, Transform target)
    {
        if (target == root) return "/";
        var stack = new Stack<string>();
        var cur = target;
        while (cur != root && cur != null)
        {
            stack.Push(cur.name);
            cur = cur.parent;
        }
        return "/" + string.Join("/", stack.ToArray());
    }

    /* 按路径查找子节点 */
    static Transform FindByPath(Transform root, string path)
    {
        if (path == "/" || string.IsNullOrEmpty(path)) return root;
        string[] seg = path.Split('/');
        Transform cur = root;
        for (int i = 1; i < seg.Length && cur; i++)
            cur = cur.Find(seg[i]);
        return cur;
    }
}
