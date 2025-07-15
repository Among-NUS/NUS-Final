using UnityEngine;
using RuntimeInspectorNamespace;

public class WorkshopManager : MonoBehaviour
{
    public string defaultPrefabName = "Circle"; // ✅ 默认 prefab 名
    public GameObject prefabToSpawn;
    public Transform spawnParent;
    public RuntimeHierarchy hierarchy;
    public RuntimeInspector inspector;
    public Camera gizmoCamera;

    public float gridSpacing = 1f;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 20f;

    private Vector3 lastMousePosition;

    private void Start()
    {
        Physics2D.simulationMode = SimulationMode2D.Script;   // 2D 物理不更新
    }

    private void Update()
    {
        // ✅ 滚轮缩放相机
        if (gizmoCamera && gizmoCamera.orthographic)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f)
            {
                gizmoCamera.orthographicSize -= scroll * zoomSpeed;
                gizmoCamera.orthographicSize = Mathf.Clamp(gizmoCamera.orthographicSize, minZoom, maxZoom);
            }
        }

        // ✅ 右键平移相机
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = gizmoCamera.ScreenToWorldPoint(lastMousePosition) - gizmoCamera.ScreenToWorldPoint(lastMousePosition + delta);
            gizmoCamera.transform.position += new Vector3(move.x, move.y, 0);
            lastMousePosition = Input.mousePosition;
        }
    }

    public void Spawn()
    {
        // ✅ 优先用 Dropdown 选中的 prefabToSpawn，否则用默认
        if (prefabToSpawn == null)
        {
            prefabToSpawn = Resources.Load<GameObject>("Prefabs/Workshop/" + defaultPrefabName);
            if (prefabToSpawn == null)
            {
                Debug.LogError($"❌ 找不到默认 Prefab: {defaultPrefabName}");
                return;
            }
        }

        string prefabName = prefabToSpawn.name;
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Workshop/" + prefabName);
        if (prefab == null)
        {
            Debug.LogError($"❌ Prefab {prefabName} 不在 Resources/Prefabs/Workshop/");
            return;
        }

        // ✅ 创建 Wrapper 父物体
        GameObject wrapper = new GameObject(prefabName + "_Wrapper");
        wrapper.transform.SetParent(spawnParent, true);

        // 放到相机中心
        float zOffset = 2f;
        Vector3 center = gizmoCamera.ViewportToWorldPoint(
            new Vector3(0.5f, 0.5f, gizmoCamera.nearClipPlane + zOffset));
        center.z = 1f;
        wrapper.transform.position = center;

        // 实例化实际 prefab 作为子物体
        GameObject child = Instantiate(prefab, wrapper.transform);
        child.name = prefabName;
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;

        // 禁用物理和脚本
        PhysicsScriptDisabler.Disable(child);

        // 用公共工具计算碰撞盒
        WrapperColliderUtils.AddBoxColliderToWrapper(wrapper, child);

        // Wrapper 贴网格
        var lockComp = wrapper.AddComponent<TransformLock2D>();
        lockComp.gridSize = gridSpacing;

        Debug.Log($"✅ 已生成 Wrapper {wrapper.name} + 子物体 {child.name}（从 Resources/Prefabs/Workshop/ 加载）");

        hierarchy?.Refresh();
        inspector?.Inspect(wrapper);
    }

}
