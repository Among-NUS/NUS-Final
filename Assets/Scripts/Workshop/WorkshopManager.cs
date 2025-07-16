using UnityEngine;
using RuntimeInspectorNamespace;
using System.Linq;

public class WorkshopManager : MonoBehaviour
{
    public string defaultPrefabName = "Circle";
    public GameObject prefabToSpawn;
    public Transform spawnParent;
    public RuntimeHierarchy hierarchy;
    public RuntimeInspector inspector;
    public Camera gizmoCamera;

    public float gridSpacing = 1f;
    public float zoomSpeed = 5f, minZoom = 2f, maxZoom = 20f;
    Vector3 lastMouse;

    /*──────── 相机缩放 / 平移 ────────*/
    void Update()
    {
        if (gizmoCamera && gizmoCamera.orthographic)
        {
            float sc = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(sc) > 0.001f)
                gizmoCamera.orthographicSize = Mathf.Clamp(
                    gizmoCamera.orthographicSize - sc * zoomSpeed, minZoom, maxZoom);
        }

        if (Input.GetMouseButtonDown(1)) lastMouse = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMouse;
            Vector3 mov = gizmoCamera.ScreenToWorldPoint(lastMouse) -
                            gizmoCamera.ScreenToWorldPoint(lastMouse + delta);
            gizmoCamera.transform.position += new Vector3(mov.x, mov.y, 0);
            lastMouse = Input.mousePosition;
        }
    }

    /*──────── 生成新物体 ────────*/
    public void Spawn()
    {
        if (!prefabToSpawn)
            prefabToSpawn = Resources.Load<GameObject>("Prefabs/Workshop/" + defaultPrefabName);
        if (!prefabToSpawn) { Debug.LogError("❌ Default prefab missing"); return; }

        string baseName = prefabToSpawn.name;
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Workshop/" + baseName);
        if (!prefab) { Debug.LogError("❌ Prefab not in Resources"); return; }

        int nextIdx = spawnParent.Cast<Transform>()
                     .Count(t => t.name.StartsWith(baseName + "_Wrapper_")) + 1;

        string wrapperName = $"{baseName}_Wrapper_{nextIdx}";
        string childName = $"{baseName}_{nextIdx}";

        // Wrapper
        GameObject wrapper = new GameObject(wrapperName);
        wrapper.transform.SetParent(spawnParent, true);

        Vector3 center = gizmoCamera.ViewportToWorldPoint(
            new Vector3(0.5f, 0.5f, gizmoCamera.nearClipPlane + 2f));
        center.z = 1f;
        wrapper.transform.position = center;

        // 子 prefab
        GameObject child = Instantiate(prefab, wrapper.transform);
        child.name = childName;
        child.transform.localPosition = Vector3.zero;

        PhysicsScriptDisabler.Disable(child);
        WrapperColliderUtils.AddBoxColliderToWrapper(wrapper, child);

        hierarchy?.Refresh();
        inspector?.Inspect(wrapper);

        Debug.Log($"✅ 生成 {wrapper.name} + {child.name}");
    }
}
