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

    public void SpawnPrefab(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Workshop/" + prefabName);
        if (!prefab)
        {
            Debug.LogError($"❌ Prefab {prefabName} 不在 Resources/Prefabs/Workshop/");
            return;
        }

        // 包装 Wrapper
        string wrapperName = prefabName + "_Wrapper";
        GameObject wrapper = new GameObject(wrapperName);
        wrapper.transform.SetParent(spawnParent, true);

        // 放到相机中心
        Vector3 center = gizmoCamera.ViewportToWorldPoint(
            new Vector3(0.5f, 0.5f, gizmoCamera.nearClipPlane + 2f));
        center.z = 1f;
        wrapper.transform.position = center;

        // 实例化 prefab 作为子物体
        GameObject child = Instantiate(prefab, wrapper.transform);
        child.name = prefabName;
        child.transform.localPosition = Vector3.zero;

        // 禁用物理脚本 & 添加碰撞盒
        PhysicsScriptDisabler.Disable(child);
        WrapperColliderUtils.AddBoxColliderToWrapper(wrapper, child);

        // 刷新 UI
        hierarchy?.Refresh();
        inspector?.Inspect(wrapper);

        Debug.Log($"✅ 已生成 {wrapper.name}");
    }

}
