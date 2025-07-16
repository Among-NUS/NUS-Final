using UnityEngine;
using System.Collections.Generic;
using System.IO;

/*──────── 数据结构 ────────*/
[System.Serializable]
public class ComponentData
{
    public string type;
    public string json;
}

[System.Serializable]
public class ObjectData
{
    public string prefabName;  // 带编号
    public string uniqueId;    // GUID
    public Vector3 position;
    public float rotationZ;
    public List<ComponentData> comps = new();
}
/*────────────────────────*/
public class WorkshopSaveLoad : MonoBehaviour
{
    public Transform spawnParent;
    public RuntimeInspectorNamespace.RuntimeHierarchy hierarchy;

    static string ResolvePath(string f) =>
        Path.IsPathRooted(f) ? f : Path.Combine(Application.dataPath, "Scenes", f);

    /*──────── 保存 ────────*/
    public void SaveLayout(string file)
    {
        var list = new List<ObjectData>();

        foreach (Transform wrapper in spawnParent)
        {
            if (wrapper.childCount == 0) continue;
            Transform childRoot = wrapper.GetChild(0);

            var uid = childRoot.GetComponentInChildren<UniqueId>();
            if (uid)
            {
                uid.EnsureId();                     // ★ 保存前强制生成 GUID
            }

            var od = new ObjectData
            {
                prefabName = childRoot.name,
                uniqueId = uid ? uid.Id : "",
                position = wrapper.position,
                rotationZ = wrapper.eulerAngles.z
            };

            foreach (var mb in childRoot.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (mb is TransformLock2D) continue;

                od.comps.Add(new ComponentData
                {
                    type = mb.GetType().AssemblyQualifiedName,
                    json = JsonUtility.ToJson(mb)
                });
            }
            list.Add(od);
        }

        string full = ResolvePath(file);
        Directory.CreateDirectory(Path.GetDirectoryName(full));
        File.WriteAllText(full,
            JsonUtility.ToJson(new SerializationWrapper<ObjectData>(list), true));
        Debug.Log($"✅ [SaveLayout] 已保存 {full}");
    }


    /*──────── 加载（编辑器 / 游戏） ────────*/
    public void LoadLayout(string file, bool editorMode)
    {
        string full = ResolvePath(file);
        if (!File.Exists(full)) { Debug.LogError("❌ Layout not found"); return; }

        foreach (Transform c in spawnParent) Destroy(c.gameObject);

        var data = JsonUtility.FromJson<SerializationWrapper<ObjectData>>(File.ReadAllText(full));
        if (data?.items == null) { Debug.LogError("❌ Deserialize failed"); return; }

        Dictionary<string, GameObject> map = new();

        foreach (var od in data.items)
        {
            string baseName = od.prefabName.Split('_')[0];
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Workshop/" + baseName);
            if (!prefab) { Debug.LogWarning("⚠ Missing prefab " + baseName); continue; }

            GameObject go, wrapper = null;
            if (editorMode)
            {
                wrapper = new GameObject(od.prefabName.Replace(baseName, baseName + "_Wrapper"));
                wrapper.transform.SetParent(spawnParent);
                wrapper.transform.position = od.position;
                wrapper.transform.rotation = Quaternion.Euler(0, 0, od.rotationZ);

                go = Instantiate(prefab, wrapper.transform);
                go.transform.localPosition = Vector3.zero;
            }
            else
            {
                go = Instantiate(prefab, od.position,
                        Quaternion.Euler(0, 0, od.rotationZ), spawnParent);
            }

            go.name = od.prefabName;

            // ★ 保证 UniqueId 正确
            var uid = go.GetComponentInChildren<UniqueId>();
            if (!uid) uid = go.AddComponent<UniqueId>();

            if (!string.IsNullOrEmpty(od.uniqueId))
            {
                // 用 JSON 中的 GUID 覆盖
                typeof(UniqueId).GetField("id",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(uid, od.uniqueId);
            }
            else
            {
                // JSON 里为空 → 运行时生成
                uid.EnsureId();
            }

            map[uid.Id] = go;

            if (editorMode)
            {
                PhysicsScriptDisabler.Disable(go);
                WrapperColliderUtils.AddBoxColliderToWrapper(wrapper, go);
                wrapper.gameObject.AddComponent<TransformLock2D>().gridSize = 1f;
            }
        }

        // 字段恢复
        foreach (var od in data.items)
        {
            if (map.TryGetValue(string.IsNullOrEmpty(od.uniqueId) ? null : od.uniqueId, out var go))
            {
                foreach (var cd in od.comps)
                {
                    var t = System.Type.GetType(cd.type);
                    if (t == null) continue;
                    var comp = go.GetComponentInChildren(t, true) ?? go.AddComponent(t);
                    JsonUtility.FromJsonOverwrite(cd.json, comp);
                }
            }
        }

        hierarchy?.Refresh();
        Debug.Log($"✅ [LoadLayout] {full}");
    }


    /* 快捷调用 */
    public void LoadLayoutForEditor(string file) => LoadLayout(file, true);
    public void LoadLayoutForGame(string file) => LoadLayout(file, false);
}
