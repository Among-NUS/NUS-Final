using UnityEngine;
using System.Collections.Generic;
using System.IO;

/*──────── 数据结构 ────────*/
[System.Serializable]                    // 只保留 Prefab 名与位置
public class ObjectData
{
    public string prefabName;            // 纯 prefab 名，不含编号
    public Vector3 position;             // 世界坐标
}
/*────────────────────────*/

public class WorkshopSaveLoad : MonoBehaviour
{
    public Transform spawnParent;                                // 生成父节点
    public RuntimeInspectorNamespace.RuntimeHierarchy hierarchy; // 可为空

    static string ResolvePath(string f) =>
        Path.IsPathRooted(f) ? f : Path.Combine(Application.dataPath, "Scenes", f);

    /*──────── 保存 ────────*/
    public void SaveLayout(string file)
    {
        var list = new List<ObjectData>();

        foreach (Transform wrapper in spawnParent)
        {
            if (wrapper.childCount == 0) continue;
            Transform child = wrapper.GetChild(0);

            // 只取 “Circle_1”→“Circle” 这样的前缀
            string baseName = child.name.Split('_')[0];

            list.Add(new ObjectData
            {
                prefabName = baseName,
                position = wrapper.position
            });
        }

        string full = ResolvePath(file);
        Directory.CreateDirectory(Path.GetDirectoryName(full));
        File.WriteAllText(full,
            JsonUtility.ToJson(new SerializationWrapper<ObjectData>(list), true));

        Debug.Log($"✅ [SaveLayout] 仅保存位置完成 → {full}");
    }

    /*──────── 加载（编辑器 / 游戏） ────────*/
    public void LoadLayout(string file, bool editorMode)
    {
        string full = ResolvePath(file);
        if (!File.Exists(full)) { Debug.LogError("❌ Layout not found"); return; }

        foreach (Transform c in spawnParent) Destroy(c.gameObject);

        var data = JsonUtility.FromJson<SerializationWrapper<ObjectData>>(File.ReadAllText(full));
        if (data?.items == null) { Debug.LogError("❌ Deserialize failed"); return; }

        // 计数器：用于给编辑器模式生成唯一的 Wrapper/子物体名
        Dictionary<string, int> counter = new();

        foreach (var od in data.items)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Workshop/" + od.prefabName);
            if (!prefab) { Debug.LogWarning($"⚠ 缺少 prefab: {od.prefabName}"); continue; }

            if (editorMode)
            {
                int idx = counter.TryGetValue(od.prefabName, out var n) ? n + 1 : 1;
                counter[od.prefabName] = idx;

                string wrapperName = $"{od.prefabName}_Wrapper_{idx}";
                string childName = $"{od.prefabName}_{idx}";

                var wrapper = new GameObject(wrapperName);
                wrapper.transform.SetParent(spawnParent);
                wrapper.transform.position = od.position;

                var go = Instantiate(prefab, wrapper.transform);
                go.name = childName;
                go.transform.localPosition = Vector3.zero;

                PhysicsScriptDisabler.Disable(go);
                WrapperColliderUtils.AddBoxColliderToWrapper(wrapper, go);
            }
            else
            {
                Instantiate(prefab, od.position, Quaternion.identity, spawnParent);
            }
        }

        hierarchy?.Refresh();
        Debug.Log($"✅ [LoadLayout] 已加载 {data.items.Count} 个物体");
    }

    /* 快捷调用 */
    public void LoadLayoutForEditor(string file) => LoadLayout(file, true);
    public void LoadLayoutForGame(string file) => LoadLayout(file, false);
}
