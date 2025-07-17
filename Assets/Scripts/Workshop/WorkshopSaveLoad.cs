using UnityEngine;
using System.Collections.Generic;
using System.IO;

/*──────── 数据结构 ────────*/
[System.Serializable] // 只保留 Prefab 名与位置
public class ObjectData
{
    public string prefabName;    // 纯 prefab 名，不含编号
    public Vector3 position;     // 世界坐标
}
/*────────────────────────*/

public class WorkshopSaveLoad : MonoBehaviour
{
    public Transform spawnParent;                                // 生成父节点
    public RuntimeInspectorNamespace.RuntimeHierarchy hierarchy; // 可为空

    /// <summary>
    /// 统一到 persistentDataPath 并自动补 json 后缀
    /// </summary>
    public static string ResolvePath(string f)
    {
        // 如果输入为空，默认 WorkshopLevel.ana
        if (string.IsNullOrEmpty(f))
            f = "WorkshopLevel.ana";

        // ✅ 去掉已有的 .json 或 .ana 后缀，统一换成 .ana
        string noExt = Path.Combine(Path.GetDirectoryName(f) ?? "",
                                    Path.GetFileNameWithoutExtension(f));
        f = noExt + ".ana";

        // ✅ 桌面路径
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        // ✅ 如果传了绝对路径就直接用，否则存桌面
        return Path.IsPathRooted(f)
            ? f
            : Path.Combine(desktopPath, f);
    }


    /*──────── 保存 ────────*/
    public void SaveLayout(string fileName)
    {
        string full = ResolvePath(fileName);

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

        Directory.CreateDirectory(Path.GetDirectoryName(full));
        File.WriteAllText(full,
            JsonUtility.ToJson(new SerializationWrapper<ObjectData>(list), true));

        Debug.Log($"✅ [SaveLayout] 仅保存位置完成 → {full}");
    }

    /*──────── 加载（编辑器 / 游戏） ────────*/
    public void LoadLayout(string fileName, bool editorMode)
    {
        string full = ResolvePath(fileName);
        if (!File.Exists(full))
        {
            Debug.LogError("❌ Layout not found: " + full);
            return;
        }

        // 清空旧的
        foreach (Transform c in spawnParent) Destroy(c.gameObject);

        var data = JsonUtility.FromJson<SerializationWrapper<ObjectData>>(File.ReadAllText(full));
        if (data?.items == null)
        {
            Debug.LogError("❌ Deserialize failed");
            return;
        }

        Dictionary<string, int> counter = new();

        List<GameObject> createdWrappers = new(); // ✅ 记录新 wrapper

        foreach (var od in data.items)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Workshop/" + od.prefabName);
            if (!prefab)
            {
                Debug.LogWarning($"⚠ 缺少 prefab: {od.prefabName}");
                continue;
            }

            if (editorMode)
            {
                int idx = counter.TryGetValue(od.prefabName, out var n) ? n + 1 : 1;
                counter[od.prefabName] = idx;

                string wrapperName = $"{od.prefabName}_Wrapper_{idx}";
                string childName = $"{od.prefabName}_{idx}";

                // ✅ 创建 Wrapper
                var wrapper = new GameObject(wrapperName);
                wrapper.transform.SetParent(spawnParent);
                wrapper.transform.position = od.position;

                // ✅ 实例化 prefab
                var go = Instantiate(prefab, wrapper.transform);
                go.name = childName;
                go.transform.localPosition = Vector3.zero;

                // ✅ 关键逻辑保持一致
                PhysicsScriptDisabler.Disable(go);
                WrapperColliderUtils.AddBoxColliderToWrapper(wrapper, go);
                wrapper.layer = LayerMask.NameToLayer("Default");

                createdWrappers.Add(wrapper); // ✅ 记录
            }
            else
            {
                Instantiate(prefab, od.position, Quaternion.identity, spawnParent);
            }
        }

        hierarchy?.Refresh();

        // ✅ 主动选中第一个 wrapper，让 highlighter 有机会触发
        if (editorMode && createdWrappers.Count > 0 && hierarchy != null)
        {
            hierarchy.Select(createdWrappers[0].transform);  // 直接调用 RuntimeHierarchy.Select
        }

        Debug.Log($"✅ [LoadLayout] 已加载 {data.items.Count} 个物体");
    }


    /* 快捷调用 */
    public void LoadLayoutForEditor(string fileName) => LoadLayout(fileName, true);
    public void LoadLayoutForGame(string fileName) => LoadLayout(fileName, false);
}
